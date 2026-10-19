# Testing Authentication and Authorization in ASP.NET Core (Solution)

Official solution for the `testing-authentication-and-authorization-in-aspnet-core` lab.

## What changed from the starter

- **`TestAuthHandler`** returns `AuthenticateResult.NoResult()` when `X-Test-User` is missing. The handler in the
  Microsoft docs always signs a user in, which is fine for a page that needs a user and useless for testing 401.
  The identity gets `authenticationType: "Test"`; a `ClaimsIdentity` without one has `IsAuthenticated == false`.
- **`NotesApiFactory`** adds the scheme in `ConfigureTestServices` with `AddAuthentication("Test")`. That runs after
  `Program.cs`, so it replaces JWT bearer as the default scheme for authenticate, challenge and forbid. A challenge
  from the fake scheme is a plain 401, a forbid is a plain 403.
- **Every rule is tested from the side that must be refused.** Bob reading or deleting Alice's note gets 403 and
  the note survives; a moderator and a plain user get 403 on `/admin/stats`. A suite that only calls as the owner
  or the admin passes when the policy lets everyone in.
- **Exact status codes.** 401 (no user) and 403 (a user without permission) are asserted separately, so returning
  `Results.Challenge()` where `Results.Forbid()` belongs turns a test red.
- **`NotesAuthorizationTests`** checks the handler with an `AuthorizationHandlerContext`, and then the real
  registration (`AddNotesAuthorization`, the same call `Program.cs` makes) through `IAuthorizationService` built
  from a `ServiceCollection` with `AddLogging()`. No host, milliseconds.
- **`EndpointAuditTests`** reads `EndpointDataSource` from the started host and requires `IAuthorizeData` or
  `IAllowAnonymous` metadata on every `RouteEndpoint`. A second test checks that the audit sees the known routes, so
  an empty list cannot make it pass.
- **`JwtAuthenticationTests`** uses the production JWT bearer handler with tokens from `JsonWebTokenHandler`, signed
  with the same test key the factory passes with `UseSetting`. The `sub` claim becomes the owner of a new note
  (`ClaimTypes.NameIdentifier`), a `role` claim in the token satisfies `RequireRole("admin")`, and an expired token gets `WWW-Authenticate: Bearer error="invalid_token",
  error_description="The token expired at '...'"`. Only these tests see expiry and audience checks. The key is
  32 characters because HS256 needs at least 256 bits; a shorter one fails with `IDX10720`.

## Bonus notes

Mutation check, run against this solution. Each change was tried on its own:

| Change to the production code | Tests that fail |
| --- | --- |
| `Admin` policy becomes `RequireAuthenticatedUser()` | both rows of `ShouldReturn403OnAdminStatsToAnyoneWhoIsNotAnAdmin`, `TheAdminPolicyShouldRequireTheAdminRole` |
| `NoteOwnerHandler` succeeds for any user | the two HTTP 403 tests, `TheHandlerShouldRefuseAnotherUser`, `TheHandlerShouldNotTreatAnAdminAsAModerator`, the `bob` row of the policy test |
| `NoteOwnerHandler` forgets the moderator | `ShouldLetAModeratorDeleteSomeoneElsesNote`, `TheHandlerShouldAllowAModerator`, the `carol` row of the policy test |
| `GET /notes/{id}` returns `Results.Challenge()` instead of `Results.Forbid()` | `ShouldReturn403WhenAnotherUserReadsTheNote` |
| A new `app.MapGet("/notes-export", ...)` with no authorization | only `EveryEndpointShouldRequireAuthorizationOrAllowAnonymousExplicitly` |
| The `/notes` group loses `RequireAuthorization()` | the audit, `ShouldReturn401ToAnAnonymousCaller`, and the expired and wrong audience JWT tests |
| `ValidateLifetime = false` in `Program.cs` | only `AnExpiredTokenShouldBeRejectedWith401` |
| `DELETE` answers 204 without removing the note | `ShouldLetAModeratorDeleteSomeoneElsesNote` |

The forgotten endpoint is the point of the audit: no other test calls `/notes-export`, because nobody knew it existed.
The `ValidateLifetime` row is the price of the fake scheme: the fake never reads a token, so only the JWT tests can see it.
