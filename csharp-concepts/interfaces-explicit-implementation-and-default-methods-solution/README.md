# Interfaces: Explicit Implementation and Default Methods (Solution)

Official solution for the `interfaces-explicit-implementation-and-default-methods` lab.

## What changed from the starter

- `Person.CompareTo` orders by age, then by name with `string.CompareOrdinal`, and puts any
  instance after `null`. `List<Person>.Sort()` finds it through `IComparable<Person>`.
- `PersonByNameComparer` is a separate `IComparer<Person>`: one type can have one natural
  order (`IComparable`) and any number of alternative orders (`IComparer`).
- `Person.Equals(Person?)` is the typed `IEquatable<Person>` method. `Equals(object?)` and
  `GetHashCode` must agree with it, otherwise `HashSet<Person>` and `Dictionary` misbehave.
  The hash code uses the same case-insensitive comparison as `Equals`.
- `Cowboy` implements `Draw` twice, explicitly. Each body is reachable only through its own
  interface, which is the only way to give two interfaces different behavior for the same
  signature. The class itself has no public `Draw`.
- `ILogger.Info` has a default body. `ListLogger` implements only `Log`, and `Info` is called
  through the interface type, because a default method is not a member of the implementing class.
- `Playlist` exposes the generic `GetEnumerator` publicly and hides the non-generic one with an
  explicit implementation that forwards to it. This is how the BCL collections do it.

## Bonus solutions

- `IUnit` declares a `static abstract` property (C# 11). `Units.Describe<T>` reads `T.Symbol`
  without an instance, which interface instance members cannot do.
- `ILeft` and `IRight` both provide a default `Name`. A class that implements both has to
  resolve the conflict itself; `Both` does it with two explicit implementations.
