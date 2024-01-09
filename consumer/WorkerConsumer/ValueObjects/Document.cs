namespace WorkerCreateUserConsumer.ValueObjects;

public sealed class Document
{
    public string Number { get; set; } = null!;

    public Document(string number)
    {
        Number = number;
    }
    public Document()
    {
        
    }
}