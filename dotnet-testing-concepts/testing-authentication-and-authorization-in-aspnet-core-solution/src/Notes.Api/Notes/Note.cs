namespace Notes.Api.Notes;

public sealed record Note(int Id, string OwnerId, string Text);

public sealed record CreateNoteRequest(string Text);

public sealed class NoteStore
{
    private readonly Lock _lock = new();
    private readonly List<Note> _notes = [];
    private int _lastId;

    public Note Add(string ownerId, string text)
    {
        lock (_lock)
        {
            var note = new Note(++_lastId, ownerId, text);
            _notes.Add(note);
            return note;
        }
    }

    public Note? Find(int id)
    {
        lock (_lock)
        {
            return _notes.FirstOrDefault(n => n.Id == id);
        }
    }

    public IReadOnlyList<Note> OwnedBy(string ownerId)
    {
        lock (_lock)
        {
            return [.. _notes.Where(n => n.OwnerId == ownerId)];
        }
    }

    public int Count()
    {
        lock (_lock)
        {
            return _notes.Count;
        }
    }

    public bool Remove(int id)
    {
        lock (_lock)
        {
            return _notes.RemoveAll(n => n.Id == id) > 0;
        }
    }
}
