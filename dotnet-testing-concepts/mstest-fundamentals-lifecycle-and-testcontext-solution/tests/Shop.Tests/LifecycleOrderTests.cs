namespace Shop.Tests;

// Only this class writes to the log, so other classes running in parallel cannot interleave entries.
// The class has exactly one test, so the expected log is a single, fixed sequence.
[TestClass]
public sealed class LifecycleOrderTests : IDisposable
{
    private static readonly List<string> Log = [];

    private static readonly string[] ExpectedOrder =
    [
        "AssemblyInitialize",
        "ClassInitialize",
        "Constructor",
        "TestInitialize",
        "Test",
        "TestCleanup",
        "Dispose",
        "ClassCleanup",
        "AssemblyCleanup",
    ];

    private readonly TestContext _testContext;

    public LifecycleOrderTests(TestContext testContext)
    {
        _testContext = testContext;
        Log.Add("Constructor");
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context) => Log.Add("AssemblyInitialize");

    [ClassInitialize]
    public static void ClassInitialize(TestContext context) => Log.Add("ClassInitialize");

    [TestInitialize]
    public void TestInitialize() => Log.Add("TestInitialize");

    [TestMethod]
    public void TheTestRunsAfterEverySetupStep()
    {
        Log.Add("Test");
        _testContext.WriteLine($"{_testContext.TestName}: {string.Join(" > ", Log)}");

        Assert.AreSequenceEqual(ExpectedOrder[..5], Log);
    }

    [TestCleanup]
    public void TestCleanup() => Log.Add("TestCleanup");

    public void Dispose() => Log.Add("Dispose");

    [ClassCleanup]
    public static void ClassCleanup() => Log.Add("ClassCleanup");

    // The last hook to run is the only place that can see the whole sequence. A failure here is
    // reported as a failed result, so the run still turns red.
    // Assembly hooks belong to the assembly, not to this class: they also run when a filter such as
    // --filter "TestCategory=Checkout" leaves this class out. Then the class never ran, and there is
    // nothing to check.
    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        Log.Add("AssemblyCleanup");
        if (!Log.Contains("Test"))
        {
            return;
        }

        Assert.AreSequenceEqual(ExpectedOrder, Log);
    }
}
