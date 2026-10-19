namespace Shop.Tests;

[TestClass]
public sealed class InventoryTests
{
    private readonly Inventory _inventory = new();

    public InventoryTests() => _inventory.Restock("apple", 10, 0.50m);

    [TestMethod]
    public void ReservingTakesUnitsOutOfStock()
    {
        _inventory.Reserve("apple", 4);

        Assert.AreEqual(6, _inventory.Available("apple"));
    }

    [TestMethod]
    public void ReservingMoreThanIsAvailableSaysHowManyAreLeft()
    {
        var error = Assert.ThrowsExactly<OutOfStockException>(() => _inventory.Reserve("apple", 11));

        Assert.AreEqual("apple", error.Sku);
        Assert.AreEqual(11, error.Requested);
        Assert.AreEqual(10, error.Available);
    }

    [TestMethod]
    public void AFailedReservationLeavesTheStockUnchanged()
    {
        Assert.ThrowsExactly<OutOfStockException>(() => _inventory.Reserve("apple", 11));

        Assert.AreEqual(10, _inventory.Available("apple"));
    }

    [TestMethod]
    public void ReservingAnUnknownProductIsRejected()
    {
        var error = Assert.ThrowsExactly<UnknownProductException>(() => _inventory.Reserve("Apple", 1));

        Assert.AreEqual("Apple", error.Sku);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void ReservingANonPositiveQuantityIsRejected(int quantity)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _inventory.Reserve("apple", quantity));

        Assert.AreEqual(10, _inventory.Available("apple"));
    }
}
