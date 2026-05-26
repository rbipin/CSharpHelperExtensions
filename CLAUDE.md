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

# Run all tests in a specific class
dotnet test --filter "ClassName~EnumerableExtensionTest"

# Build only the library project
dotnet build CSharpHelperExtensions/CSharpHelperExtensions.csproj

# Pack as NuGet package
dotnet pack CSharpHelperExtensions/CSharpHelperExtensions.csproj

# Build using the solution file explicitly
dotnet build CSharpHelperExtensions.slnx

# Test using the solution file explicitly
dotnet test CSharpHelperExtensions.slnx
```

## Architecture

This is a two-project solution:

- **`CSharpHelperExtensions/`** — `netstandard2.1` class library. The publishable NuGet package (`CSharpHelperExtensions` v1.0.1). Depends only on `Newtonsoft.Json`.
- **`CSharpHelperExtensions.Test/`** — xUnit test project (`net6.0`) using FluentAssertions.

### Extension method namespaces

The library splits extensions across three namespaces — callers must import the right one:

| File | Namespace | Key types |
|------|-----------|-----------|
| `GenericExtensions.cs` | `CSharpHelperExtensions` | `In`, `IsNullOrEmpty` (string), `IsBetween`, `ToJson` |
| `EnumerableExtensions.cs` | `CSharpHelperExtensions.Enumerable` | `IsNullOrEmpty<T>`, `CleanNullOrEmptyItems`, `ContainsOnly`, `AreEqual`, `ForEach`, `Reduce` |
| `StringExtensions.cs` | `CSharpHelperExtensions.Strings` | `ToNullable<T>` |

`IsNullOrEmpty` exists in **both** `GenericExtensions` (for `string`) and `EnumerableExtensions` (for `IEnumerable<T>`). Be careful about which namespace is imported.

### `BetweenComparison` enum

Defined in `GenericExtensions.cs`, controls how `IsBetween` handles bounds:
- `None` (default) — inclusive on both ends
- `ExcludeBoth` — exclusive on both ends
- `ExcludeLower` — excludes lower bound, includes upper
- `ExcludeUpper` — includes lower bound, excludes upper

### `Compare` enum

Defined in `EnumerableExtensions.cs`, used by `AreEqual`:
- `NoOrder` (default) — order-insensitive equality
- `InOrder` — positional equality
