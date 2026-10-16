# Parameter Binding and Endpoint Filters

## Goal

Understand how minimal APIs bind handler parameters (route, query, header,
services, body), extend binding with `TryParse` and `BindAsync`, expand a request
type with `[AsParameters]`, and run a rule around a whole group with an endpoint
filter.

## Prerequisites

- Basic minimal APIs and route groups
- C# records and static members
- .NET 10 SDK

## Task

Several endpoints are half built. The first thing you will notice is that all of
them fail, even the ones you did not touch. Find out why, then build the custom
binders and the filter.

## Instructions

Complete the following TODOs:

- TODO-00: Find out why every endpoint answers `500` and register the missing
  service in `Program.cs`.
- TODO-01: Mark the `Clock` parameter of `/time` with an explicit source.
- TODO-02: Implement `Point.TryParse` (`/map?p=12.3,10.1`).
- TODO-03: Implement `Caller.BindAsync` (header, then query fallback, `null` when absent).
- TODO-04: Give each property of `SearchRequest` an explicit binding source.
- TODO-05: Implement `OrderValidationFilter` (reject bad orders, trim the name).
- TODO-06: Attach the filter to the `/orders` group in `Program.cs`.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-07 (optional): Remove the registration from TODO-00 again, then call
  `/time` with and without `[FromServices]`. Compare the status codes.
- TODO-08 (optional): Add a `[FromHeader] int[]` parameter and send
  `X-Nums: 3, 4`. What happens when the header is missing?
