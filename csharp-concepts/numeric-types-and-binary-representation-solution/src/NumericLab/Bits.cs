using System.Numerics;

namespace NumericLab;

public static class Bits
{
    public static string ToBinary32(int value) => Convert.ToString(value, 2).PadLeft(32, '0');

    public static int Negate(int value) => ~value + 1;

    public static int ShiftRightArithmetic(int value, int count) => value >> count;

    public static int ShiftRightLogical(int value, int count) => value >>> count;

    public static int OneShiftedLeft(int count) => 1 << count;

    public static uint PackRgb(byte red, byte green, byte blue) =>
        ((uint)red << 16) | ((uint)green << 8) | blue;

    public static byte Channel(uint packed, int index) => index switch
    {
        0 => (byte)(packed >> 16),
        1 => (byte)(packed >> 8),
        2 => (byte)packed,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public static uint SetFlag(uint flags, int bit) => flags | (1u << bit);

    public static uint ClearFlag(uint flags, int bit) => flags & ~(1u << bit);

    public static bool HasFlag(uint flags, int bit) => (flags & (1u << bit)) != 0;

    public static int CountSetBits(uint value) => BitOperations.PopCount(value);

    public static uint NextPowerOfTwo(uint value) => value == 0 ? 1u : BitOperations.RoundUpToPowerOf2(value);
}
