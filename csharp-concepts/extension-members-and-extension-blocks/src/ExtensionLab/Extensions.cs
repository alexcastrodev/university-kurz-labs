namespace ExtensionLab;

// Extension blocks (C# 14) live in a top-level static class. The members below compile but
// return placeholder values: replace each placeholder with the real logic.
public static class SequenceExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        // TODO-00: IsEmpty is true when the sequence has no elements (use source.Any()).
        public bool IsEmpty => false;

        // TODO-01: JoinWith joins the elements with the separator (string.Join).
        public string JoinWith(string separator) => "";

        // TODO-05 (optional): Add `public static IEnumerable<T> operator +(IEnumerable<T> left,
        //   IEnumerable<T> right)` here so `names + others` concatenates the two sequences.
    }

    extension(IEnumerable<int>)
    {
        // TODO-02: A static member on a type you do not control: Sequence(start, count) returns
        //   count consecutive integers beginning at start (Enumerable.Range).
        public static IEnumerable<int> Sequence(int start, int count) => [];
    }
}

public static class MoneyExtensions
{
    extension(Money)
    {
        // TODO-02: Zero is a static extension property returning new Money(0).
        public static Money Zero => new(-1);

        // TODO-07 (optional): Add `public static Money operator *(Money left, int factor)` here
        //   so `new Money(10) * 3` is Money(30).
    }
}

public static class TextExtensions
{
    extension(string? text)
    {
        // TODO-03: IsBlank is true for null, empty and whitespace. The receiver may be null:
        //   an extension member is a static call underneath, so it does not throw by itself.
        public bool IsBlank => false;
    }
}

public static class RefExtensions
{
    extension(ref int value)
    {
        // TODO-04: Increment adds one to the caller's variable (the receiver is passed by ref).
        public void Increment() { }
    }
}

// TODO-06 (optional): Add `public static class MathExtensions` with `extension(Math)` and a
//   static `double Remap(double v, double fromLo, double fromHi, double toLo, double toHi)`
//   that maps v linearly between the two ranges, so `Math.Remap(5, 0, 10, 0, 100)` is 50.
