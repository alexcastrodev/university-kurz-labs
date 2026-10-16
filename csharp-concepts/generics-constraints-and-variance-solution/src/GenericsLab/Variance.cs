namespace GenericsLab;

public class Animal;

public class Dog : Animal;

public interface IProducer<out T>
{
    T Next();
}

public interface IConsumer<in T>
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
