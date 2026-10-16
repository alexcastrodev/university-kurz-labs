namespace InterfacesLab;

public interface IUnit
{
    static abstract string Symbol { get; }
}

public struct Meter : IUnit
{
    public static string Symbol => "m";
}

public struct Kilogram : IUnit
{
    public static string Symbol => "kg";
}

public static class Units
{
    public static string Describe<T>(double value) where T : IUnit => $"{value} {T.Symbol}";
}

public interface ILeft
{
    string Name => "left";
}

public interface IRight
{
    string Name => "right";
}

public class Both : ILeft, IRight
{
    string ILeft.Name => "left+right";

    string IRight.Name => "right+left";
}
