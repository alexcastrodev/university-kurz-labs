# Nullable Reference Types and Flow Analysis (Solution)

Official solution for the `nullable-reference-types-and-flow-analysis` lab. The solution
project sets `WarningsAsErrors` to `nullable`, so any nullable warning breaks the build.

## What changed from the starter

- `User.Nickname` is `string?` and `Email` stays `string` with `required`. `DisplayName` checks
  `IsNullOrWhiteSpace`, which carries `[NotNullWhen(false)]`, so the compiler knows the
  nickname is not null in the other branch with no `!`.
- `FindUser` returns `User?`: the signature now tells callers the lookup can miss.
- `TryGetUser` marks the `out` parameter with `[NotNullWhen(true)]`. When the method returns
  `true`, the caller can use `user` without a null check, which is the same trick `Dictionary.TryGetValue` uses.
- `Trim` is annotated with `[return: NotNullIfNotNull(nameof(value))]`: a non-null argument
  gives a non-null result, a nullable argument gives a nullable result.
- `Connection.Initialize` has `[MemberNotNull(nameof(_endpoint))]`, so the constructor can
  delegate field initialization to a helper without warning CS8618.
- `Require` keeps the guard clause: nullable annotations are not enforced at run time, and a
  `null!` or code compiled without the feature can still pass `null`.
- `Compare` relies on lifted operators: `==` on two nulls is `true`, but `<` and `>` with a null
  operand are `false`, hence the `unordered` result.
- `FirstOrNull<T> where T : struct` makes `T?` mean `Nullable<T>`. Without the constraint, `T?`
  is only an annotation and the method would return `default` for an empty sequence.

## Bonus solutions

- `TryFirst` uses `[MaybeNullWhen(false)]` on an unconstrained generic `out T`: on failure the
  value is `default`, which may be null for reference types.
- `CountBy<TKey> where TKey : notnull` matches the constraint on `Dictionary<TKey, TValue>`,
  so no warning appears when a nullable key type is not allowed.

## Why the tests use reflection

The nullable annotations and the flow-analysis attributes are stored in metadata, so the tests
read them with `NullabilityInfoContext` and `GetCustomAttribute`. That is simpler than running
the compiler with Roslyn, and it fails exactly when an annotation is missing.
