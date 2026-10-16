namespace GenericsLab;

public class Animal;

public class Dog : Animal;

// TODO-06: A producer of Dog is a producer of Animal. Mark T so the interface is covariant.
public interface IProducer<T>
{
    T Next();
}

// TODO-07: A consumer of Animal is a consumer of Dog. Mark T so the interface is contravariant.
public interface IConsumer<T>
{
    void Accept(T item);
}

public class Kennel : IProducer<Dog>
{
    public Dog Next() => new();
}

public class AnimalLog : IConsumer<Animal>
{
    public List<Animal> Seen { get; } = [];

    public void Accept(Animal item) => Seen.Add(item);
}
