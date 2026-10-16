# Strings: Comparison, Culture, and Ordering

## Goal

Choose between ordinal and culture-sensitive string comparison, sort text the way people
expect, order numbered names with `NumericOrdering`, and avoid locale-dependent bugs.

## Prerequisites

- C# strings and LINQ
- `CultureInfo` basics
- .NET 10 SDK

## Task

`TextOps` has eight small operations, each with one right comparison. The tests feed text
where the choices disagree: uppercase against lowercase, `é` precomposed against `e` plus a
combining accent, a soft hyphen, the Turkish `I`, and `file10` against `file2`.

The cultural tests need ICU, which is present on a normal macOS, Linux or Windows install.
In a container without ICU, or with `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`, they fail by
design (see the bonus).

## Instructions

Complete the following TODOs:

- TODO-00: `SortForPerson` with a cultural comparer.
- TODO-01: `SortOrdinal`.
- TODO-02: `SameIdentifier` with `OrdinalIgnoreCase`.
- TODO-03: `SameText` with the invariant culture.
- TODO-04: `LowerProtocol` without the current culture.
- TODO-05: `NaturalOrder` with `CompareOptions.NumericOrdering`.
- TODO-06: `ContainsIgnoringAccents` with `CompareOptions`.
- TODO-07: `Canonical` with `string.Intern`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): Run the finished tests with `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`.
  Which ones fail, and which one would silently return a wrong answer in production?
