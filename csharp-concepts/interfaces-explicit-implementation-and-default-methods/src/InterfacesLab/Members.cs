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
    // TODO-02: Replace this single public method with two explicit implementations,
    //   IDrawable.Draw returning "draws a picture" and IWeapon.Draw returning "draws a gun".
    //   The class must not keep a public Draw.
    public string Draw() => throw new NotImplementedException("Not implemented yet.");
}

public interface ILogger
{
    void Log(string message);

    // TODO-04: Give Info a real default body that calls Log with "INFO: " followed by the
    //   message, so implementers only have to write Log.
    void Info(string message) => throw new NotImplementedException("Not implemented yet.");
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

    // TODO-05: Forward to the generic GetEnumerator. This explicit implementation keeps the
    //   non-generic method off the public surface of Playlist.
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException("Not implemented yet.");
}
