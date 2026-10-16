namespace PartialLab;

public partial class Quiet
{
    public int ExpensiveCalls { get; private set; }

    // Nobody implements this hook, so the compiler removes the call and its arguments.
    partial void OnSaving(string detail);

    public string Save()
    {
        OnSaving(Expensive());
        return "saved";
    }

    private string Expensive()
    {
        ExpensiveCalls++;
        return "detail";
    }
}
