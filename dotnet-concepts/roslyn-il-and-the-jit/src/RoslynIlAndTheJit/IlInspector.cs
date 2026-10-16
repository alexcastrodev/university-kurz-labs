using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace RoslynIlAndTheJit;

public static class IlInspector
{
    private static readonly Dictionary<ushort, OpCode> Table = typeof(OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(field => (OpCode)field.GetValue(null)!)
        .ToDictionary(op => (ushort)op.Value);

    public static byte[] GetMethodIl(byte[] image, string typeName, string methodName)
    {
        // TODO-03: Without loading the assembly, open it with PEReader and MetadataReader, find the type
        //   definition and the method definition by name, and return the IL bytes of its body
        //   (GetMethodBody(method.RelativeVirtualAddress).GetILBytes()).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static IReadOnlyList<OpCode> ReadOpCodes(byte[] il)
    {
        // TODO-02: Walk the IL: read one byte (0xFE means a two-byte opcode), look it up in `Table`,
        //   then skip the operand according to opcode.OperandType (0, 1, 2, 4 or 8 bytes;
        //   InlineSwitch is a 4-byte count followed by count * 4 bytes). Return the opcodes in order.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string? GetEntryPointName(byte[] image)
    {
        // TODO-06 (optional): Read the entry point from pe.PEHeaders.CorHeader.EntryPointTokenOrRelativeVirtualAddress
        //   (a MethodDef token), turn it into a handle and return the method name, or null for a library.
        throw new NotImplementedException("Not implemented yet.");
    }
}
