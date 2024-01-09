using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace CreateUserConsumer;
internal class UserConsumer : BackgroundService
{
    //private IModel model = null!;
    public UserConsumer()
    {
        Console.WriteLine("Consumindo");
        Console.WriteLine("Consumindo");
        Console.WriteLine("Consumindo");
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Consumindo");
        Console.WriteLine("Consumindo");
        Console.WriteLine("Consumindo");
        Console.WriteLine("Consumindo");

        return Task.CompletedTask;
    }

    //private void BindExchangeQueue()
    //{
    //    model.QueueBind(queue: _defaultConfigs.Queue,
    //                    exchange: _defaultConfigs.Exchange,
    //                    routingKey: _defaultConfigs.RoutingKey
    //                    );
    //}

    //private void DeclareQueue()
    //{
    //    model.QueueDeclare(queue: _defaultConfigs.Queue,
    //        durable: _defaultConfigs.Durable,
    //        exclusive: _defaultConfigs.ExclusiveQueue,
    //        autoDelete: _defaultConfigs.AutoDeleteQueue,
    //        arguments: null
    //        );
    //}

    //private void DeclareExchange()
    //{
    //    model.ExchangeDeclare(
    //        exchange: _defaultConfigs.Exchange,
    //        type: "topic"
    //        );
    //}
}
