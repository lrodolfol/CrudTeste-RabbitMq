using CreateUser.Core.ValueObjects;

namespace CreateUser.Core.Exceptions;

public class DocumentException : Exception
{
    public DocumentException(string message = $"Invalid data for {nameof(Document)}") : base(message) { }

    public static void ThrowIfNullOrEmpty(string phome)
    {
        if (string.IsNullOrEmpty(phome))
            throw new DocumentException();
    }
}