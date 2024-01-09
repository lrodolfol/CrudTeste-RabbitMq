using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Hosting;
using CreateUser.Infrastructure.MessageBrocker;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using CreateUser.Core.Entities;
using CreateUser.Core.Services;
using Serilog;

namespace CreateUser.Infrastructure;

public class RabbitMqConsumerReply : BackgroundService
{
    private IModel _model = null!;
    private readonly IConnection _connection;
    private readonly IConfiguration _configuration;
    private DefaultConfigs _defaultConfigs;
    private readonly ILogger _logger;

    public RabbitMqConsumerReply(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;

        SetDefaultConfigs();

        _logger.Information($"Try connection with {_defaultConfigs.HostName}");

        var factory = new ConnectionFactory()
        {
            HostName = _defaultConfigs.HostName,
            UserName = _defaultConfigs.UserName,
            Password = _defaultConfigs.Password,
            Port = _defaultConfigs.Port
        };

        _connection = factory.CreateConnection();
        _model = _connection.CreateModel();
        _model.BasicQos(0, 1, false);

        DeclareQueue();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_model);

        consumer.Received += (model, ea) =>
        {
            try
            {
                var passReturn = ReadMessage(ea);
                if (passReturn != string.Empty)
                {
                    SendReplyMessage(passReturn, ea);
                    _model.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    throw new InvalidOperationException("Fail in RPC message");
                }
            }
            catch (Exception)
            {
                _logger.Error("Processing rpc message failure");
                _model.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _model.BasicConsume(queue: _defaultConfigs.Queue,
                             autoAck: false,
                             consumer: consumer);

        return Task.CompletedTask;
    }

    private void SendReplyMessage(string passReturn, BasicDeliverEventArgs ea)
    {
        var props = ea.BasicProperties;
        var replyProps = _model.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        var responseBytes = Encoding.UTF8.GetBytes(passReturn);

        _model.BasicPublish(exchange: "", 
            routingKey: props.ReplyTo, 
            basicProperties: replyProps, 
            body: responseBytes);
    }

    private void SetDefaultConfigs()
    {
        _defaultConfigs.HostName = _configuration["MessageBrockerConfigAuth:HostName"]!;
        _defaultConfigs.UserName = _configuration["MessageBrockerConfigAuth:UserName"]!;
        _defaultConfigs.Password = _configuration["MessageBrockerConfigAuth:Password"]!;
        _defaultConfigs.Port = int.Parse(_configuration["MessageBrockerConfigAuth:Port"]!)!;

        _defaultConfigs.Queue = $"{_configuration["MessageBrockerConfigUserQueue:Queue"]}_password"!;
        _defaultConfigs.Exchange = $"{_configuration["MessageBrockerConfigUserQueue:Exchange"]}_password"!;
        _defaultConfigs.RoutingKey = $"{_configuration["MessageBrockerConfigUserQueue:RoutingKeyCreate"]}_password"!;

        _defaultConfigs.Durable = true;
        _defaultConfigs.Transient = false;
        _defaultConfigs.AutoDeleteQueue = false;
        _defaultConfigs.ExclusiveQueue = false;
    }

    private string ReadMessage(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        if (body.Length == 0)
            throw new InvalidDataException("Message with out properties");

        var message = Encoding.UTF8.GetString(body);

        if (string.IsNullOrEmpty(message))
            throw new InvalidDataException("Message body invalid");
        
        var user = JsonSerializer.Deserialize<User>(message);

        var passReturn = CreateUserPassWord.CreatePassword(user!);

        _logger.Information($"Rpc successful process. Password sent to {user.Name}");

        return passReturn;
    }

    private void DeclareQueue()
    {
        _model.QueueDeclare(queue: _defaultConfigs.Queue,
            durable: _defaultConfigs.Durable,
            exclusive: _defaultConfigs.ExclusiveQueue,
            autoDelete: _defaultConfigs.AutoDeleteQueue,
            arguments: null
            );
    }
}
