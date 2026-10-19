using NuGet.Versioning;

namespace NugetRestoreVersionsAndDependencyResolution.Tests;

public class NugetRestoreVersionsAndDependencyResolutionTests
{
    private static NuGetVersion V(string v) => NuGetVersion.Parse(v);
    private static VersionRange R(string r) => VersionRange.Parse(r);
    private static IReadOnlyList<NuGetVersion> Vs(params string[] v) => v.Select(V).ToList();
    private static string Show(IReadOnlyDictionary<string, NuGetVersion> p, string id) => p[id].ToNormalizedString();

    // ---- TODO-00: ordering and range notation ----

    [Theory]
    [InlineData("1.0.0-beta.2", "1.0.0", -1)]
    [InlineData("1.0.0-beta.2", "1.0.0-beta.10", -1)]
    [InlineData("1.0.0-alpha", "1.0.0-beta", -1)]
    [InlineData("1.0", "1.0.0", 0)]
    [InlineData("1.0.0+abc", "1.0.0+def", 0)]
    [InlineData("2.0.0", "1.9.9", 1)]
    public void Compare_follows_semver_ordering(string a, string b, int expected) =>
        Assert.Equal(expected, VersionRules.Compare(a, b));

    [Theory]
    [InlineData("1.0", "1.0", true)]
    [InlineData("5.0", "1.0", true)]
    [InlineData("0.9", "1.0", false)]
    [InlineData("1.0", "[1.0]", true)]
    [InlineData("1.0.1", "[1.0]", false)]
    [InlineData("1.0", "(1.0,2.0)", false)]
    [InlineData("2.0", "(1.0,2.0)", false)]
    [InlineData("1.5", "(1.0,2.0)", true)]
    [InlineData("1.0", "[1.0,2.0)", true)]
    [InlineData("2.0", "[1.0,2.0)", false)]
    public void Satisfies_understands_range_notation(string version, string range, bool expected) =>
        Assert.Equal(expected, VersionRules.Satisfies(version, range));

    // ---- TODO-01: lowest applicable version ----

    [Theory]
    [InlineData("1.2", "1.5.0")]       // 1.2 is not on the feed: the lowest one above it
    [InlineData("1.0", "1.0.0")]       // not the newest
    [InlineData("[1.0,2.0)", "1.0.0")]
    [InlineData("(1.0,3.0)", "1.5.0")]
    [InlineData("[2.0]", "2.0.0")]
    public void PickLowest_returns_the_lowest_satisfying_version(string range, string expected) =>
        Assert.Equal(expected, Resolver.PickLowest(Vs("2.0.0", "1.0.0", "3.0.0", "1.5.0"), R(range))!.ToNormalizedString());

    [Fact]
    public void PickLowest_returns_null_when_nothing_satisfies() =>
        Assert.Null(Resolver.PickLowest(Vs("1.0.0", "2.0.0"), R("[3.0,)")));

    // ---- TODO-02: floating ----

    [Theory]
    [InlineData("1.*", "1.9.0")]
    [InlineData("1.0.*", "1.0.7")]
    [InlineData("2.*", "2.0.0")]
    public void PickFloating_returns_the_highest_stable_match(string range, string expected) =>
        Assert.Equal(expected,
            Resolver.PickFloating(Vs("1.0.0", "1.0.7", "1.9.0", "1.10.0-beta.1", "2.0.0", "3.0.0"), R(range))!.ToNormalizedString());

    [Fact]
    public void PickFloating_ignores_prerelease_versions()
    {
        var picked = Resolver.PickFloating(Vs("1.0.0", "1.1.0", "1.2.0-beta.1"), R("1.*"));
        Assert.Equal("1.1.0", picked!.ToNormalizedString());
    }

    [Fact]
    public void Pick_dispatches_between_lowest_and_floating()
    {
        var feed = Vs("1.0.0", "1.4.0", "1.9.0");
        Assert.Equal("1.4.0", Resolver.Pick(feed, R("1.2"))!.ToNormalizedString());
        Assert.Equal("1.9.0", Resolver.Pick(feed, R("1.*"))!.ToNormalizedString());
    }

    // ---- TODO-03: cousin dependencies ----

    [Fact]
    public void PickForGroup_single_request_uses_the_normal_rule() =>
        Assert.Equal("1.2.0", Resolver.PickForGroup(Vs("1.0.0", "1.2.0", "2.0.0"), [R("1.1")])!.ToNormalizedString());

    [Fact]
    public void PickForGroup_takes_the_lowest_version_satisfying_every_request() =>
        Assert.Equal("1.5.0",
            Resolver.PickForGroup(Vs("1.0.0", "1.2.0", "1.5.0", "2.0.0"), [R("1.1"), R("[1.4,2.0)")])!.ToNormalizedString());

    [Fact]
    public void PickForGroup_returns_null_when_ranges_do_not_overlap() =>
        Assert.Null(Resolver.PickForGroup(Vs("1.0.0", "2.0.0"), [R("[1.0]"), R("[2.0]")]));

    // ---- TODO-04: nearest wins ----

    [Fact]
    public void Resolve_picks_the_lowest_version_for_every_package()
    {
        var feed = new Feed()
            .Add("App.Lib", "1.0.0", new Dep("Json", "2.1"))
            .Add("Json", "2.0.0").Add("Json", "2.2.0").Add("Json", "3.0.0");
        var packages = Resolver.Resolve(feed, [new Dep("App.Lib", "1.0")]);
        Assert.Equal("1.0.0", Show(packages, "App.Lib"));
        Assert.Equal("2.2.0", Show(packages, "Json"));
    }

    [Fact]
    public void Resolve_nearest_request_wins_over_a_deeper_one()
    {
        // App -> A -> C [1.0]            (depth 2)
        // App -> B -> D -> C 2.0         (depth 3, ignored)
        var feed = new Feed()
            .Add("A", "1.0.0", new Dep("C", "[1.0]"))
            .Add("B", "1.0.0", new Dep("D", "1.0"))
            .Add("D", "1.0.0", new Dep("C", "2.0"))
            .Add("C", "1.0.0").Add("C", "2.0.0");
        var packages = Resolver.Resolve(feed, [new Dep("A", "1.0"), new Dep("B", "1.0")]);
        Assert.Equal("1.0.0", Show(packages, "C"));
    }

    [Fact]
    public void Resolve_direct_dependency_beats_a_transitive_request()
    {
        var feed = new Feed()
            .Add("A", "1.0.0", new Dep("C", "3.0"))
            .Add("C", "2.0.0").Add("C", "3.0.0").Add("C", "4.0.0");
        var packages = Resolver.Resolve(feed, [new Dep("A", "1.0"), new Dep("C", "2.0")]);
        Assert.Equal("2.0.0", Show(packages, "C"));
    }

    [Fact]
    public void Resolve_merges_cousins_at_the_same_depth()
    {
        var feed = new Feed()
            .Add("A", "1.0.0", new Dep("C", "1.0"))
            .Add("B", "1.0.0", new Dep("C", "2.0"))
            .Add("C", "1.0.0").Add("C", "2.0.0").Add("C", "3.0.0");
        var packages = Resolver.Resolve(feed, [new Dep("A", "1.0"), new Dep("B", "1.0")]);
        Assert.Equal("2.0.0", Show(packages, "C"));
    }

    [Fact]
    public void Resolve_floating_root_takes_the_highest_and_follows_its_dependencies()
    {
        var feed = new Feed()
            .Add("A", "1.0.0", new Dep("Old", "1.0"))
            .Add("A", "1.3.0", new Dep("New", "1.0"))
            .Add("Old", "1.0.0").Add("New", "1.0.0");
        var packages = Resolver.Resolve(feed, [new Dep("A", "1.*")]);
        Assert.Equal("1.3.0", Show(packages, "A"));
        Assert.True(packages.ContainsKey("New"));
        Assert.False(packages.ContainsKey("Old"));
    }

    // ---- TODO-05: diagnostics ----

    [Fact]
    public void Diagnose_reports_a_downgrade_when_the_direct_reference_is_too_low()
    {
        var feed = new Feed()
            .Add("Logging", "9.0.0", new Dep("Abstractions", "9.0.0"))
            .Add("Abstractions", "8.0.0").Add("Abstractions", "9.0.0").Add("Abstractions", "10.0.0");
        Dep[] roots = [new("Logging", "9.0.0"), new("Abstractions", "8.0.0")];
        var run = Resolver.Run(feed, roots);
        Assert.Equal("8.0.0", Show(run.Packages, "Abstractions"));
        Assert.Equal(["NU1605: Detected package downgrade: Abstractions from 9.0.0 to 8.0.0"], run.Diagnostics);
    }

    [Fact]
    public void Diagnose_is_quiet_once_the_direct_reference_is_raised()
    {
        var feed = new Feed()
            .Add("Logging", "9.0.0", new Dep("Abstractions", "9.0.0"))
            .Add("Abstractions", "8.0.0").Add("Abstractions", "9.0.0").Add("Abstractions", "10.0.0");
        var run = Resolver.Run(feed, [new Dep("Logging", "9.0.0"), new Dep("Abstractions", "10.0.0")]);
        Assert.Equal("10.0.0", Show(run.Packages, "Abstractions"));
        Assert.Empty(run.Diagnostics);
    }

    [Fact]
    public void Diagnose_reports_a_conflict_for_a_diamond_with_disjoint_exact_ranges()
    {
        var feed = new Feed()
            .Add("A", "1.0.0", new Dep("C", "[1.0]"))
            .Add("B", "1.0.0", new Dep("C", "[2.0]"))
            .Add("C", "1.0.0").Add("C", "2.0.0");
        var run = Resolver.Run(feed, [new Dep("A", "1.0"), new Dep("B", "1.0")]);
        var message = Assert.Single(run.Diagnostics);
        Assert.StartsWith("NU1107: Version conflict detected for C", message);
        Assert.Contains("B 1.0.0 requires [2.0]", message);
    }

    [Fact]
    public void Diagnose_reports_missing_packages_and_missing_versions()
    {
        var feed = new Feed().Add("A", "1.0.0", new Dep("Ghost", "1.0"), new Dep("Tiny", "5.0")).Add("Tiny", "1.0.0");
        var run = Resolver.Run(feed, [new Dep("A", "1.0")]);
        Assert.Equal(
            ["NU1101: Unable to find package Ghost", "NU1102: Unable to find package Tiny with version 5.0"],
            run.Diagnostics);
    }

    [Fact]
    public void Diagnose_works_on_a_hand_made_resolution()
    {
        var feed = new Feed().Add("A", "1.0.0", new Dep("C", "3.0")).Add("C", "2.0.0");
        var packages = new Dictionary<string, NuGetVersion> { ["A"] = V("1.0.0"), ["C"] = V("2.0.0") };
        var diagnostics = Resolver.Diagnose(feed, [new Dep("A", "1.0"), new Dep("C", "2.0")], packages);
        Assert.Equal(["NU1605: Detected package downgrade: C from 3.0.0 to 2.0.0"], diagnostics);
    }

    // ---- Bonus: TODO-06 and TODO-07 ----

    [Fact]
    public void Why_returns_the_shortest_path_to_a_transitive_package()
    {
        var feed = new Feed()
            .Add("Logging", "9.0.0", new Dep("Options", "9.0.0"))
            .Add("Options", "9.0.0", new Dep("Primitives", "9.0.0"))
            .Add("Primitives", "9.0.0");
        var roots = new[] { new Dep("Logging", "9.0.0") };
        var packages = Resolver.Resolve(feed, roots);
        Assert.Equal(["Logging", "Options", "Primitives"], Resolver.Why(feed, roots, packages, "Primitives"));
        Assert.Null(Resolver.Why(feed, roots, packages, "Nope"));
    }

    [Fact]
    public void CheckLocked_detects_drift_between_the_lock_file_and_a_fresh_resolution()
    {
        var locked = new Dictionary<string, NuGetVersion> { ["A"] = V("1.0.0"), ["B"] = V("2.0.0") };
        var same = new Dictionary<string, NuGetVersion> { ["a"] = V("1.0.0"), ["b"] = V("2.0.0") };
        var drifted = new Dictionary<string, NuGetVersion> { ["A"] = V("1.0.1"), ["B"] = V("2.0.0") };
        var extra = new Dictionary<string, NuGetVersion> { ["A"] = V("1.0.0"), ["B"] = V("2.0.0"), ["C"] = V("1.0.0") };
        Assert.Null(Resolver.CheckLocked(locked, same));
        Assert.StartsWith("NU1004", Resolver.CheckLocked(locked, drifted));
        Assert.StartsWith("NU1004", Resolver.CheckLocked(locked, extra));
    }
}
