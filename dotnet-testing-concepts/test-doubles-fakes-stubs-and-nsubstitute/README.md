# Test Doubles in .NET: Fakes, Stubs and NSubstitute

## Goal

A notification service is already fully implemented: when a shipment leaves, it tells the customer by
email or SMS, falls back to email when the SMS gateway refuses, respects opt-outs, and never notifies
twice for the same shipment. Your job is to write the MSTest tests that prove it, with a hand-written
fake for one port and NSubstitute substitutes for the others, and to learn where a test double makes a
test pass for the wrong reason.

## Prerequisites

- MSTest Fundamentals: Lifecycle and TestContext (the concept and its lab)
- The Java concepts on test doubles and mock fragility help, but are not required
- .NET 10 SDK

## Task

Read `src/Notifications` first (`Ports.cs`, `NotificationService.cs`). Then open the test project.
Every test method is already declared with a hint, and fails with `TODO-NN: write this test` until you
write it. The fake log and the `Known` helper are shells for now.

Three traps are waiting, and each one gives you a green test that proves nothing:

- a substitute you did not configure still answers, with a default value (`false`, `null`, `""`);
- `Received` and `DidNotReceive` check a call, not the outcome the customer sees;
- a stub that answers for any argument hides a service that asks for the wrong thing.

## Instructions

Complete the following TODOs in `tests/Notifications.Tests`:

- TODO-00: `TestDoubles.cs`: write `InMemoryNotificationLog`, a hand-written fake of the log port.
- TODO-01: `Known(contact)`: configure the directory substitute for the exact customer id.
- TODO-02: an email customer gets the shipping email at their address.
- TODO-03: an SMS customer gets an SMS when the gateway accepts it (configure the gateway first).
- TODO-04: an SMS customer falls back to email when the gateway refuses.
- TODO-05: opted out, unknown customer, and no reachable channel: assert the outcome, not only the silence.
- TODO-06: the same shipment notifies the customer once (the fake log at work).
- TODO-07: a failed email is not recorded, so the retry sends it.

Run the tests until they all pass. The build also runs `NSubstitute.Analyzers.CSharp`: read its
warnings, they point at misuses of the library.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): Test `HttpSmsSender` with the hand-written `StubHttpHandler`. Remove the
  `[Ignore]` attributes first.
- TODO-09 (optional): Write a learning test that shows what NSubstitute does with the sealed
  `HttpSmsSender`, and think about what it means for the design of the service.
- TODO-10 (optional): Mutation check. Change the production code on purpose, one change at a time, and
  see whether your suite notices: query the directory with `shipment.OrderId`, treat every SMS as
  delivered, remove the opt-out check, write the log entry before delivering. Then change `Known` to
  `Arg.Any<Guid>()` and repeat the first mutation. A change that leaves everything green means a
  missing or a weak test.
