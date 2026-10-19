namespace Notifications.Tests;

[TestClass]
public sealed class NotificationServiceTests(TestContext testContext)
{
    private static readonly Shipment Shipment = new(OrderId: Guid.NewGuid(), CustomerId: Guid.NewGuid(), OrderNumber: "A-1001");

    private readonly ICustomerDirectory _directory = Substitute.For<ICustomerDirectory>();
    private readonly IEmailSender _email = Substitute.For<IEmailSender>();
    private readonly ISmsSender _sms = Substitute.For<ISmsSender>();
    private readonly InMemoryNotificationLog _log = new();

    private Task<NotificationOutcome> NotifyAsync() =>
        new NotificationService(_directory, _email, _sms, _log).NotifyShippedAsync(Shipment, testContext.CancellationToken);

    private static Contact Ana(
        Channel preferred = Channel.Email,
        string? email = "ana@example.com",
        string? phone = "+15550100",
        bool optedOut = false) =>
        new(Shipment.CustomerId, email, phone, preferred, optedOut);

    // The stub answers for this customer id only. With Arg.Any<Guid>() it would also answer when the
    // service looks up the wrong id (the order id, say), and every test below would still pass.
    private void Known(Contact contact) =>
        _directory.FindAsync(Shipment.CustomerId, Arg.Any<CancellationToken>()).Returns(contact);

    [TestMethod]
    public async Task AnEmailCustomerGetsTheShippingEmailAtTheirAddress()
    {
        Known(Ana());

        var outcome = await NotifyAsync();

        Assert.AreEqual(NotificationOutcome.SentByEmail, outcome);
        await _email.Received(1).SendAsync(
            "ana@example.com", Arg.Is<string>(subject => subject.Contains("A-1001")), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _sms.DidNotReceiveWithAnyArgs().SendAsync(default!, default!, default);
    }

    [TestMethod]
    public async Task AnSmsCustomerGetsAnSmsWhenTheGatewayAcceptsIt()
    {
        Known(Ana(Channel.Sms));
        // Without this line the substitute returns false (the default of Task<bool>), the service falls
        // back to email, and a test that only checks Received() on the SMS sender still passes.
        _sms.SendAsync("+15550100", Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        var outcome = await NotifyAsync();

        Assert.AreEqual(NotificationOutcome.SentBySms, outcome);
        await _email.DidNotReceiveWithAnyArgs().SendAsync(default!, default!, default!, default);
    }

    [TestMethod]
    public async Task AnSmsCustomerFallsBackToEmailWhenTheGatewayRefuses()
    {
        Known(Ana(Channel.Sms));
        _sms.SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var outcome = await NotifyAsync();

        Assert.AreEqual(NotificationOutcome.FellBackToEmail, outcome);
        await _email.Received(1).SendAsync("ana@example.com", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ACustomerWhoOptedOutGetsNothing()
    {
        Known(Ana(optedOut: true));

        var outcome = await NotifyAsync();

        // The outcome names the reason. DidNotReceive alone would also pass for an unknown customer.
        Assert.AreEqual(NotificationOutcome.OptedOut, outcome);
        await _email.DidNotReceiveWithAnyArgs().SendAsync(default!, default!, default!, default);
        await _sms.DidNotReceiveWithAnyArgs().SendAsync(default!, default!, default);
        Assert.IsEmpty(_log.Keys);
    }

    [TestMethod]
    public async Task AnUnknownCustomerIsReportedAndNothingIsSent()
    {
        // No Known(...): the unconfigured directory returns null, which is exactly "not found".
        var outcome = await NotifyAsync();

        Assert.AreEqual(NotificationOutcome.UnknownCustomer, outcome);
        await _email.DidNotReceiveWithAnyArgs().SendAsync(default!, default!, default!, default);
    }

    [TestMethod]
    public async Task AnSmsCustomerWithoutPhoneOrEmailCannotBeReached()
    {
        Known(Ana(Channel.Sms, email: null, phone: null));

        var outcome = await NotifyAsync();

        Assert.AreEqual(NotificationOutcome.NoReachableChannel, outcome);
        Assert.IsEmpty(_log.Keys);
    }

    [TestMethod]
    public async Task TheSameShipmentNotifiesTheCustomerOnlyOnce()
    {
        Known(Ana());

        var first = await NotifyAsync();
        var second = await NotifyAsync();

        Assert.AreEqual(NotificationOutcome.SentByEmail, first);
        Assert.AreEqual(NotificationOutcome.AlreadySent, second);
        await _email.Received(1).SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task AFailedEmailIsNotRecordedSoTheRetrySendsIt()
    {
        Known(Ana());
        // First call: a faulted task, as an SMTP server that is down. Second call: success.
        _email.SendAsync("ana@example.com", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new EmailDeliveryException("smtp down")), Task.CompletedTask);

        await Assert.ThrowsExactlyAsync<EmailDeliveryException>(NotifyAsync);
        Assert.IsEmpty(_log.Keys);

        var retry = await NotifyAsync();

        Assert.AreEqual(NotificationOutcome.SentByEmail, retry);
        Assert.HasCount(1, _log.Keys);
    }
}
