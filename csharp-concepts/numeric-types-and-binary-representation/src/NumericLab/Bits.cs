namespace NumericLab;

public static class Bits
{
    public static string ToBinary32(int value)
    {
        // TODO-00: Return the 32-character two's complement text of the value, with leading zeros
        //   (5 is "000...0101", -5 is "111...1011"). Convert.ToString(value, 2) is a starting point.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int Negate(int value)
    {
        // TODO-00: Negate without using the unary minus or multiplication: invert all bits and add
        //   one. int.MinValue has no positive twin, so it must come back unchanged (no exception).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int ShiftRightArithmetic(int value, int count)
    {
        // TODO-01: Shift right copying the sign bit (-16 >> 2 is -4).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int ShiftRightLogical(int value, int count)
    {
        // TODO-01: Shift right filling with zeros, even for a negative int (-16 -> 1073741820 for
        //   count 2). C# 11 has an operator for it, so no cast to uint is needed.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int OneShiftedLeft(int count)
    {
        // TODO-01: Return 1 << count using the plain operator. The count is masked to 5 bits for
        //   int, so a count of 32 wraps to 1 and 33 to 2. Do not "fix" that, the tests expect it.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static uint PackRgb(byte red, byte green, byte blue)
    {
        // TODO-02: Pack the channels into one uint laid out as 0x00RRGGBB (red in bits 16..23).
        //   Remember that byte arithmetic happens in int, so cast before shifting if needed.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static byte Channel(uint packed, int index)
    {
        // TODO-02: Read a channel back: index 0 is red, 1 is green, 2 is blue. Use a shift and a
        //   mask. Throw ArgumentOutOfRangeException for any other index.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static uint SetFlag(uint flags, int bit)
    {
        // TODO-02: Return flags with the given bit set.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static uint ClearFlag(uint flags, int bit)
    {
        // TODO-02: Return flags with the given bit cleared, leaving the others alone.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool HasFlag(uint flags, int bit)
    {
        // TODO-02: True when the given bit is set.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int CountSetBits(uint value)
    {
        // TODO-03: Count the 1 bits with System.Numerics.BitOperations, not a loop.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static uint NextPowerOfTwo(uint value)
    {
        // TODO-03: Smallest power of two that is >= value (1000 -> 1024, 64 -> 64). Use
        //   BitOperations. Return 1 for 0.
        throw new NotImplementedException("Not implemented yet.");
    }
}
