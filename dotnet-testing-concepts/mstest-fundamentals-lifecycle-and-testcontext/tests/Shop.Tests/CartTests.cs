namespace Shop.Tests;

// TODO-03: Write the cart tests. The fixture stocks 10 apples at 0.50 and 5 pears at 0.80 for every test.
[TestClass]
public sealed class CartTests
{
    private readonly Inventory _inventory = new();
    private readonly Cart _cart;

    public CartTests()
    {
        _inventory.Restock("apple", 10, 0.50m);
        _inventory.Restock("pear", 5, 0.80m);
        _cart = new Cart(_inventory);
    }

    [TestMethod]
    public void AddingTheSameProductTwiceAddsUpTheQuantity()
    {
        // Check the cart line and the stock left.
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void TheTotalIsTheSumOfEveryLine()
    {
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void RemovingALineGivesTheStockBack()
    {
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void AddingZeroUnitsIsRejected()
    {
        // A colleague wrote the line below, and it passes. Delete the quantity check
        // (ArgumentOutOfRangeException.ThrowIfNegativeOrZero) in Inventory.Reserve and run it again: still green.
        // Find out why, restore the check, and rewrite the test so that it fails for that change.
        Assert.Throws<ArgumentException>(() => _cart.Add("Apple", 0));

        Assert.Fail("TODO-03: this test passes for the wrong reason. Fix it.");
    }
}
