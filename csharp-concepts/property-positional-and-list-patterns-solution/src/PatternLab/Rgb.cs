namespace PatternLab;

public sealed class Rgb(byte red, byte green, byte blue)
{
    public void Deconstruct(out byte r, out byte g, out byte b) => (r, g, b) = (red, green, blue);
}
