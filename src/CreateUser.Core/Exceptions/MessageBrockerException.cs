namespace CreateUser.Core.Exceptions;

public class MessageBrockerException : Exception
{
    public MessageBrockerException(string message = $"Invalid data for Message Brocker propertie") : base(message) { }

    public static void ThrowIfNullOrEmpty(object? propertie, string? keyName = null)
    {
        if (propertie is null)
            throw new MessageBrockerException($"Invalid data for Message Brocker propertie -> {keyName ?? keyName}");
    }
}