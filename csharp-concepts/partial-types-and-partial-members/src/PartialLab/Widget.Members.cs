namespace PartialLab;

public partial class Widget
{
    private string _title = "";

    public partial string Title
    {
        // TODO-02: Implement the property: the getter returns the stored title and the
        //   setter stores the value trimmed. An implementing declaration cannot be an
        //   auto-property (CS9250).
        get => throw new NotImplementedException("Not implemented yet.");
        set => throw new NotImplementedException("Not implemented yet.");
    }

    // TODO-02: Implement the indexer: return i multiplied by the widget size.
    public partial int this[int i] => throw new NotImplementedException("Not implemented yet.");

    // TODO-02: Implement the extended partial method: return "<Title>:<this[1]>".
    public partial string Describe() => throw new NotImplementedException("Not implemented yet.");
}
