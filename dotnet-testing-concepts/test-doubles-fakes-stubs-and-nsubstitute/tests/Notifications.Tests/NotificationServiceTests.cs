namespace Notifications.Tests;

// Read src/Notifications/NotificationService.cs first. The three senders and the directory are ports:
// the directory and the senders are NSubstitute substitutes, the log is your hand-written fake.
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

    // TODO-01: Make the directory return this contact when it is asked for Shipment.CustomerId.
    // Use the exact id, not Arg.Any<Guid>(): a stub that answers for any id would hide a service that
    // looks up the wrong one (try it later, in TODO-10).
    private void Known(Contact contact) =>
        throw new NotImplementedException("TODO-01: configure the directory substitute.");

    [TestMethod]
    public async Task AnEmailCustomerGetsTheShippingEmailAtTheirAddress()
    {
        // Assert the outcome first. Then check that exactly one email went to ana@example.com with the
        // order number in the subject (Arg.Is), and that the SMS sender received nothing.
        await Task.CompletedTask;
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public async Task AnSmsCustomerGetsAnSmsWhenTheGatewayAcceptsIt()
    {
        // Careful: ISmsSender.SendAsync returns Task<bool>, and a substitute you did not configure returns
        // false. Write the test with only _sms.Received(1) first and print the outcome: it passes, and
        // the customer got an email. Then configure the gateway to accept and assert the real outcome.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task AnSmsCustomerFallsBackToEmailWhenTheGatewayRefuses()
    {
        // Configure the refusal explicitly (Returns(false)), even though it is the default: the test
        // should say what it depends on.
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task ACustomerWhoOptedOutGetsNothing()
    {
        // DidNotReceive on its own passes for any reason that sends nothing, including a stub that was
        // set up for another customer id. Assert the outcome that names the reason.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task AnUnknownCustomerIsReportedAndNothingIsSent()
    {
        // Here the default (null) of an unconfigured directory is exactly what you want. Say so in a comment.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task AnSmsCustomerWithoutPhoneOrEmailCannotBeReached()
    {
        // Nothing can be delivered, so nothing may be recorded in the log either.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task TheSameShipmentNotifiesTheCustomerOnlyOnce()
    {
        // Notify twice with the same shipment. Check both outcomes and that one email was sent.
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }

    [TestMethod]
    public async Task AFailedEmailIsNotRecordedSoTheRetrySendsIt()
    {
        // Make the first call to the email sender fail and the second succeed: Returns accepts several
        // values, and Task.FromException(new EmailDeliveryException("...")) is a faulted task.
        // Then: the first notify throws, the log is empty, the retry sends the email and records it.
        await Task.CompletedTask;
        Assert.Fail("TODO-07: write this test.");
    }
}
