using CreateUser.Core.Entities;
using CreateUser.Core.Exceptions;
using CreateUser.Infrastructure.MessageBrocker;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace CreateUser.Infrastructure;

public class RabbitMqClientePublish
{
    private readonly IModel _model = null!;
    private readonly ConnectionFactory _connectionFactory;
    private readonly IConnection _connection;
    private IBasicProperties _requestProperties = null!;
    private readonly IConfiguration _configuration;
    private DefaultConfigs _defaultConfigs;
    private readonly ILogger _logger;

    public RabbitMqClientePublish(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;

        SetDefaultConfigs();
        ValidateConfig();

        _connectionFactory = new ConnectionFactory()
        {
            HostName = _defaultConfigs.HostName,
            UserName = _defaultConfigs.UserName,
            Password = _defaultConfigs.Password,
            Port = _defaultConfigs.Port
        };

        _connection = _connectionFactory.CreateConnection();
        _model = _connection.CreateModel();

        CreateRouteUser();
        CreateRoutePassword();
    }

    private void ValidateConfig()
    {
        //do reflection
        foreach (var item in _defaultConfigs.GetType().GetProperties())
            MessageBrockerException.ThrowIfNullOrEmpty(item.GetValue(_defaultConfigs), item.Name);
    }

    private void SetDefaultConfigs()
    {
        _defaultConfigs.HostName = _configuration["MessageBrockerConfigAuth:HostName"]!;
        _defaultConfigs.UserName = _configuration["MessageBrockerConfigAuth:UserName"]!;
        _defaultConfigs.Password = _configuration["MessageBrockerConfigAuth:Password"]!;
        _defaultConfigs.Port = int.Parse(_configuration["MessageBrockerConfigAuth:Port"]!)!;

        _defaultConfigs.Queue = _configuration["MessageBrockerConfigUserQueue:Queue"]!;
        _defaultConfigs.Exchange = _configuration["MessageBrockerConfigUserQueue:Exchange"]!;
        _defaultConfigs.RoutingKey = _configuration["MessageBrockerConfigUserQueue:RoutingKeyCreate"]!;

        _defaultConfigs.Durable = true;
        _defaultConfigs.Transient = false;
        _defaultConfigs.AutoDeleteQueue = false;
        _defaultConfigs.ExclusiveQueue = false;
    }

    public async Task<string> SetPassWord(User user)
    {
        if (!user!.IsValid())
            throw new MessageBrockerException($"Invalid Model. Message -> {user.ToString()}");

        var queueAnonymous = DeclareAnonymousQueue();
        var correlationId = Guid.NewGuid().ToString("D");
        var password = string.Empty;

        var tcs = new TaskCompletionSource<string>();

        var consumer = new EventingBasicConsumer(_model);
        consumer.Received += (model, ea) =>
        {
            if (correlationId == ea.BasicProperties.CorrelationId)
            {
                password = ReceiveMessageRpc(ea);
                _model.BasicAck(ea.DeliveryTag, false);
                tcs.SetResult(password);

                return;
            }

            _logger.Warning("Invalid correlatioId for message");
        };
        _model.BasicConsume(queue: queueAnonymous, autoAck: false, consumer: consumer);

        SetRequestProperties(queueAnonymous, correlationId);

        _model.BasicPublish(_defaultConfigs.Exchange,
            _defaultConfigs.RoutingKey,
            _requestProperties,
            GetBody(user)
            );

        _model.ConfirmSelect();

        return await tcs.Task;
    }

    private void SetRequestProperties(QueueDeclareOk queueAnonymous, string correlationId)
    {
        _requestProperties = _model.CreateBasicProperties();
        _requestProperties.MessageId = Guid.NewGuid().ToString("D"); //para que preciso mesmo da messageId ?
        _requestProperties.CorrelationId = correlationId;
        _requestProperties.ReplyTo = queueAnonymous.QueueName;
    }

    private string ReceiveMessageRpc(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        if (string.IsNullOrEmpty(message))
            throw new InvalidDataException("Nullable or empty return rpc message");

        return message;
    }

    private void CreateRouteUser()
    {
        //dead letter has no bind
        _model.ExchangeDeclare(exchange: $"{_defaultConfigs.Exchange}_deadletter", type: ExchangeType.Fanout);
        _model.QueueDeclare($"{_defaultConfigs.Queue}_deadletter", true, false, false, null);
        _model.QueueBind(queue: $"{_defaultConfigs.Queue}_deadletter", exchange: $"{_defaultConfigs.Exchange}_deadletter", "");

        var arguments = new Dictionary<string, object>()
            {
                { "x-dead-letter-exchange", $"{_defaultConfigs.Exchange}_deadletter" }
            };
        _model.QueueDeclare(queue: _defaultConfigs.Queue, durable: _defaultConfigs.Durable, exclusive: _defaultConfigs.ExclusiveQueue, autoDelete: _defaultConfigs.AutoDeleteQueue, arguments: arguments);
        _model.ExchangeDeclare(exchange: _defaultConfigs.Exchange, type: "topic");
        _model.QueueBind(queue: _defaultConfigs.Queue, exchange: _defaultConfigs.Exchange, routingKey: _defaultConfigs.RoutingKey);
    }

    private void CreateRoutePassword()
    {
        _model.QueueDeclare(queue: $"{_defaultConfigs.Queue}_password", durable: _defaultConfigs.Durable, exclusive: _defaultConfigs.ExclusiveQueue, autoDelete: _defaultConfigs.AutoDeleteQueue, arguments: null);
        _model.ExchangeDeclare(exchange: $"{_defaultConfigs.Exchange}_password", type: "topic");
        _model.QueueBind(queue: $"{_defaultConfigs.Queue}_password", exchange: $"{_defaultConfigs.Exchange}_password", routingKey: _defaultConfigs.RoutingKey);

        _model.ExchangeBind($"{_defaultConfigs.Exchange}_password", _defaultConfigs.Exchange, _defaultConfigs.RoutingKey);
    }

    private QueueDeclareOk DeclareAnonymousQueue()
    {
        return _model.QueueDeclare(queue: string.Empty, durable: false, exclusive: true, autoDelete: true, arguments: null);
    }

    private ReadOnlyMemory<byte> GetBody(User user)
    {
        string msgJson = System.Text.Json.JsonSerializer.Serialize(user);
        var body = Encoding.UTF8.GetBytes(msgJson);

        return body;
    }
}