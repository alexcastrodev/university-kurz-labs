# Integration Testing ASP.NET Core with WebApplicationFactory (Solution)

Official solution for the `integration-testing-with-webapplicationfactory` lab.

## What changed from the starter

- **No `public partial class Program`.** Since .NET 10 a source generator makes the `Program` class of top-level
  statements public, so `WebApplicationFactory<Program>` compiles without it (analyzer ASP0027 suggests removing
  an explicit declaration).
- **`OrdersApiFactory`** overrides `ConfigureWebHost`. `UseSetting("Orders:MaxQuantity", "3")` reaches the read
  that `Program.cs` does before `Build`. `ConfigureAppConfiguration` would not: with minimal hosting its sources
  are only added after `Build`, so `OrderRules` would still get 10 (`ConfigurationOverrideTests` proves both).
- **`ConfigureTestServices`** runs after `Program.cs` registered its services. `RemoveAll<IInventoryClient>()`
  drops the typed `HttpClient` registration and the fake takes its place, so no test calls the network.
- **`OrdersApiTests`** shares one factory per class: a `static` field created in `[ClassInitialize]` and disposed
  in an async `[ClassCleanup]`. This is MSTest's `IClassFixture<T>`. Because `OrderStore` is a singleton, the
  store and the fake inventory are reset in `[TestInitialize]`, and the class is `[DoNotParallelize]` so two
  tests cannot reset and fill the same store at once. The alternative is one factory per test (isolated, slower).
- **Requests use the production record** (`new PlaceOrderRequest(...)`), so a misspelled property cannot produce
  a 400 that looks like a validation failure.
- **Errors are read as ProblemDetails.** `GetFromJsonAsync` is used only for 200s, because it throws
  `HttpRequestException` on any other status. A 404 or 400 is read with `GetAsync`/`PostAsJsonAsync` and then
  `ReadFromJsonAsync<ProblemDetails>` or `ReadFromJsonAsync<HttpValidationProblemDetails>`.
- **The limit test asks for 4, not 50.** 50 is rejected by the default limit of 10 too, so that test would pass
  even if the override never reached `Program.cs`.
- **The redirect test uses `AllowAutoRedirect = false`** and asserts the 302 and the `Location` header. A client
  that follows the redirect sees the 200 of `/orders/{id}` and cannot tell a redirect from a direct answer.
- **`KestrelTests`** uses `UseKestrel(0)` and `StartServer()` (new in .NET 10). `CreateClient()` then points at
  `http://127.0.0.1:<port>`, and `factory.Server` throws `NotSupportedException` because there is no `TestServer`.

## Bonus notes

Mutation check, run against this solution. Each change was tried on its own:

| Change to the production code | Tests that fail |
| --- | --- |
| `Program.cs` builds `new OrderRules(10)` and ignores the configuration | `ShouldRejectAQuantityJustAboveTheConfiguredLimit` |
| The limit check is off by one (`> MaxQuantity + 1`) | `ShouldRejectAQuantityJustAboveTheConfiguredLimit` |
| `/orders/latest` returns `Results.Ok(order)` instead of a redirect | `ShouldRedirectLatestToTheNewestOrder` |
| `OrderStore.Latest` returns the oldest order | `ShouldRedirectLatestToTheNewestOrder` |
| The stock check is skipped | `ShouldReturn409WhenTheInventoryHasNotEnoughStock` |
| The 404 is `Results.NotFound()` with no ProblemDetails body | `ShouldReturnProblemDetailsForAnUnknownOrder` |

And one change to the tests: without the reset in `[TestInitialize]`, `ShouldListExactlyTheOrdersPlacedInThisTest`
fails with "Different number of elements", because the orders of the other tests are still in the singleton.

The naive versions pass every mutation in their row: a limit test with 50 passes with a hard-coded 10, and a
redirect test with the default client passes when `/orders/latest` returns the order directly.
