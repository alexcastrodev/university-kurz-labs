# Blazor Static SSR and Enhanced Navigation

## Goal

Build a Blazor Web App that uses only static server-side rendering (no circuit, no
WebAssembly) and see exactly what the server sends: plain HTML pages, a route
parameter with a type constraint, a query string bound into a component, and an
enhanced `EditForm` that posts with an antiforgery token, validates, and follows
Post/Redirect/Get.

## Prerequisites

- ASP.NET Core basics (routing, middleware, `WebApplicationFactory`)
- Razor components basics (`@page`, `[Parameter]`, `@code`)
- .NET 10 SDK (the tests use xUnit and Microsoft.AspNetCore.Mvc.Testing, restored from nuget.org)

## Task

The `BlazorSsrLab` web project has five pages: `Home`, `Orders`, `Search`, `Contact`
and `Thanks`, wrapped by `MainLayout` and `App.razor`. It registers only
`AddRazorComponents()`, so there is no interactive render mode. Today the home page
is empty, the layout does not load the Blazor script, the order route has no
constraint, the search page ignores the query string, and the contact page is a
plain form that Blazor does not handle. The tests call the app in memory with
`WebApplicationFactory` and check the HTML, status codes and headers the server
returns. No browser is involved.

## Instructions

Complete the following TODOs (each one is marked in the code):

- TODO-00: In `Home.razor`, render `<h1>Home</h1>` and the paragraph with the render
  time (`<span id="stamp">`). The test also checks that the page is `text/html` and
  that it references neither `blazor.server.js` nor `blazor.webassembly.js`.
- TODO-01: In `App.razor`, load `_framework/blazor.web.js`. The `<base href="/" />`
  is already there; the test checks both.
- TODO-02: In `Orders.razor`, constrain the route to an integer: `/orders/42` renders
  `Order 42` and `/orders/abc` must return `404`.
- TODO-03: In `Search.razor`, bind the `term` query string value into the `Term`
  parameter. `/search?term=blazor` shows `Results for: blazor`, and a value like
  `<b>` must come out HTML-encoded (`&lt;b&gt;`).
- TODO-04: In `Contact.razor`, replace the plain form with an enhanced `EditForm`
  named `contact`. The rendered HTML must have `method="post"`, `data-enhance`, a
  hidden `_handler` field with value `contact`, the antiforgery token
  (`__RequestVerificationToken`) and an input named `Model.Name`. Keep
  `DataAnnotationsValidator` and a `ValidationMessage`, and call `Submit` on a
  valid post.
- TODO-05: In `Contact.razor`, bind the posted form into `Model` with
  `[SupplyParameterFromForm]`. An empty name must re-render the page with
  `200` and `Name is required`. A valid name must answer `302` to
  `/thanks?name=Ana` (Post/Redirect/Get), and that page shows `Thanks, Ana`.

Run the tests until they all pass. The bonus test carries
`[Trait("Category", "Bonus")]`.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

To run only the required work: `dotnet test --filter "Category!=Bonus"`.

### What is not tested here

The tests only check what the server returns. `blazor.web.js` is never executed, so
enhanced navigation and enhanced forms are not tested as browser behavior. To see
them yourself, run `dotnet run --project src/BlazorSsrLab`, open the URL it prints
and try:

- Click the links in the nav bar with the browser developer tools Network tab open.
  An enhanced navigation is a single `fetch` for the page (the request has
  `Accept: text/html; blazor-enhanced-nav=on`), not a full document load with CSS
  and scripts again.
- On `/contact`, submit an empty name: the validation message appears without a
  page reload. Submit a name: you land on `/thanks`.
- Add `data-enhance-nav="false"` to one nav link and compare: that link reloads the
  whole document.
- Remove the script tag (TODO-01) and click again: everything still works, but every
  click is a full page load.

## Bonus (Optional)

- TODO-06 (optional): `ShouldRejectPostWithoutAntiforgeryToken` posts to `/contact`
  with the cookie but without the token field and expects `400`. There is no code to
  write: read `Program.cs` and find which line makes the server reject that post, and
  why the `EditForm` from TODO-04 adds the token field for you.
