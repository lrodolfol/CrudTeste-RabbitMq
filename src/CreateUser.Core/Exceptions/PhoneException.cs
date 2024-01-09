using CreateUser.Core.ValueObjects;

namespace CreateUser.Core.Exceptions;

public class PhoneException : Exception
{
    public PhoneException(string message = $"Invalid data for {nameof(Phone)}") : base(message) { }

    public static void ThrowIfNullOrEmpty(string phome)
    {
        if (string.IsNullOrEmpty(phome))
            throw new PhoneException();
    }
}