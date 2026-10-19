# System.Text.Json: Source Generation and Utf8JsonReader

## Goal

Replace reflection-based JSON with a source-generated `JsonSerializerContext`, learn what it does and does not discover, and drop to `Utf8JsonReader` / `Utf8JsonWriter` when you want to process JSON without building objects or strings.

## Prerequisites

- C# records and attributes
- Basic `JsonSerializer.Serialize` / `Deserialize`
- `Span<T>` and `ref struct` basics
- .NET 10 SDK

## Task

`Order` and `Person` are given. `AppJsonContext` is an empty partial class and `Codec` is stubbed. The System.Text.Json source generator ships with the SDK, so no package is needed: you only declare attributes. `Person` is never registered on purpose, it plays the "forgotten type".

## Instructions

Complete the following TODOs (in `AppJsonContext.cs` and `Codec.cs`):

- TODO-00: Declare the context with `[JsonSourceGenerationOptions]` and `[JsonSerializable]`, then serialize and deserialize `Order` through `JsonTypeInfo<Order>`.
- TODO-01: Serialize an array through the generated `OrderArray` metadata and ask the context which types it knows (`GetTypeInfo`). Collections are not discovered.
- TODO-02: Keep reflection as a fallback with `JsonTypeInfoResolver.Combine` for types you did not list.
- TODO-03: Walk a payload with `Utf8JsonReader` (`TokenType`, `ValueTextEquals`, `Skip`, `CurrentDepth`) and sum values without allocating strings.
- TODO-04: Write JSON straight into an `IBufferWriter<byte>` with `Utf8JsonWriter`, see its encoder and its structure validation.
- TODO-05: Read data that arrives in pieces with `isFinalBlock: false`, `JsonReaderState` and `BytesConsumed`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Add a second context with `GenerationMode = JsonSourceGenerationMode.Serialization`. Write an `Order` with it, then try to read one. What happens and why?
- TODO-07 (optional): Parse a payload with `JsonDocument`, return a `JsonElement` from the method that disposed the document, and observe the exception. Then return `element.Clone()` instead.
