using System.Reflection;

namespace Checkout.Tests;

// These two tests check the setup of the suite, not the production code.
[TestClass]
[TestCategory("Runner")]
public sealed class RunnerTests
{
    // Under VSTest the tests are loaded by a host process called testhost. Under Microsoft.Testing.Platform
    // the test project is itself the executable, so the entry assembly is Checkout.Tests.
    [TestMethod]
    public void TheSuiteRunsOnMicrosoftTestingPlatform() =>
        Assert.AreEqual(
            "Checkout.Tests",
            Assembly.GetEntryAssembly()?.GetName().Name,
            "TODO-00: the tests still run inside VSTest's testhost. Select the MTP runner (see the README).");

    // --filter "TestCategory=..." can only select a class that declares a category.
    [TestMethod]
    public void EveryTestClassDeclaresACategory()
    {
        var uncategorized = typeof(RunnerTests).Assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<TestClassAttribute>() is not null)
            .Where(t => t.GetCustomAttributes<TestCategoryAttribute>().Any() is false)
            .Select(t => t.Name)
            .ToList();

        Assert.IsEmpty(uncategorized, $"TODO-04: add [TestCategory] to {string.Join(", ", uncategorized)}.");
    }
}
