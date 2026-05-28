# ToTitleCase Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a `ToTitleCase` extension method to `StringExtensions` that lowercases the input, collapses whitespace, and capitalizes the first letter of each space-delimited word.

**Architecture:** Single method added to the existing `StringExtensions` static class. Reuses the already-compiled `CollapseRegex` field in the same file. Follows the null-safe, `string.Empty`-on-null pattern used throughout the class.

**Tech Stack:** C# 13 / net10.0, xUnit, FluentAssertions

---

### Task 1: Write the failing test

**Files:**
- Modify: `src/CSharpHelperExtensions.Test/StringExtensionTest.cs`

- [ ] **Step 1: Add the failing test**

Open `src/CSharpHelperExtensions.Test/StringExtensionTest.cs` and append the following fact inside the `StringExtensionTest` class (after the last closing `}`  of the last `[Fact]`, before the class `}`):

```csharp
[Fact]
public void Verify_ToTitleCase_CapitalizesFirstLetterOfEachWord()
{
    "hello world".ToTitleCase().Should().Be("Hello World");
    "  hELLO   wORLD  ".ToTitleCase().Should().Be("Hello World");
    "it's a test".ToTitleCase().Should().Be("It's A Test");
    "SINGLE".ToTitleCase().Should().Be("Single");
    ((string)null).ToTitleCase().Should().Be("");
    "".ToTitleCase().Should().Be("");
    "   ".ToTitleCase().Should().Be("");
}
```

- [ ] **Step 2: Run the test to confirm it fails**

```bash
dotnet test --filter "FullyQualifiedName~Verify_ToTitleCase"
```

Expected: build error — `'string' does not contain a definition for 'ToTitleCase'`

---

### Task 2: Implement `ToTitleCase`

**Files:**
- Modify: `src/CSharpHelperExtensions/StringExtensions.cs`

- [ ] **Step 1: Add the method**

Open `src/CSharpHelperExtensions/StringExtensions.cs`. Append the following method inside the `StringExtensions` class, after the closing brace of `ToSlug` and before the final class `}`:

```csharp
/// <summary>
/// Converts <paramref name="input"/> to simple title case: lowercases the string,
/// collapses whitespace, then capitalizes the first letter of each word.
/// </summary>
/// <param name="input">The string to convert. Returns <see cref="string.Empty"/> when <see langword="null"/> or whitespace.</param>
/// <returns>The title-cased string, e.g. <c>"hello world"</c> → <c>"Hello World"</c>.</returns>
public static string ToTitleCase(this string input)
{
    if (input.IsNullOrEmpty()) return string.Empty;
    var collapsed = CollapseRegex.Replace(input.Trim().ToLowerInvariant(), " ");
    var words = collapsed.Split(' ');
    for (var i = 0; i < words.Length; i++)
    {
        if (words[i].Length > 0)
            words[i] = char.ToUpperInvariant(words[i][0]) + words[i][1..];
    }
    return string.Join(" ", words);
}
```

- [ ] **Step 2: Run the test to confirm it passes**

```bash
dotnet test --filter "FullyQualifiedName~Verify_ToTitleCase"
```

Expected output:
```
Passed! - Failed: 0, Passed: 1, Skipped: 0
```

- [ ] **Step 3: Run the full test suite to check for regressions**

```bash
dotnet test
```

Expected: all tests pass, no failures.

- [ ] **Step 4: Commit**

```bash
git add src/CSharpHelperExtensions/StringExtensions.cs src/CSharpHelperExtensions.Test/StringExtensionTest.cs
git commit -m "feat: add ToTitleCase string extension"
```
