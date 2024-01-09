using CreateUser.Core.Exceptions;
using CreateUser.Core.ValueObjects;

namespace CreateUser.Core.ValueObjects;

public sealed class Document : ValueObject
{
    protected override void Validate()
    {
        DocumentException.ThrowIfNullOrEmpty(Number);
    }

    public string Number { get; set; } = null!;

    public Document(string number)
    {
        Number = number;
        Validate();
    }
    public Document()
    {
        
    }
}