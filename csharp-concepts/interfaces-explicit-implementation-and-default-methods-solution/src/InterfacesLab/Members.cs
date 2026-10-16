using System.Collections;

namespace InterfacesLab;

public interface IDrawable
{
    string Draw();
}

public interface IWeapon
{
    string Draw();
}

public class Cowboy : IDrawable, IWeapon
{
    string IDrawable.Draw() => "draws a picture";

    string IWeapon.Draw() => "draws a gun";
}

public interface ILogger
{
    void Log(string message);

    void Info(string message) => Log($"INFO: {message}");
}

public class ListLogger : ILogger
{
    public List<string> Lines { get; } = [];

    public void Log(string message) => Lines.Add(message);
}

public class Playlist(IEnumerable<string> songs) : IEnumerable<string>
{
    private readonly List<string> _songs = [.. songs];

    public IEnumerator<string> GetEnumerator() => _songs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
