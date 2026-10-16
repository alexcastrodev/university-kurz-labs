# Fields, Constants and Initialization Order (Solution)

Official solution for the `fields-constants-and-initialization-order` lab.

## What changed from the starter

- The field initializer of `Derived` runs before the base constructor, so the call from `Base` sees `name=initialized;late=null`; the field assigned in the constructor body is still `null` at that moment.
- `const` is emitted as a literal field and inlined into callers; `static readonly` is a real field read at run time, so shipping only a new library updates it.
- An explicit static constructor removes `beforefieldinit`, which is what guarantees initialization right before first access.
- `required` is a call-site check: reflection sees `RequiredMemberAttribute`, and `[SetsRequiredMembers]` is an unchecked promise by the constructor.
- Bonus: each closed generic type has its own statics (`1 2 1`); only the full constructor of `Settings` assigns and validates.

## Running

```bash
dotnet test
```
