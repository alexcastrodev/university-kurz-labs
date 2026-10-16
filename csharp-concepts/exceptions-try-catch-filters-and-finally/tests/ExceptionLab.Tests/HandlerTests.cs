using System.Runtime.ExceptionServices;

namespace ExceptionLab.Tests;

public class HandlerTests
{
    private static void Boom() => throw new InvalidOperationException("boom");

    [Fact]
    public void ShouldReturnNoneWhenNothingIsThrown() =>
        Assert.Equal(Failure.None, Handlers.Classify(() => { }));

    [Fact]
    public void ShouldClassifyAMissingFileBeforeAGenericIoError() =>
        Assert.Equal(Failure.NotFound, Handlers.Classify(() => throw new FileNotFoundException()));

    [Fact]
    public void ShouldClassifyOtherIoErrors() =>
        Assert.Equal(Failure.Io, Handlers.Classify(() => throw new IOException()));

    [Theory]
    [InlineData(500, Failure.Transient)]
    [InlineData(503, Failure.Transient)]
    [InlineData(400, Failure.Permanent)]
    [InlineData(404, Failure.Permanent)]
    public void ShouldUseAFilterOnTheStatusCode(int status, Failure expected) =>
        Assert.Equal(expected, Handlers.Classify(() => throw new RemoteException(status)));

    [Fact]
    public void ShouldClassifyUnknownExceptionsAsUnexpected() =>
        Assert.Equal(Failure.Unexpected, Handlers.Classify(() => throw new InvalidOperationException()));

    [Fact]
    public void ShouldCleanUpAfterSuccess()
    {
        var log = new List<string>();

        Handlers.RunWithCleanup(() => log.Add("work"), log);

        Assert.Equal(["start", "work", "cleanup"], log);
    }

    [Fact]
    public void ShouldCleanUpAndStillPropagateTheException()
    {
        var log = new List<string>();

        Assert.Throws<InvalidOperationException>(() => Handlers.RunWithCleanup(Boom, log));

        Assert.Equal(["start", "cleanup"], log);
    }

    [Fact]
    public void ShouldRunTheFilterBeforeTheInnerFinally()
    {
        var log = new List<string>();

        Handlers.ObserveTwoPasses(Boom, log);

        Assert.Equal(["filter", "finally", "catch"], log);
    }

    [Fact]
    public void ShouldNotRunFilterOrCatchWhenNothingIsThrown()
    {
        var log = new List<string>();

        Handlers.ObserveTwoPasses(() => { }, log);

        Assert.Equal(["finally"], log);
    }

    [Fact]
    public void ShouldReturnNullWhenNothingIsCaptured() =>
        Assert.Null(Handlers.TryCapture(() => { }));

    [Fact]
    public void ShouldKeepTheOriginalStackTraceWhenRethrowing()
    {
        ExceptionDispatchInfo? info = Handlers.TryCapture(Boom);

        Assert.NotNull(info);
        var rethrown = Assert.Throws<InvalidOperationException>(() => info.Throw());
        Assert.Contains(nameof(Boom), rethrown.StackTrace);
    }

    [Fact]
    public void ShouldCollectEveryMessageFromAFaultedWhenAll()
    {
        var all = Task.WhenAll(
            Task.FromException(new InvalidOperationException("first")),
            Task.FromException(new TimeoutException("second")));

        Assert.Equal(["first", "second"], Handlers.CollectMessages(all));
    }

    [Fact]
    public void ShouldReturnNoMessagesWhenTheTaskSucceeds() =>
        Assert.Empty(Handlers.CollectMessages(Task.CompletedTask));

    [Fact]
    public void ShouldCarryTheOrderIdAndTheInnerException()
    {
        var inner = new TimeoutException("db");

        var ex = new OrderRejectedException("rejected", "A-42", inner);

        Assert.Equal("A-42", ex.OrderId);
        Assert.Same(inner, ex.InnerException);
        Assert.Equal("rejected", ex.Message);
    }
}
