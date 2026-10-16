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
        var buffer = new Buffer4();
        Span<int> span = buffer;
        for (var i = 0; i < span.Length; i++) span[i] = i * i;
        return buffer;
    }
}
