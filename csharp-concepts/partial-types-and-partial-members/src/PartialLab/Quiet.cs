namespace PartialLab;

public partial class Quiet
{
    public int ExpensiveCalls { get; private set; }

    // Nobody implements this hook, so the compiler removes the call and its arguments.
    partial void OnSaving(string detail);

    public string Save()
    {
        // TODO-01: Call OnSaving(Expensive()) and return "saved". Because the hook has no
        //   implementation, the compiler drops the call, so Expensive() must never run.
        throw new NotImplementedException("Not implemented yet.");
    }

    private string Expensive()
    {
        ExpensiveCalls++;
        return "detail";
    }
}
