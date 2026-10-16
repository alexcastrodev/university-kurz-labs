using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace GuardLab.Tests;

public class BonusTests
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Boom() => throw new InvalidOperationException("boom");

    [Fact]
    public void ShouldRethrowOutsideTheCatchBlockWithTheOriginalStackTrace()
    {
        var pending = new List<ExceptionDispatchInfo>();

        Guards.RethrowLater(Boom, pending);

        var info = Assert.Single(pending);
        var ex = Assert.Throws<InvalidOperationException>(info.Throw);
        Assert.Contains(nameof(Boom), ex.StackTrace);
    }

    [Fact]
    public void ShouldGuardADisposedResource()
    {
        var resource = new Resource();
        Assert.Equal("data", resource.Read());

        resource.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(resource.Read);
        Assert.Equal(typeof(Resource).FullName, ex.ObjectName);
    }
}
