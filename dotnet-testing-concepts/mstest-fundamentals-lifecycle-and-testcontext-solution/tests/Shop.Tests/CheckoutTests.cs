namespace Shop.Tests;

[TestClass]
[TestCategory("Checkout")]
public sealed class CheckoutTests
{
    private readonly TestContext _testContext;
    private readonly Inventory _inventory = new();
    private readonly Cart _cart;

    public CheckoutTests(TestContext testContext)
    {
        _testContext = testContext;
        _inventory.Restock("apple", 10, 0.50m);
        _cart = new Cart(_inventory);
    }

    [TestMethod]
    public async Task AnApprovedPaymentProducesAReceiptAndEmptiesTheCart()
    {
        var payments = new ApprovingGateway();
        _cart.Add("apple", 3);

        var receipt = await _cart.CheckoutAsync(payments, _testContext.CancellationToken);
        _testContext.WriteLine($"{_testContext.TestName}: {receipt}");

        Assert.AreEqual(1.50m, receipt.Total);
        Assert.AreEqual(3, receipt.Lines["apple"]);
        Assert.AreEqual(1.50m, Assert.ContainsSingle(payments.Charged));
        Assert.IsEmpty(_cart.Lines);
        Assert.AreEqual(7, _inventory.Available("apple"));
    }

    [TestMethod]
    public async Task ADeclinedPaymentGivesTheStockBack()
    {
        _cart.Add("apple", 3);

        var error = await Assert.ThrowsExactlyAsync<PaymentDeclinedException>(
            () => _cart.CheckoutAsync(new DecliningGateway(), _testContext.CancellationToken));

        Assert.AreEqual(1.50m, error.Amount);
        Assert.AreEqual(10, _inventory.Available("apple"));
        Assert.IsEmpty(_cart.Lines);
    }

    // Task.Delay throws TaskCanceledException, which derives from OperationCanceledException.
    // Here the contract is "it was cancelled", whatever the subtype, so Assert.ThrowsAsync is the
    // right choice; ThrowsExactlyAsync<OperationCanceledException> would fail.
    // The token is linked to TestContext.CancellationToken, so a test timeout or an aborted run
    // also stops the gateway. [Timeout] is the safety net if the code ignores the token.
    [TestMethod]
    [Timeout(5000, CooperativeCancellation = true)]
    public async Task ACancelledCheckoutGivesTheStockBack()
    {
        _cart.Add("apple", 3);
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_testContext.CancellationToken);
        cancellation.CancelAfter(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _cart.CheckoutAsync(new HangingGateway(), cancellation.Token));

        Assert.AreEqual(10, _inventory.Available("apple"));
        Assert.IsEmpty(_cart.Lines);
    }

    [TestMethod]
    public async Task AnEmptyCartCannotBeCheckedOut()
    {
        var payments = new ApprovingGateway();

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => _cart.CheckoutAsync(payments, _testContext.CancellationToken));

        Assert.IsEmpty(payments.Charged);
    }
}
