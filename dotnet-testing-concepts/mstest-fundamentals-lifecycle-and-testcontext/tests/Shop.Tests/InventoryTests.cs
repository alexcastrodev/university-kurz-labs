namespace Shop.Tests;

// TODO-01: Write the inventory tests. MSTest creates a new instance of this class for every test, so the
// readonly field below is a fresh inventory with 10 apples in each one. Read src/Shop/Inventory.cs first.
[TestClass]
public sealed class InventoryTests
{
    private readonly Inventory _inventory = new();

    public InventoryTests() => _inventory.Restock("apple", 10, 0.50m);

    [TestMethod]
    public void ReservingTakesUnitsOutOfStock()
    {
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void ReservingMoreThanIsAvailableSaysHowManyAreLeft()
    {
        // Assert.ThrowsExactly<T> returns the exception: check Sku, Requested and Available on it.
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void AFailedReservationLeavesTheStockUnchanged()
    {
        // An operation that fails must not leave the object half changed. What would prove it?
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void ReservingAnUnknownProductIsRejected()
    {
        // SKUs are case-sensitive. UnknownProductException derives from ArgumentException.
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void ReservingANonPositiveQuantityIsRejected()
    {
        // Turn this into one test that runs for 0 and for -1: give it an int parameter and add [DataRow] rows.
        // Which exception does ArgumentOutOfRangeException.ThrowIfNegativeOrZero throw, exactly?
        Assert.Fail("TODO-01: write this test.");
    }
}
