# Testing Blazor Components with bUnit and MSTest

## Goal

`src/TaskBoard` is a small Razor class library with three components: a `Counter`, a `NewTaskForm` built on
`EditForm`, and a `TaskList` that loads tasks from an `ITaskStore`, asks `window.confirm` before deleting,
announces new tasks through JavaScript, reads a cascading `BoardSettings`, and shows a button only to leads.
The components are finished. Your job is to write the bUnit tests, in MSTest, that prove they work.

## Prerequisites

- MSTest Fundamentals: Lifecycle and TestContext
- Blazor Component Lifecycle and Parameters, Blazor Forms: EditForm and Validation
- .NET 10 SDK

## Task

Read the three `.razor` files first. Then open `tests/TaskBoard.Tests`. Every test method is already declared
with a hint and fails with `TODO-NN: write this test` until you write it. `FakeTaskStore` is ready to use: it
keeps tasks in a list, counts saves, and with `HoldLoading = true` makes the component wait until you call
`Release()`.

Each test class inherits from `BunitContext`, so `Render`, `Services`, `JSInterop` and `AddAuthorization`
are available directly. Three things will bite you, and that is the point of the lab:

- A rendered component is not finished when `Render` returns if it awaits something. An assertion made too early
  can fail, or worse, pass because the component is still loading.
- bUnit's JSInterop is strict by default. A JavaScript call that the test did not set up throws.
- A selector on a CSS class breaks when someone restyles the page. Find elements by what the user sees: an
  `aria-label`, an `id` tied to a `<label>`, the text of a button.

## Instructions

Complete the following TODOs in `tests/TaskBoard.Tests`:

- TODO-00: `CounterTests`: a click updates the status; at `Max` the button is disabled and `OnLimitReached`
  gets the count once.
- TODO-01: `NewTaskFormTests`: an empty title and an out-of-range priority show their messages and create
  nothing; a valid submit raises `OnCreate` with the values and clears the form.
- TODO-02: `TaskListTests.RenderList`: register the fake store and render `TaskList`, with the cascading
  settings when given. Read the exception from the first run: `TaskList` needs one more thing set up.
- TODO-03: `TaskListTests`: the loading text, then the list after `Release()`; the empty message for an
  empty store. Use `WaitForAssertion` or `WaitForState`.
- TODO-04: `TaskListTests`: the checkbox, delete confirmed and declined (`JSInterop.Setup<bool>`), and adding
  a task (`JSInterop.SetupVoid` and `VerifyInvoke`).
- TODO-05: `TaskListTests`: `HideCompleted` from a cascading value, the lead-only button, and what it clears.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Refactor check. In `TaskList.razor`, change the class `"task done text-muted"` to
  `"task is-complete"`. Nothing a user can do changed, so no test should turn red. If one does, it selects by
  a CSS class.
- TODO-07 (optional): Mutation check. Change the components on purpose, one change at a time: make
  `OnInitializedAsync` await the store without assigning `tasks` (the list never leaves "Loading"), delete
  without looking at the answer of `confirm`, remove `Roles="lead"` from the `AuthorizeView`, remove
  `<DataAnnotationsValidator />`. Each change should turn at least one of your tests red.
