namespace CreateUser.Core.ValueObjects;

public sealed class Email
{
    public string Address { get; set; } = null!;

    public Email(string endereco)
    {
        Address = endereco;
    }
    public Email()
    {
        
    }
}