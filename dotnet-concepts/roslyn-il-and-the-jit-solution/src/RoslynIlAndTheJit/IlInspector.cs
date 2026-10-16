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
        using var pe = new PEReader(new MemoryStream(image));
        var reader = pe.GetMetadataReader();
        foreach (var typeHandle in reader.TypeDefinitions)
        {
            var type = reader.GetTypeDefinition(typeHandle);
            if (reader.GetString(type.Name) != typeName) continue;
            foreach (var methodHandle in type.GetMethods())
            {
                var method = reader.GetMethodDefinition(methodHandle);
                if (reader.GetString(method.Name) == methodName)
                    return pe.GetMethodBody(method.RelativeVirtualAddress).GetILBytes()!;
            }
        }
        throw new ArgumentException($"{typeName}.{methodName} not found.");
    }

    public static IReadOnlyList<OpCode> ReadOpCodes(byte[] il)
    {
        // TODO-02: Walk the IL: read one byte (0xFE means a two-byte opcode), look it up in `Table`,
        //   then skip the operand according to opcode.OperandType (0, 1, 2, 4 or 8 bytes;
        //   InlineSwitch is a 4-byte count followed by count * 4 bytes). Return the opcodes in order.
        var result = new List<OpCode>();
        var i = 0;
        while (i < il.Length)
        {
            ushort code = il[i++];
            if (code == 0xFE) code = (ushort)(0xFE00 | il[i++]);
            var op = Table[code];
            result.Add(op);
            i += op.OperandType switch
            {
                OperandType.InlineNone => 0,
                OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
                OperandType.InlineVar => 2,
                OperandType.InlineI8 or OperandType.InlineR => 8,
                OperandType.InlineSwitch => 4 + 4 * BitConverter.ToInt32(il, i),
                _ => 4,
            };
        }
        return result;
    }

    public static string? GetEntryPointName(byte[] image)
    {
        // TODO-06 (optional): Read the entry point from pe.PEHeaders.CorHeader.EntryPointTokenOrRelativeVirtualAddress
        //   (a MethodDef token), turn it into a handle and return the method name, or null for a library.
        using var pe = new PEReader(new MemoryStream(image));
        var token = pe.PEHeaders.CorHeader!.EntryPointTokenOrRelativeVirtualAddress;
        if (token == 0) return null;
        var reader = pe.GetMetadataReader();
        var handle = (MethodDefinitionHandle)MetadataTokens.Handle(token);
        return reader.GetString(reader.GetMethodDefinition(handle).Name);
    }
}
