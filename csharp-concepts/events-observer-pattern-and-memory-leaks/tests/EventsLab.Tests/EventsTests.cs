namespace EventsLab.Tests;

public class EventsTests
{
    private static void FullCollect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    // TODO-00
    [Fact]
    public void ShouldDepositWithoutSubscribers()
    {
        var account = new Account();

        account.Deposit(10);

        Assert.Equal(10, account.Balance);
    }

    [Fact]
    public void ShouldRaiseWithOldAndNewBalanceAndSender()
    {
        var account = new Account();
        account.Deposit(10);
        object? seenSender = null;
        ValueChangedEventArgs? seen = null;
        account.BalanceChanged += (s, e) => { seenSender = s; seen = e; };

        account.Deposit(5);

        Assert.Same(account, seenSender);
        Assert.Equal(10, seen!.OldValue);
        Assert.Equal(15, seen.NewValue);
    }

    [Fact]
    public void ShouldCallHandlersInSubscriptionOrder()
    {
        var account = new Account();
        var order = new List<int>();
        account.BalanceChanged += (_, _) => order.Add(1);
        account.BalanceChanged += (_, _) => order.Add(2);

        account.Deposit(1);

        Assert.Equal([1, 2], order);
    }

    [Fact]
    public void ShouldRejectNonPositiveDeposit()
    {
        var account = new Account();
        var raised = false;
        account.BalanceChanged += (_, _) => raised = true;

        Assert.Throws<ArgumentOutOfRangeException>(() => account.Deposit(0));

        Assert.False(raised);
        Assert.Equal(0, account.Balance);
    }

    // TODO-01
    [Fact]
    public void ShouldSubscribeInConstructorAndUnsubscribeOnDispose()
    {
        var source = new TickSource();

        var widget = new Widget(source);
        Assert.Equal(1, source.HandlerCount);
        source.Raise();
        Assert.Equal(1, widget.Ticks);

        widget.Dispose();
        Assert.Equal(0, source.HandlerCount);
        source.Raise();
        Assert.Equal(1, widget.Ticks);
    }

    // TODO-02
    [Fact]
    public void ShouldUnsubscribeWhenSubscriptionIsDisposed()
    {
        var feed = new PriceFeed();
        var prices = new List<int>();

        using (feed.Subscribe((_, p) => prices.Add(p)))
        {
            Assert.Equal(1, feed.HandlerCount);
            feed.Publish(10);
        }

        feed.Publish(20);
        Assert.Equal(0, feed.HandlerCount);
        Assert.Equal([10], prices);
    }

    [Fact]
    public void ShouldAllowDisposingTheSubscriptionTwice()
    {
        var feed = new PriceFeed();
        var other = feed.Subscribe((_, _) => { });
        var subscription = feed.Subscribe((_, _) => { });

        subscription.Dispose();
        subscription.Dispose();

        Assert.Equal(1, feed.HandlerCount);
        other.Dispose();
        Assert.Equal(0, feed.HandlerCount);
    }

    // TODO-03
    [Fact]
    public void ShouldLeakWhenTheWidgetIsNeverDisposed()
    {
        var source = new TickSource();

        var reference = WidgetFactory.Create(source, dispose: false);
        FullCollect();

        Assert.True(reference.IsAlive);
        Assert.Equal(1, source.HandlerCount);
        GC.KeepAlive(source);
    }

    [Fact]
    public void ShouldAllowCollectionAfterDispose()
    {
        var source = new TickSource();

        var reference = WidgetFactory.Create(source, dispose: true);
        FullCollect();

        Assert.False(reference.IsAlive);
        Assert.Equal(0, source.HandlerCount);
        GC.KeepAlive(source);
    }

    // TODO-04
    [Fact]
    public void ShouldCountTicksWhileAttached()
    {
        var source = new TickSource();
        var listener = new LambdaListener();

        listener.Attach(source);
        source.Raise();
        source.Raise();

        Assert.Equal(2, listener.Count);
        Assert.Equal(1, source.HandlerCount);
    }

    [Fact]
    public void ShouldStopCountingAfterDetach()
    {
        var source = new TickSource();
        var listener = new LambdaListener();
        listener.Attach(source);
        source.Raise();

        listener.Detach(source);
        source.Raise();

        Assert.Equal(1, listener.Count);
        Assert.Equal(0, source.HandlerCount);
    }

    // TODO-05 (optional)
    [Fact]
    public void ShouldStoreAndRemoveClickHandlersWithCustomAccessors()
    {
        var button = new Button();
        var clicks = 0;
        EventHandler handler = (_, _) => clicks++;

        button.Click += handler;
        button.Click += handler;
        button.Click += null;
        Assert.Equal(2, button.HandlerCount);
        button.Press();
        Assert.Equal(2, clicks);

        button.Click -= handler;
        button.Click -= null;
        Assert.Equal(1, button.HandlerCount);
    }

    // TODO-06 (optional)
    [Fact]
    public void ShouldSubscribeOnlyWhenAccountIsNotNull()
    {
        var account = new Account();
        var calls = 0;
        EventHandler<ValueChangedEventArgs> handler = (_, _) => calls++;

        Subscriber.Attach(null, handler);
        Subscriber.Attach(account, handler);
        account.Deposit(1);

        Assert.Equal(1, calls);
    }
}
