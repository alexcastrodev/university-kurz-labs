using NuGet.Versioning;

namespace NugetRestoreVersionsAndDependencyResolution;

public sealed record Resolution(IReadOnlyDictionary<string, NuGetVersion> Packages, IReadOnlyList<string> Diagnostics);

public static class Resolver
{
    /// <summary>Ready to use: floating ranges take the highest match, every other range takes the lowest.</summary>
    public static NuGetVersion? Pick(IEnumerable<NuGetVersion> candidates, VersionRange range) =>
        range.IsFloating ? PickFloating(candidates, range) : PickLowest(candidates, range);

    /// <summary>Ready to use: resolve, then diagnose.</summary>
    public static Resolution Run(Feed feed, IReadOnlyList<Dep> roots)
    {
        var packages = Resolve(feed, roots);
        return new Resolution(packages, Diagnose(feed, roots, packages));
    }

    public static NuGetVersion? PickLowest(IEnumerable<NuGetVersion> candidates, VersionRange range)
    {
        // TODO-01: The "lowest applicable version" rule. Keep the candidates that satisfy the range and
        //   return the lowest one (order with VersionComparer.Default), or null when none does.
        //   Version="1.2" does NOT mean "1.2 exactly" or "the newest": it means the lowest available >= 1.2.
        return candidates.Where(range.Satisfies).OrderBy(v => v, VersionComparer.Default).FirstOrDefault();
    }

    public static NuGetVersion? PickFloating(IEnumerable<NuGetVersion> candidates, VersionRange range)
    {
        // TODO-02: A floating range ("1.*", "1.2.*") resolves to the HIGHEST match, and pre-release versions
        //   are ignored unless the floating pattern itself has a label ("1.*-*").
        //   VersionRange.FindBestMatch already implements this: call it with the candidates.
        return range.FindBestMatch(candidates);
    }

    public static NuGetVersion? PickForGroup(IEnumerable<NuGetVersion> candidates, IReadOnlyList<VersionRange> ranges)
    {
        // TODO-03: Cousin dependencies: several branches at the same depth ask for the same package.
        //   One range: delegate to Pick. Several ranges: the lowest candidate that satisfies ALL of them
        //   (use Satisfies on every range). Return null when no version satisfies the whole group.
        if (ranges.Count == 1) return Pick(candidates, ranges[0]);
        return candidates.Where(v => ranges.All(r => r.Satisfies(v)))
            .OrderBy(v => v, VersionComparer.Default).FirstOrDefault();
    }

    public static IReadOnlyDictionary<string, NuGetVersion> Resolve(Feed feed, IReadOnlyList<Dep> roots)
    {
        // TODO-04: Walk the graph level by level (breadth first), starting with the roots (depth 1).
        //   Rule "nearest wins": the first level at which an id is requested decides it, and requests for
        //   an id that is already resolved (deeper in the graph) are ignored, they do not change the choice.
        //   For each id requested at this level, group its requests (ranges) and call PickForGroup over
        //   feed.Versions(id). If the group is unsatisfiable, fall back to Pick with the FIRST request's range.
        //   If nothing can be picked, leave the id out of the result.
        //   Queue the dependencies (feed.DependenciesOf) of each newly resolved package for the next level.
        var resolved = new Dictionary<string, NuGetVersion>(StringComparer.OrdinalIgnoreCase);
        IEnumerable<Dep> level = roots;
        while (level.Any())
        {
            var next = new List<Dep>();
            foreach (var group in level.Where(d => !resolved.ContainsKey(d.Id))
                         .GroupBy(d => d.Id, StringComparer.OrdinalIgnoreCase).ToList())
            {
                var ranges = group.Select(d => d.Parsed).ToList();
                var versions = feed.Versions(group.Key);
                var picked = Resolver.PickForGroup(versions, ranges) ?? Pick(versions, ranges[0]);
                if (picked is null) continue;
                resolved[group.Key] = picked;
                next.AddRange(feed.DependenciesOf(group.Key, picked));
            }
            level = next;
        }
        return resolved;
    }

    public static IReadOnlyList<string> Diagnose(Feed feed, IReadOnlyList<Dep> roots, IReadOnlyDictionary<string, NuGetVersion> packages)
    {
        // TODO-05: Check every edge: the roots (as edges from the project) and the dependencies of every
        //   resolved package (feed.DependenciesOf(id, version)), visiting packages ordered by id
        //   (StringComparer.OrdinalIgnoreCase) so the output is deterministic. For an edge to `dep`:
        //   - not in `packages`: "NU1101: Unable to find package {id}" when the feed has no versions of it,
        //     otherwise "NU1102: Unable to find package {id} with version {range}".
        //   - resolved but not satisfying the edge's range:
        //       * if the id is one of the roots (the direct dependency won):
        //         "NU1605: Detected package downgrade: {id} from {range.MinVersion.ToNormalizedString()} to {resolved.ToNormalizedString()}"
        //       * otherwise: "NU1107: Version conflict detected for {id}: {parentId} {parentVersion} requires {range}, resolved {resolved}"
        //   Print versions with ToNormalizedString() (3.0 prints as 3.0.0); {range} is the range text as written (dep.Range)
        //   A satisfied edge produces nothing. Raising the direct reference (e.g. to 4.0) makes NU1605 disappear.
        var result = new List<string>();
        var rootIds = roots.Select(r => r.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);

        void Check(Dep dep, string? parent, NuGetVersion? parentVersion)
        {
            var range = dep.Parsed;
            if (!packages.TryGetValue(dep.Id, out var got))
            {
                result.Add(feed.Versions(dep.Id).Count == 0
                    ? $"NU1101: Unable to find package {dep.Id}"
                    : $"NU1102: Unable to find package {dep.Id} with version {dep.Range}");
            }
            else if (!range.Satisfies(got))
            {
                result.Add(rootIds.Contains(dep.Id)
                    ? $"NU1605: Detected package downgrade: {dep.Id} from {range.MinVersion!.ToNormalizedString()} to {got.ToNormalizedString()}"
                    : $"NU1107: Version conflict detected for {dep.Id}: {parent} {parentVersion} requires {dep.Range}, resolved {got.ToNormalizedString()}");
            }
        }

        foreach (var root in roots) Check(root, null, null);
        foreach (var (id, version) in packages.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
            foreach (var dep in feed.DependenciesOf(id, version)) Check(dep, id, version);
        return result;
    }

    public static IReadOnlyList<string>? Why(Feed feed, IReadOnlyList<Dep> roots, IReadOnlyDictionary<string, NuGetVersion> packages, string target)
    {
        // TODO-06 (optional): Like `dotnet nuget why`: the shortest chain of package ids from a root to
        //   `target` following the RESOLVED versions' dependencies, e.g. ["Logging", "Options", "Primitives"].
        //   Breadth first search with paths in the queue, skipping ids already seen. Null when target is not in the graph.
        var queue = new Queue<List<string>>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in roots)
            if (packages.ContainsKey(r.Id) && seen.Add(r.Id)) queue.Enqueue([r.Id]);
        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var last = path[^1];
            if (string.Equals(last, target, StringComparison.OrdinalIgnoreCase)) return path;
            foreach (var dep in feed.DependenciesOf(last, packages[last]))
                if (packages.ContainsKey(dep.Id) && seen.Add(dep.Id)) queue.Enqueue([.. path, dep.Id]);
        }
        return null;
    }

    public static string? CheckLocked(IReadOnlyDictionary<string, NuGetVersion> locked, IReadOnlyDictionary<string, NuGetVersion> fresh)
    {
        // TODO-07 (optional): Lock file consistency, as in `--locked-mode`. Return null when `fresh` has
        //   exactly the same ids and versions as `locked` (ids compared ignoring case). Otherwise return
        //   "NU1004: The packages lock file is inconsistent with the project dependencies" and nothing else.
        var same = locked.Count == fresh.Count && fresh.All(p =>
            locked.Any(l => string.Equals(l.Key, p.Key, StringComparison.OrdinalIgnoreCase)
                            && VersionComparer.Default.Equals(l.Value, p.Value)));
        return same ? null : "NU1004: The packages lock file is inconsistent with the project dependencies";
    }
}
