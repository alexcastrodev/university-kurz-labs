namespace Shop.Tests;

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
        _cart.Add("apple", 2);
        _cart.Add("apple", 3);

        Assert.AreEqual(5, _cart.Lines["apple"]);
        Assert.AreEqual(5, _inventory.Available("apple"));
    }

    [TestMethod]
    public void TheTotalIsTheSumOfEveryLine()
    {
        _cart.Add("apple", 4);
        _cart.Add("pear", 2);

        Assert.AreEqual(3.60m, _cart.Total);
    }

    [TestMethod]
    public void RemovingALineGivesTheStockBack()
    {
        _cart.Add("pear", 5);

        _cart.Remove("pear");

        Assert.IsEmpty(_cart.Lines);
        Assert.AreEqual(5, _inventory.Available("pear"));
    }

    // The starter's version was Assert.Throws<ArgumentException>(() => _cart.Add("Apple", 0)).
    // "Apple" is not "apple", so it threw UnknownProductException, which derives from ArgumentException,
    // and the test stayed green even with the quantity check deleted. ThrowsExactly names the real type,
    // and the SKU is one the inventory knows, so only the quantity rule can make it pass.
    [TestMethod]
    public void AddingZeroUnitsIsRejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _cart.Add("apple", 0));

        Assert.IsEmpty(_cart.Lines);
    }
}
