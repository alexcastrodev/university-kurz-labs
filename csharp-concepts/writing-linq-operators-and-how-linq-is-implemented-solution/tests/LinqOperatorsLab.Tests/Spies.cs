using System.Collections;

namespace LinqOperatorsLab.Tests;

/// <summary>An iterator source that records how many items were pulled and how it was disposed.</summary>
public class TrackedSource(int count) : IEnumerable<int>
{
    public int Pulled { get; private set; }
    public int Enumerations { get; private set; }
    public int Disposed { get; private set; }

    public IEnumerator<int> GetEnumerator()
    {
        Enumerations++;
        try
        {
            for (var i = 1; i <= count; i++)
            {
                Pulled++;
                yield return i;
            }
        }
        finally
        {
            Disposed++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>An IList that counts how many enumerators were requested.</summary>
public class ListSpy(params int[] items) : IList<int>
{
    private readonly List<int> _inner = [.. items];

    public int EnumeratorsCreated { get; private set; }

    public int Count => _inner.Count;
    public bool IsReadOnly => true;
    public int this[int index]
    {
        get => _inner[index];
        set => throw new NotSupportedException();
    }

    public IEnumerator<int> GetEnumerator()
    {
        EnumeratorsCreated++;
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public bool Contains(int item) => _inner.Contains(item);
    public void CopyTo(int[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);
    public int IndexOf(int item) => _inner.IndexOf(item);
    public void Add(int item) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();
    public void Insert(int index, int item) => throw new NotSupportedException();
    public bool Remove(int item) => throw new NotSupportedException();
    public void RemoveAt(int index) => throw new NotSupportedException();
}
