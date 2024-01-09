namespace WorkerCreateUserConsumer.ValueObjects;

public abstract class Entitie<T> where T : new()
{
    public T Id { get; set; }
    public bool Active { get; set; } = true;

    protected Entitie() => Id = new T();
    protected Entitie(T id) => Id = id;
}
public abstract class Entitie : Entitie<Guid>
{
    protected Entitie() => Id = Guid.NewGuid();
}