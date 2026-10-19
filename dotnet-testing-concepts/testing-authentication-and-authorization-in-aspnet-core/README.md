# Testing Authentication and Authorization in ASP.NET Core

## Goal

The notes API in `src/Notes.Api` is already fully implemented: JWT bearer authentication, an `Admin` policy that
requires a role, and a resource-based `NoteOwner` policy whose handler needs the note itself. Your job is to write
the MSTest tests that prove who gets in and who stays out: a fake authentication scheme for the HTTP tests, unit
tests for the handler and the policies, and an audit that fails when someone maps an endpoint and forgets to
decide who may call it.

## Prerequisites

- Authentication and authorization fundamentals in ASP.NET Core (schemes, policies, roles)
- The Integration Testing ASP.NET Core with WebApplicationFactory lab
- .NET 10 SDK

## Task

Read `src/Notes.Api/Program.cs` and the two files in `src/Notes.Api/Notes`, then open `tests/Notes.Api.Tests`.
Every test method is already declared with a hint and fails with `TODO-NN: write this test` until you write it.

Three mistakes are easy to make here, and each one gives you a green suite that proves less than it seems:

- testing only the happy path, so an endpoint mapped without any authorization is never visited;
- testing only as the admin or the owner, so a policy that lets everyone in still passes;
- asserting "not successful" instead of the exact 401 or 403.

## Instructions

Complete the following TODOs:

- TODO-00: `TestAuthHandler`: build the caller from the `X-Test-User` and `X-Test-Roles` headers, and nobody
  when the header is missing.
- TODO-01: `NotesApiFactory`: register the fake scheme in `ConfigureTestServices` and make it the default.
- TODO-02: 401 for an anonymous caller, and a health endpoint that stays open.
- TODO-03: the `NoteOwner` rule through HTTP: the owner, another user (403, and the note is still there), a moderator.
- TODO-04: the `Admin` policy: 403 for every non-admin, 401 for anonymous, 200 for an admin.
- TODO-05: `NoteOwnerHandler` as a plain class, with an `AuthorizationHandlerContext`.
- TODO-06: the real policy registration through `IAuthorizationService`, with no host.
- TODO-07: `EndpointAuditTests`: every endpoint has `RequireAuthorization` or `AllowAnonymous`, and the audit
  really sees the endpoints.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): `JwtAuthenticationTests`: real tokens signed with the test key, against the production JWT
  bearer handler: a valid token, the `sub` claim as the note owner, a role claim, an expired token, another
  audience.
- TODO-09 (optional): Mutation check. Break the production code on purpose, one change at a time: make the
  `Admin` policy `RequireAuthenticatedUser()`, make `NoteOwnerHandler` succeed for any user, forget the moderator,
  return `Results.Challenge()` instead of `Results.Forbid()`, map `app.MapGet("/notes-export", ...)` with no
  authorization, set `ValidateLifetime = false`. Each change should turn at least one test red.
