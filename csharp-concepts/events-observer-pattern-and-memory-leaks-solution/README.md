# Events, the Observer Pattern and Memory Leaks

## Goal

Raise events safely, return subscriptions as `IDisposable`, and fix the classic leak where a long-lived publisher keeps dead subscribers alive.

## Prerequisites

- Delegates and multicast delegates
- `IDisposable` and `using`
- .NET 10 SDK

## Task

`TickSource` (a long-lived publisher) and the event args are ready. `Account`, `Widget`, `PriceFeed`, `WidgetFactory` and `LambdaListener` are stubbed. You will raise events with the standard pattern, make subscribers unsubscribe, and prove with a `WeakReference` that the leak is gone.

## Instructions

Complete the following TODOs:

- TODO-00: `Account.Deposit` and `OnBalanceChanged`: the standard raise pattern with `?.Invoke`.
- TODO-01: `Widget`: subscribe in the constructor and unsubscribe in `Dispose` with the same method.
- TODO-02: `PriceFeed.Subscribe`: return the subscription as an `IDisposable`.
- TODO-03: `WidgetFactory.Create`: build a widget in a `NoInlining` method and return only a `WeakReference`, so the test can check whether the GC may collect it.
- TODO-04: `LambdaListener`: a lambda written twice is two delegates, so `-=` needs the same delegate instance.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): `Button.Click` as a custom event with `add`/`remove` accessors that store handlers in a list under a lock (the shape of a C# 14 `partial event` implementation).
- TODO-06 (optional): `Attach` with C# 14 null-conditional assignment: `maybe?.BalanceChanged += handler;`.
