# Partial Types and Partial Members

## Goal

Split one type across several files, use the legacy and extended forms of partial methods, implement partial properties, indexers, constructors and events, and understand the contract between hand-written code and a source generator.

## Prerequisites

- C# classes, properties and events
- Primary constructors
- .NET 10 SDK (C# 14)

## Task

The `src/PartialLab` folder holds several types that are split into files named `Type.Part.cs`. Defining declarations are already written. You fill the implementing parts. `Greeter.Generated.cs` simulates what a source generator would emit: no generator package is used, and you must not edit that file.

## Instructions

Complete the following TODOs:

- TODO-00: `Order.Comparison.cs`: add `sealed` from the second part and implement `IComparable<Order>`. Modifiers and interfaces are merged across parts.
- TODO-01: `Quiet.cs` and `Loud.Hooks.cs`: call a legacy partial hook that nobody implements (the call and its argument vanish), and implement a hook that does exist.
- TODO-02: `Widget.Members.cs`: implement the partial property, the partial indexer and the extended partial method. The implementing property cannot be an auto-property.
- TODO-03: `Widget.Lifecycle.cs`: implement the partial constructor (with validation) and the partial event with explicit `add` and `remove`, plus the raise helper.
- TODO-04: `Greeter.cs`: provide what the generated half needs (`Prefix`, from the primary constructor parameter) and use the generated `Greet` method.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): Rewrite the `Title` implementation using the C# 14 `field` keyword instead of the `_title` field. Does the property still compile as an implementing declaration?
- TODO-06 (optional): Add a second implementing body for `Describe` in another file, or remove the implementation of `Describe`. Read the compiler error code and match it with the rule in the concept.
