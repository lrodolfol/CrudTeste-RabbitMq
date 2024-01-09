using CreateUser.Core.ValueObjects;

namespace CreateUser.Core.Exceptions;

public class EmailException : Exception
{
    public EmailException(string message = $"Invalid data for {nameof(Email)}") : base(message) { }

    public static void ThrowIfNullOrEmpty(string phome)
    {
        if (string.IsNullOrEmpty(phome))
            throw new EmailException();
    }
}