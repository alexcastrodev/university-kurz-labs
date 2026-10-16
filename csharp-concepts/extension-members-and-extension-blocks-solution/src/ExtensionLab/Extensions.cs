namespace ExtensionLab;

public static class SequenceExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public bool IsEmpty => !source.Any();

        public string JoinWith(string separator) => string.Join(separator, source);

        public static IEnumerable<T> operator +(IEnumerable<T> left, IEnumerable<T> right) => left.Concat(right);
    }

    extension(IEnumerable<int>)
    {
        public static IEnumerable<int> Sequence(int start, int count) => Enumerable.Range(start, count);
    }
}

public static class MoneyExtensions
{
    extension(Money)
    {
        public static Money Zero => new(0);

        public static Money operator *(Money left, int factor) => new(left.Amount * factor);
    }
}

public static class TextExtensions
{
    extension(string? text)
    {
        public bool IsBlank => string.IsNullOrWhiteSpace(text);
    }
}

public static class RefExtensions
{
    extension(ref int value)
    {
        public void Increment() => value++;
    }
}

public static class MathExtensions
{
    extension(Math)
    {
        public static double Remap(double v, double fromLo, double fromHi, double toLo, double toHi)
            => toLo + (v - fromLo) * (toHi - toLo) / (fromHi - fromLo);
    }
}
