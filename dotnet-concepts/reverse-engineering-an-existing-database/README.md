# Reverse Engineering an Existing Database

## Goal

Run the EF Core 10 scaffolder (the engine behind `dotnet ef dbcontext scaffold`) against
a small legacy SQLite database and read what it produces: names, keys, defaults, a join
table, a view, a table without a primary key and a column whose type the scaffolder had
to guess. Then change the output safely, with a hand-written `partial` class that
survives a re-scaffold. The scaffolder runs inside the test process, so no `dotnet-ef`
tool, server, Docker or network is needed. The tests assert on the generated source
text (substrings), and compile it with Roslyn to prove your hand-written code fits.

## Prerequisites

- C# and EF Core basics (DbContext, Fluent API, data annotations)
- Basic SQL (primary and foreign keys, views)
- .NET 10 SDK

## Task

`LegacySchema` creates a file-based SQLite database with upper-case snake names:
`CUSTOMER`, `ORDER_HDR`, `ORDER_LINE` (composite key), `TAG`, `ORDER_TAG` (a plain join
table), `AUDIT_LOG` (no primary key) and the view `V_ORDER_TOTALS` (its `SUM` column has
no declared type). Every test creates its own copy in a unique temp directory and deletes
it afterwards. `Scaffolding.CreateScaffolder` already builds the scaffolder the way the
CLI does. The rest are stubs that throw `NotImplementedException`: you translate command
line flags into scaffolder options, then read and extend the result.

## Instructions

Complete the following TODOs:

- TODO-00: `Scaffolding.ScaffoldModel`: call `IReverseEngineerScaffolder.ScaffoldModel` with the settings (tables, `UseDatabaseNames`, `NoPluralize`, `UseDataAnnotations`, context name, namespaces) and suppress the `OnConfiguring` method and the connection string warning. Real-world counterpart: `dotnet ef dbcontext scaffold "Data Source=legacy.db" Microsoft.EntityFrameworkCore.Sqlite --context LegacyDb --namespace Legacy.Models --no-onconfiguring`.
- TODO-01: `Presets.DatabaseNames`: keep names as the database has them. Counterpart: `--use-database-names --no-pluralize`.
- TODO-02: `Presets.DataAnnotations`: attributes (`[Table]`, `[Column]`, `[Key]`, `[Keyless]`) instead of Fluent calls. Counterpart: `--data-annotations`. Notice what stays in `OnModelCreating` anyway.
- TODO-03: `Presets.OnlyTables`: scaffold only the named tables or views. Counterpart: `--table CUSTOMER --table ORDER_HDR`.
- TODO-04: `Findings.KeylessEntities` and `Findings.ByteArrayProperties`: read the generated text and report the entities the scaffolder made keyless (`HasNoKey()`) and the properties it typed as `byte[]` because the view column had no declared type.
- TODO-05: `PartialExtension.Source`: a hand-written second file with `partial class LegacyDb` (`OnModelCreatingPartial`, cascade delete for `OrderLine`) and `partial class Customer` (a `Display` property). It must compile against the scaffolded code and survive a re-scaffold, because the generated files are never edited.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): `Scaffolding.ScaffoldToDirectory`: write the files to disk. Without `force` the scaffolder must refuse to overwrite existing files. Counterpart: `--output-dir Models` and `--force`.
- TODO-07 (optional): `PartialExtension.SourceWithGeneratedKeys`: fix the `ValueGeneratedNever()` the scaffolder guessed for `INTEGER PRIMARY KEY` columns, again from the partial class, keeping the cascade.
