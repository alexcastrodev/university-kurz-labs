namespace FileBasedAppsLab;

public static class Apps
{
    public static string HelloApp() =>
        """
        Console.WriteLine("hello from a file-based app");
        """;

    public static string AppWithArgs() =>
        """
        Console.WriteLine(string.Join(",", args));
        return args.Length;
        """;

    public static string AppWithProperty() =>
        """
        #:property PublishAot=false
        Console.WriteLine(System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported);
        """;

    public static string ShebangApp() =>
        """
        #!/usr/bin/env dotnet
        Console.WriteLine("shebang ok");
        """;

    public static string AppWithCompileError() =>
        """
        Console.WriteLine(ThisNameDoesNotExist);
        """;
}
