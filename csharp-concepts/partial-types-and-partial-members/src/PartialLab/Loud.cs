namespace PartialLab;

// Legacy partial method: the hook is declared here and implemented in Loud.Hooks.cs.
public partial class Loud
{
    public List<string> Log { get; } = new();

    partial void OnSaving(string detail);

    public string Save(string detail)
    {
        OnSaving(detail);
        return "saved";
    }
}
