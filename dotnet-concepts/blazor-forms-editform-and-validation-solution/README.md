# Blazor Forms: EditForm and Validation (Solution)

## Goal

See a Blazor form as three separate pieces (the model, the `EditContext` and the
components that read them) and exercise it without a browser: you will render
the components with bUnit, declare rules on the model, wire `EditForm` to a
validator, add a custom rule, surface a server-side error and style fields from
the validation state.

## Prerequisites

- Basic Razor components (`@code`, `[Parameter]`, `EventCallback`)
- Data annotations (`[Required]`, `[Range]`, `IValidatableObject`)
- .NET 10 SDK (the tests use bUnit 2.11.3, restored from nuget.org)

## Task

The `BlazorFormsLab` Razor class library has two forms, already complete in this
solution. `SignupForm` edits a `Signup` model (name, age, email, plan, terms) and
`UsernameForm` edits a user name that the "server" may reject as taken. Every
TODO below is implemented. Read the code and the tests together, and run them.
The tests render the components with bUnit and fill inputs with `Change(...)`,
like a browser `change` event. No browser, JavaScript or SignalR is involved.

## Instructions

The following TODOs are implemented (each one was marked in the starter):

- TODO-00: Declare the rules on `Signup`: `Name` required with 3 to 10
  characters, `Age` from 18 to 120, `Email` must look like an email, and an
  `IValidatableObject` rule `admin is reserved.` for member `Name` (ignore case).
- TODO-01: In `SignupForm.razor`, add `DataAnnotationsValidator` and connect
  `OnValidSubmit` and `OnInvalidSubmit`. Check what the tests say about when the
  `IValidatableObject` rule runs (field change or submit?) and why they accept the
  terms before expecting it. Also read the `OnSubmit` test: why can `OnSubmit` not
  be combined with the other two callbacks?
- TODO-02: Implement `NoBannedWordsAttribute` (`root` and `system` are banned,
  ignore case) so the message `root is banned.` is attached to the member.
- TODO-03: In `UsernameForm.razor`, when `IsTaken(name)` is true add
  `{name} is already taken.` to a `ValidationMessageStore`, and clear the edited
  field's messages when it changes. Why must you clear them yourself?
- TODO-04: Implement `LabFieldCssClassProvider` (`pristine`, `touched ok`,
  `touched bad`) and register it on the `EditContext` of `SignupForm`.

All tests pass. Bonus tests carry `[Trait("Category", "Bonus")]`.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

Also implemented here:

- TODO-05 (optional): Show `<p class="dirty">Unsaved changes</p>` while
  `ctx.IsModified()` is true and call `ctx.MarkAsUnmodified()` after a valid save.
- TODO-06 (optional): Make the terms mandatory with
  `[Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms.")]`
  on `Signup.Terms`. Why does the bool need this trick instead of `[Required]`?
