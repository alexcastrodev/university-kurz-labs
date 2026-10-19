# Blazor Component Lifecycle and Parameters

## Goal

Make the Blazor component lifecycle visible and correct: the order of the hooks, the two renders of an async `OnInitializedAsync`, parameters that report back with `EventCallback<T>`, reloading only when a parameter really changed, cascading values, and code that wakes up outside the renderer's dispatcher.

## Prerequisites

- C# async/await and `TaskCompletionSource`
- Razor components (`[Parameter]`, `@code`)
- .NET 10 SDK

## Task

The `BlazorComponentLifecycleAndParameters` Razor class library holds small components, each with a deliberate gap. The tests use [bUnit](https://bunit.dev) (version 2.11.3) to render them in memory: no browser, no JavaScript, no SignalR. Async work is driven by a `TaskCompletionSource` the test completes, never by a delay.

## Instructions

Complete the following TODOs:

- TODO-00: `LifecycleProbe`: log every hook in the order the framework runs it, including the real `firstRender` value.
- TODO-01: `AsyncLoader`: await the data source in `OnInitializedAsync` and see the "Loading..." render before the final one.
- TODO-02: `SelectableCard`: mark `Name` as `[EditorRequired]` and invoke the `EventCallback<string>` from the button, so `CardPicker` refreshes.
- TODO-03: `ProductView`: reload only when `ProductId` changed, even though the `List<int>` parameter makes `OnParametersSet` run on every parent render.
- TODO-04: `ThemedLabel`: read the theme from a `[CascadingParameter]`.
- TODO-05: `Ticker`: handle an event raised on another thread with `InvokeAsync`, and unsubscribe in `Dispose`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): `Badge`: capture unmatched attributes with `CaptureUnmatchedValues` and splat them with `@attributes`.
- TODO-07 (optional): `CounterBox`: override `ShouldRender` to skip the render when only `Other` changed, and check that `OnParametersSet` still runs.
