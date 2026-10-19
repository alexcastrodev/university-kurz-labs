namespace Shop.Tests;

// TODO-02: These tests share one static Inventory, stocked once in [ClassInitialize]. Each test takes 6 of the
// 10 apples. Delete the Assert.Fail lines and run only this class a few times:
//   dotnet test --filter "FullyQualifiedName~StockIsolationTests"
// Which test fails? Is it always the same one? Then fix the class, not the tests: give every test its own
// Inventory in a readonly instance field set by the constructor, and remove [ClassInitialize].
[TestClass]
public sealed class StockIsolationTests
{
    private static readonly Inventory SharedInventory = new();

    [ClassInitialize]
    public static void StockTheShelves(TestContext context) => SharedInventory.Restock("apple", 10, 0.50m);

    [TestMethod]
    public void ACustomerCanBuySixApples()
    {
        Assert.Fail("TODO-02: delete this line, run the class, then fix the shared state.");

        new Cart(SharedInventory).Add("apple", 6);

        Assert.AreEqual(4, SharedInventory.Available("apple"));
    }

    [TestMethod]
    public void AnotherCustomerCanAlsoBuySixApples()
    {
        Assert.Fail("TODO-02: delete this line, run the class, then fix the shared state.");

        new Cart(SharedInventory).Add("apple", 6);

        Assert.AreEqual(4, SharedInventory.Available("apple"));
    }

    // Each data row is a separate test run: does each row get its own instance of the class, too?
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void EveryDataRowAlsoGetsAFreshInventory(int row)
    {
        Assert.Fail("TODO-02: delete this line, run the class, then fix the shared state.");

        Assert.AreEqual(10, SharedInventory.Available("apple"), $"row {row}");

        SharedInventory.Reserve("apple", 10);
    }
}
