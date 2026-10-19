namespace Shop.Tests;

// TODO-00: Prove the order in which MSTest runs its lifecycle hooks.
// 1. Add every hook below to this class. Each one appends its own name (as spelled in ExpectedOrder) to Log:
//    [AssemblyInitialize], [ClassInitialize], a constructor that receives the TestContext, [TestInitialize],
//    [TestCleanup], Dispose (implement IDisposable), [ClassCleanup] and [AssemblyCleanup].
//    Check the required signatures: which hooks must be public static, and which take a TestContext?
// 2. In the test, append "Test" and assert that Log equals the first five entries of ExpectedOrder
//    (Assert.AreSequenceEqual, and C# ranges: ExpectedOrder[..5]).
// 3. The test cannot see what runs after it. Assert the whole ExpectedOrder in [AssemblyCleanup], the last hook.
//    Then swap two entries of ExpectedOrder on purpose and check that `dotnet test` turns red. Read which test
//    the failure is reported on.
// 4. Run `dotnet test --filter "FullyQualifiedName~CartTests"`. Does your [AssemblyCleanup] still pass when
//    this class did not run at all?
// Only this class writes to Log, so tests of other classes running in parallel cannot interleave entries.
[TestClass]
public sealed class LifecycleOrderTests
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

    [TestMethod]
    public void TheTestRunsAfterEverySetupStep()
    {
        // Also write Log to the test output with the TestContext you received in the constructor:
        // WriteLine and TestName. Run `dotnet test --logger "console;verbosity=detailed"` to see it.
        Assert.Fail("TODO-00: write this test.");
    }
}
