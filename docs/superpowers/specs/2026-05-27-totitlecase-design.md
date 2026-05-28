# Design: `ToTitleCase` String Extension

**Date:** 2026-05-27
**Branch:** new-extensions

## Summary

Add a `ToTitleCase` extension method to `StringExtensions.cs` that converts a string to simple title case: lowercase the entire input, collapse whitespace, then capitalize the first letter of each space-delimited word.

## Method Signature

```csharp
public static string ToTitleCase(this string input)
```

Namespace: `CSharpHelperExtensions.Strings`
File: `src/CSharpHelperExtensions/StringExtensions.cs`

## Behaviour

1. `null`, empty, or whitespace-only input → return `string.Empty`
2. Lowercase the entire string
3. Collapse whitespace: trim leading/trailing, collapse internal whitespace runs to a single space (reuse existing `CollapseRegex`)
4. Split on `' '`, uppercase the first character of each segment, rejoin with `" "`

## Examples

| Input | Output |
|-------|--------|
| `"hello world"` | `"Hello World"` |
| `"  hELLO   wORLD  "` | `"Hello World"` |
| `"it's a test"` | `"It's A Test"` |
| `"SINGLE"` | `"Single"` |
| `null` | `""` |
| `""` | `""` |
| `"   "` | `""` |

## Edge Cases

- **Single word:** `"hello"` → `"Hello"`
- **Word starting with non-letter:** e.g. `"(hello)"` — the `(` stays as-is; `h` is not capitalized. Acceptable for simple title case.
- **Punctuation between words** is not treated as a word boundary; only spaces are.

## Implementation Notes

- Reuses the existing `CollapseRegex` static field already defined in the class.
- No new dependencies.

## Tests

One `[Fact]` in `StringExtensionTest.cs` named `Verify_ToTitleCase_CapitalizesFirstLetterOfEachWord` covering:
- Normal two-word string
- Mixed-case input
- Extra surrounding and internal whitespace
- Single word
- `null` input
- Empty string input
