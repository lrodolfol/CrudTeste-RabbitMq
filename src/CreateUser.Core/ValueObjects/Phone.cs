using CreateUser.Core.Exceptions;
using CreateUser.Core.ValueObjects;

namespace CreateUser.Core.ValueObjects;

public sealed class Phone : ValueObject
{
    protected override void Validate()
    {
        PhoneException.ThrowIfNullOrEmpty(Number);
    }

    public Phone(string number)
    {
        Number = number;
        Validate();
    }
    public Phone()
    {
        
    }
    public string Number { get; set; } = null!;
}