# Authentication and Authorization Fundamentals (Solution)

Official solution for the `authentication-and-authorization-fundamentals` lab, with the optional tasks implemented.

## What changed from the starter

- `Program.cs`: `AddAuthentication(Cookies).AddCookie()`, both handlers as `IAuthorizationHandler` singletons, and `AddAuthorizationBuilder` with a fallback policy plus the `AdminOnly`, `Sales` and `Adult` policies. There is no `UseAuthentication()` or `UseAuthorization()`: `WebApplication` adds both when the services are registered.
- `/login`: the `ClaimsIdentity` is created with the cookie scheme name. Without an authentication type `IsAuthenticated` is `false` and every later request is anonymous.
- `MinimumAgeHandler` calls `Succeed` only for a valid `age` claim. It never calls `Fail`, so a missing claim leaves the requirement unmet and the caller gets a 403.
- `OwnerHandler` is a resource handler: the document owner is the resource passed to `IAuthorizationService.AuthorizeAsync`.

## Why it is shaped this way

- 401 means the caller is not known (challenge), 403 means known and refused (forbid).
- Since .NET 10 the cookie handler answers 401/403 for endpoints recognized as APIs (JSON, `TypedResults`). A plain string endpoint still redirects, so `/text` redirects and `/me` answers 401. `DisableCookieRedirect()` and `AllowCookieRedirect()` force either behavior per endpoint.
- `/fallback` has no metadata and is still closed because of the fallback policy.
- `/login` and `/public` need `AllowAnonymous()`, or the fallback policy would close them too.

## Bonus notes

- `/admin-redirect` returns JSON like an API endpoint but uses `AllowCookieRedirect()`, so an anonymous caller is sent to `/Account/Login`.
- `BonusTests` swaps the default scheme for a `TestAuthHandler` through `ConfigureTestServices`. It authenticates from the `X-Test-User` header and returns `NoResult` without it, so no cookie or `/login` call is needed.

## Running

```bash
dotnet test
```
