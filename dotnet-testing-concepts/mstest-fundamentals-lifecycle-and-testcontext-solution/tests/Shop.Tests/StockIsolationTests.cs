namespace Shop.Tests;

// The starter shared one static Inventory, created in [ClassInitialize], between these tests. Each test
// takes 6 of the 10 apples, so whichever ran second found only 4 left and failed, and with
// method-level parallelism two tests could also write to the same Dictionary at the same time.
// A readonly instance field, set by the constructor, gives each test its own inventory.
[TestClass]
public sealed class StockIsolationTests
{
    private readonly Inventory _inventory = new();

    public StockIsolationTests() => _inventory.Restock("apple", 10, 0.50m);

    [TestMethod]
    public void ACustomerCanBuySixApples()
    {
        new Cart(_inventory).Add("apple", 6);

        Assert.AreEqual(4, _inventory.Available("apple"));
    }

    [TestMethod]
    public void AnotherCustomerCanAlsoBuySixApples()
    {
        new Cart(_inventory).Add("apple", 6);

        Assert.AreEqual(4, _inventory.Available("apple"));
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void EveryDataRowAlsoGetsAFreshInventory(int row)
    {
        Assert.AreEqual(10, _inventory.Available("apple"), $"row {row}");

        _inventory.Reserve("apple", 10);
    }
}
