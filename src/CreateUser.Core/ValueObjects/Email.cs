using CreateUser.Core.Exceptions;
using CreateUser.Core.ValueObjects;

namespace CreateUser.Core.ValueObjects;

public sealed class Email : ValueObject
{
    protected override void Validate()
    {
        EmailException.ThrowIfNullOrEmpty(Address);
    }

    public string Address { get; set; } = null!;

    public Email(string endereco)
    {
        Address = endereco;
        Validate();
    }
    public Email()
    {
        
    }
}