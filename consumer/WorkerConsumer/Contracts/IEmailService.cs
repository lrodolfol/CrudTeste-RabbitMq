namespace WorkerCreateUserConsumer.Contracts;
public interface IEmailService
{
    public Task Send((string toMail, string toName) receiver);
}
