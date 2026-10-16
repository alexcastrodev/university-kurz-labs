# Authentication and Authorization Fundamentals

## Goal

Wire the cookie scheme, a fallback policy, role, claim and custom-requirement policies and a resource-based check, then predict for each request whether it gets a 200, a 401, a 403 or a redirect, and why.

## Prerequisites

- Basic minimal APIs
- Middleware order (see the pipeline lab)
- .NET 10 SDK

## Task

`Program.cs` has the endpoints but no security: every protected endpoint fails with a `500` because no authentication or authorization service is registered. The `/login` endpoint signs in whoever asks (demo only) so the tests can create `ana` (role admin, department sales, age 30) and `bob` (age 16) with a real cookie. No Docker and no external identity provider.

## Instructions

Complete the following TODOs:

- TODO-00: Register authentication with the cookie scheme as default.
- TODO-01: Register `MinimumAgeHandler` and `OwnerHandler`.
- TODO-02: Set a fallback policy that requires an authenticated user.
- TODO-03: Add the `AdminOnly`, `Sales` and `Adult` policies.
- TODO-04: Sign in with an identity that counts as authenticated.
- TODO-05: Make `/public` anonymous.
- TODO-06: Require authentication on `/text` and `/me`.
- TODO-07: Make `/text-401` answer 401 instead of a redirect.
- TODO-08: Attach the matching policy to `/admin`, `/sales` and `/adult`.
- TODO-09: `MinimumAgeHandler`: succeed from the `age` claim.
- TODO-10: `OwnerHandler`: succeed when the user owns the resource.

Run the tests until they all pass. Notice which endpoints redirect and which answer 401, and why.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-11 (optional): Add `/admin-redirect`, protected by `AdminOnly`, that keeps redirecting an anonymous caller to the login page even though it returns JSON (`AllowCookieRedirect`).
- TODO-12 (optional): In the tests, replace the cookie scheme with a test scheme that authenticates from an `X-Test-User` header (`ConfigureTestServices`), so a test needs no `/login` call.
