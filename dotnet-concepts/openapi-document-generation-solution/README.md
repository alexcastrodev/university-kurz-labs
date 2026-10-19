# OpenAPI Document Generation

## Goal

Generate an OpenAPI document with the built-in `Microsoft.AspNetCore.OpenApi`
package and control its content with endpoint metadata, transformers, exclusion
and several named documents.

## Prerequisites

- Minimal APIs and route groups
- Endpoint metadata (see the parameter binding lab)
- .NET 10 SDK

## Task

`Program.cs` already has the endpoints, but no document is served yet. You will
register the generator, serve `/openapi/v1.json`, then shape what it contains.
The tests parse the served JSON with `System.Text.Json.Nodes` and check paths and
properties, not the whole text (the generator output changes between versions).

## Instructions

Complete the following TODOs in `src/OpenApiLab/Program.cs`:

- TODO-00: Call `AddOpenApi` and `MapOpenApi` so `/openapi/v1.json` is served.
- TODO-01: Give `GET /games/{id}` a summary, an operation name, a tag and typed
  `200` and `404` responses using endpoint metadata.
- TODO-02: Add a document transformer that sets the title, the version and a
  server.
- TODO-03: Mark only `GET /games/legacy` as deprecated, using a transformer
  attached to that endpoint.
- TODO-04: Exclude `/internal/health` from the document without removing the
  endpoint.
- TODO-05: Register a second document named `admin` and put `/admin/stats` only
  there (endpoints without a group name appear in every document).

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Add a schema transformer that sets the description of every
  `decimal` to `Money in euros`. This enables the last test.
- TODO-07 (optional): Set `options.OpenApiVersion` to `OpenApiSpecVersion.OpenApi3_0`
  and compare how `price` and nullable properties are written in 3.0 and 3.1.
