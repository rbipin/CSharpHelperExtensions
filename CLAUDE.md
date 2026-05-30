# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the entire solution
dotnet build

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run a single test by name (partial match works)
dotnet test --filter "FullyQualifiedName~<TestMethodName>"
# Example:
dotnet test --filter "FullyQualifiedName~Verify_In_Exists"

# Run all tests in a specific class (ClassName~ filter does NOT work; use FullyQualifiedName~)
dotnet test --filter "FullyQualifiedName~EnumerableExtensionTest"

# Build only the library project
dotnet build src/CSharpHelperExtensions/CSharpHelperExtensions.csproj

# Pack as NuGet package
dotnet pack src/CSharpHelperExtensions/CSharpHelperExtensions.csproj

# Restore local .NET tools (CSharpier formatter)
dotnet tool restore

# Build using the solution file explicitly
dotnet build CSharpHelperExtensions.slnx

# Test using the solution file explicitly
dotnet test CSharpHelperExtensions.slnx
```

## Architecture

This is a two-project solution:

- **`src/CSharpHelperExtensions/`** — `net10.0` class library. The publishable NuGet package (`CSharpHelperExtensions` v2.0.0). Depends only on `Newtonsoft.Json`.
- **`src/CSharpHelperExtensions.Test/`** — xUnit test project (`net10.0`) using Shouldly for assertions.

Root-level config files: `CSharpHelperExtensions.slnx` (solution entry point), `global.json` (pins SDK to 10.0.x), `.editorconfig` (C# code style + formatting rules), `.config/dotnet-tools.json` (CSharpier formatter).

### Extension method namespaces

The library splits extensions across three namespaces — callers must import the right one:

| File | Namespace | Key types |
|------|-----------|-----------|
| `ValueExtensions.cs` | `CSharpHelperExtensions.Values` | `In`, `IsBetween`, `ToJson` |
| `EnumerableExtensions.cs` | `CSharpHelperExtensions.Enumerable` | `IsNullOrEmpty<T>`, `HasAny`, `None`, `CleanNullOrEmptyItems`, `WhereNotNull`, `ContainsOnly`, `AreEqual`, `ForEach`, `Reduce`, `SelectAsync`, `WhenAllList`, `Partition`, `Batch`, `MinByOrDefault`, `MaxByOrDefault`, `ToDictionarySafe`, `AddIf`, `AddRangeIf`, `ConcatIf`, `IsSingle`, `IndexOf`, `Yield`, `WithIndex`, `JoinAsString`, `AsReadOnlyList`, `ToHashSetSafe`, `OrEmpty` |
| `StringExtensions.cs` | `CSharpHelperExtensions.Strings` | `IsNullOrEmpty`, `HasValue`, `OrEmpty`, `OrDefault`, `Truncate`, `Reverse`, `TrimToLower`, `TrimToUpper`, `ToTitleCase`, `ToSlug`, `MaskStart`, `EqualsIgnoreCase`, `ContainsIgnoreCase`, `StartsWithIgnoreCase`, `EndsWithIgnoreCase`, `EnsurePrefix`, `EnsureSuffix`, `TrimPrefix`, `TrimSuffix`, `SplitNonEmpty`, `JoinWith`, `ReplaceMany`, `RemoveWhitespace`, `CollapseWhitespace`, `RemoveDiacritics`, `IsNumeric`, `IsAlpha`, `IsAlphaNumeric`, `ToNullable<T>`, `ToIntOrNull`, `ToDecimalOrNull`, `ToDateTimeOrNull`, `ToGuidOrNull`, `ToBoolOrNull`, `Base64Encode`, `Base64Decode`, `ToBase64Url`, `FromBase64Url`, `ToUtf8Bytes`, `ToUtf8Stream` |
| `DictionaryExtensions.cs` | `CSharpHelperExtensions.Dictionaries` | `GetOrAdd`, `Merge`, `AddRange`, `RemoveWhere`, `AsReadOnly` |

`IsNullOrEmpty` exists in **both** `StringExtensions` (for `string`, namespace `CSharpHelperExtensions.Strings`) and `EnumerableExtensions` (for `IEnumerable<T>`, namespace `CSharpHelperExtensions.Enumerable`). Be careful about which namespace is imported.

### `BetweenComparison` enum

Defined in `ValueExtensions.cs` (namespace `CSharpHelperExtensions.Values`), controls how `IsBetween` handles bounds:
- `None` (default) — inclusive on both ends
- `ExcludeBoth` — exclusive on both ends
- `ExcludeLower` — excludes lower bound, includes upper
- `ExcludeUpper` — includes lower bound, excludes upper

### `Compare` enum

Defined in `EnumerableExtensions.cs`, used by `AreEqual`:
- `NoOrder` (default) — order-insensitive equality
- `InOrder` — positional equality
