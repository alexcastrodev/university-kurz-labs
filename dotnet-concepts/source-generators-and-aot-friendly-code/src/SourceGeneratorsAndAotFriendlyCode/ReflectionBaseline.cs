using System.Diagnostics.CodeAnalysis;

namespace SourceGeneratorsAndAotFriendlyCode;

public static class ReflectionBaseline
{
    // The runtime-reflection way to do what the generator does at compile time.
    // The attribute is how the trim and AOT analyzers (IL2026) learn that callers are not safe.
    [RequiresUnreferencedCode("Reads public properties by reflection, which the trimmer cannot see.")]
    public static string ToDisplayString(object value)
    {
        // TODO-05: Produce the same text the generated code produces, for example "Person { Name=Ada, Age=36 }":
        //   the type name, then " { ", then "Name=value" for each public instance property with a getter
        //   (use value.GetType().GetProperties()), joined by ", ", then " }".
        //   A class without properties gives "Empty { }".
        throw new NotImplementedException("Not implemented yet.");
    }
}
