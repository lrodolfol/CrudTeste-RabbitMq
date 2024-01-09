namespace WorkerCreateUserConsumer.Exceptions;
public class EmailInvalidDataException : Exception
{
    public EmailInvalidDataException(string message = "Email with invalid properties") : base(message) { }
}
