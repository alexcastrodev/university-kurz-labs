namespace PartialLab;

public partial class Loud
{
    partial void OnSaving(string detail) => Log.Add($"saving:{detail}");
}
