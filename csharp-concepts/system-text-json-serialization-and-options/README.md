# System.Text.Json: Serialization and Options

## Goal

Build the `JsonSerializerOptions` instances a real application needs, and see how each
setting changes what is written, what is accepted, and what fails loudly.

## Prerequisites

- C# records and attributes
- Basic JSON
- .NET 10 SDK

## Task

`JsonCodec` wraps `JsonSerializer` with four shared option sets: the everyday ones, a strict
set for contracts, a lenient set for config files, and a set for JavaScript clients. The
wrapper methods already exist; the option sets and the `Shape` hierarchy are empty. The
defaults of `System.Text.Json` are lenient on read and silent on loss, so most tests
check that a bad payload now throws or that a name is now matched.

## Instructions

Complete the following TODOs:

- TODO-00: Build `JsonCodec.Options` (Web defaults, omit nulls, string enums).
- TODO-01: Point `JsonCodec.StrictOptions` at the .NET 10 strict preset.
- TODO-02: Annotate `Shape` so the hierarchy round-trips with a `kind` discriminator.
- TODO-03: Build `JsonCodec.ConfigOptions` (comments and trailing commas) from `Options`.
- TODO-04: Build `JsonCodec.JavaScriptOptions` (numbers as strings) from `Options`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): Measure `JsonSerializer.Serialize` 20,000 times with one shared
  options instance and with a `new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }`
  per call. Compare time and allocated bytes.
