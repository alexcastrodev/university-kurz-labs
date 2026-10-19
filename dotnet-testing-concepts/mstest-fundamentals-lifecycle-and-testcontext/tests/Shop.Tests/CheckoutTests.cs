namespace Shop.Tests;

// TODO-05: Receive the TestContext through the constructor and keep it in a readonly field. Pass its
// CancellationToken to CheckoutAsync (never CancellationToken.None): it is cancelled when the test times out
// or the run is aborted. Read Cart.CheckoutAsync in src/Shop/Cart.cs first.
[TestClass]
public sealed class CheckoutTests
{
    private readonly Inventory _inventory = new();
    private readonly Cart _cart;

    public CheckoutTests()
    {
        _inventory.Restock("apple", 10, 0.50m);
        _cart = new Cart(_inventory);
    }

    [TestMethod]
    public async Task AnApprovedPaymentProducesAReceiptAndEmptiesTheCart()
    {
        // Check the receipt, the single amount charged, the empty cart and the stock left.
        // Write the receipt to the test output with TestContext.WriteLine.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task ADeclinedPaymentGivesTheStockBack()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task ACancelledCheckoutGivesTheStockBack()
    {
        // Use a CancellationTokenSource linked to TestContext.CancellationToken, cancelled after 50 ms, and the
        // HangingGateway. Add [Timeout(5000, CooperativeCancellation = true)] as a safety net.
        // Try Assert.ThrowsExactlyAsync<OperationCanceledException> first, read the failure, then choose.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task AnEmptyCartCannotBeCheckedOut()
    {
        // Nothing may be charged either.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }
}
