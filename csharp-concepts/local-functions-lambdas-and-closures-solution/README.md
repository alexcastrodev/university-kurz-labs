# Local Functions, Lambdas and Closures (Solution)

Official solution for the `local-functions-lambdas-and-closures` lab.

## What changed from the starter

- A captured local lives on a compiler-generated closure object, so `count` survives after `MakeCounter` returns and each call gets a fresh one.
- Inside the `for` body the `copy` local is new on every iteration; capturing `i` directly would make every reader see the final value.
- An iterator method runs nothing until it is enumerated, so the null check would be late. Validating in the outer method and iterating in a `static` local function throws immediately; `static` guarantees the local function captures nothing.
- `int.Parse` converts to `Func<string, int>` by overload resolution on the method group.
- The returned lambda captures `cache` and `compute`; the cache lives as long as the delegate.
- Bonus: `(string person, string greeting = "Hello") => ...` has a default value; `(value, out result) => ...` infers the types from `TryParser<int>`; `static value => value + 1` does not compile if it touches an outer variable.

## Running

```bash
dotnet test
```
