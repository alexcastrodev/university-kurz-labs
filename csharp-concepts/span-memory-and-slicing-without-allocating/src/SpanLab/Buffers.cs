using System.Runtime.CompilerServices;

namespace SpanLab;

[InlineArray(4)]
public struct Buffer4
{
    private int _element;
}

public static class Buffers
{
    public static Buffer4 Squares()
    {
        // TODO-06: Return a Buffer4 whose elements are 0, 1, 4, 9. An inline array converts
        //   to Span<int> implicitly, so fill it through a span.
        throw new NotImplementedException("Not implemented yet.");
    }
}
