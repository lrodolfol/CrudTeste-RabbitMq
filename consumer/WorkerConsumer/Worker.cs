using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using WorkerCreateUserConsumer.Entities;
using WorkerCreateUserConsumer.Contracts;
using WorkerCreateUserConsumer.Exceptions;

namespace WorkerConsumer;

public class Worker : BackgroundService
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ConnectionFactory _connectionFactory;
    private readonly IConnection _connection;
    private IModel _model = null!;
    private DefaultConfigs _defaultConfigs;
    private readonly Serilog.ILogger _logger;

    public Worker(IConfiguration configuration, IEmailService emailService, Serilog.ILogger logger)
    {
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;

        if (_emailService == null)
            throw new ArgumentNullException($"{nameof(_emailService)} is inválid");

        if (_configuration == null)
            throw new ArgumentNullException($"{nameof(_configuration)} is inválid");

        SetDefaultConfigs();
        _connectionFactory = new ConnectionFactory()
        {
            HostName = _defaultConfigs.HostName,
            UserName = _defaultConfigs.UserName,
            Password = _defaultConfigs.Password,
            Port = _defaultConfigs.Port
        };

        _connection = _connectionFactory.CreateConnection();
        _model = _connection.CreateModel();
        //_model.BasicQos(0, 1, false);

        DeclareQueue();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_model);

        consumer.Received += (model, ea) =>
        {
            try
            {
                ReadMessage(ea);
                _model.BasicAck(ea.DeliveryTag, false);
            }
            catch(MessageBrokerInvalidDataExcepion)
            {
                _logger.Error($"Fail to try get message. Check the deadLetter {_defaultConfigs.Queue}_deadletter");
                _model.BasicNack(ea.DeliveryTag, false, false);
            }
            catch (EmailInvalidDataException)
            {
                _logger.Error($"Fail sendEmail. Check the deadLetter {_defaultConfigs.Queue}_deadletter");
                _model.BasicNack(ea.DeliveryTag, false, false);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failure to process. {ex.Message}");
               _model.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _model.BasicConsume(queue: _defaultConfigs.Queue,
                             autoAck: false,
                             consumer: consumer);

        return Task.CompletedTask;
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

    private void DeclareQueue()
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

    private void ReadMessage(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        if (body.Length == 0)
            return;

        var message = Encoding.UTF8.GetString(body);

        var modelObject = JsonSerializer.Deserialize<User>(message);
        if (!modelObject!.IsValid())
            throw new MessageBrokerInvalidDataExcepion($"The message {message} have invalid information. Check the deadLetter");

        _emailService.Send((modelObject.Email.Address, modelObject.Name));

        _logger.Information($"Message processed successfully and email sent to ${modelObject.Email.Address}");
    }
}
