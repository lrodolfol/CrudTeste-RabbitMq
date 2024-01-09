namespace WorkerCreateUserConsumer.Exceptions;
public class MessageBrokerInvalidDataExcepion : Exception
{
    public MessageBrokerInvalidDataExcepion(string message = "Message broker with inválid message") : base(message) { }
}
