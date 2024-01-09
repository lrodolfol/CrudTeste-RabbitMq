namespace WorkerCreateUserConsumer.ValueObjects;

public sealed class Phone
{

    public Phone(string number)
    {
        Number = number;
    }
    public Phone()
    {
        
    }
    public string Number { get; set; } = null!;
}