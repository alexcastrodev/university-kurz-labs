namespace PartialLab;

public partial class Widget
{
    private string _title = "";

    public partial string Title
    {
        get => _title;
        set => _title = value.Trim();
    }

    public partial int this[int i] => i * _size;

    public partial string Describe() => $"{Title}:{this[1]}";
}
