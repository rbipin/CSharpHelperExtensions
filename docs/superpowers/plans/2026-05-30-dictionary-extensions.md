# DictionaryExtensions Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a `DictionaryExtensions` static class with five fluent extension methods for `IDictionary<TKey, TValue>` in a new `CSharpHelperExtensions.Dictionaries` namespace.

**Architecture:** Two new files following the existing pattern: one implementation file alongside `EnumerableExtensions.cs` and `StringExtensions.cs`, and one test file alongside their test counterparts. TDD throughout — write the failing test first, implement minimally, confirm green, commit.

**Tech Stack:** C# / .NET 10, xUnit, Shouldly, `System.Collections.ObjectModel.ReadOnlyDictionary`

---

## File Map

| Action | Path |
|---|---|
| Create | `src/CSharpHelperExtensions/DictionaryExtensions.cs` |
| Create | `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs` |

---

### Task 1: Scaffold the files

**Files:**
- Create: `src/CSharpHelperExtensions/DictionaryExtensions.cs`
- Create: `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs`

- [ ] **Step 1: Create the implementation stub**

Create `src/CSharpHelperExtensions/DictionaryExtensions.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable enable
namespace CSharpHelperExtensions.Dictionaries;

public static class DictionaryExtensions
{
}
```

- [ ] **Step 2: Create the test file stub**

Create `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs`:

```csharp
using System.Collections.Generic;
using CSharpHelperExtensions.Dictionaries;
using Shouldly;
using Xunit;

namespace CSharpHelperExtensions.Test;

public class DictionaryExtensionTest
{
}
```

- [ ] **Step 3: Verify the solution builds**

Run:
```bash
dotnet build
```
Expected: Build succeeded with 0 errors.

- [ ] **Step 4: Commit**

```bash
git add src/CSharpHelperExtensions/DictionaryExtensions.cs src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs
git commit -m "scaffold DictionaryExtensions and test files"
```

---

### Task 2: `GetOrAdd`

**Files:**
- Modify: `src/CSharpHelperExtensions/DictionaryExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `DictionaryExtensionTest`:

```csharp
[Fact]
public void GetOrAdd_KeyExists_ReturnsExistingValue()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var factoryCalled = false;
    var result = dict.GetOrAdd("a", _ => { factoryCalled = true; return 99; });
    result.ShouldBe(1);
    factoryCalled.ShouldBeFalse();
}

[Fact]
public void GetOrAdd_KeyMissing_InvokesFactoryAddsAndReturnsValue()
{
    var dict = new Dictionary<string, int>();
    var result = dict.GetOrAdd("b", key => 42);
    result.ShouldBe(42);
    dict["b"].ShouldBe(42);
}

[Fact]
public void GetOrAdd_NullFactory_Throws()
{
    var dict = new Dictionary<string, int>();
    Should.Throw<ArgumentNullException>(() => dict.GetOrAdd("a", null!));
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:
```bash
dotnet test --filter "FullyQualifiedName~GetOrAdd"
```
Expected: FAIL — `GetOrAdd` not found.

- [ ] **Step 3: Implement `GetOrAdd`**

Add to `DictionaryExtensions`:

```csharp
public static TValue GetOrAdd<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    TKey key,
    Func<TKey, TValue> factory)
{
    ArgumentNullException.ThrowIfNull(key);
    ArgumentNullException.ThrowIfNull(factory);

    if (dict.TryGetValue(key, out var existing))
        return existing;

    var value = factory(key);
    dict[key] = value;
    return value;
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:
```bash
dotnet test --filter "FullyQualifiedName~GetOrAdd"
```
Expected: 3 tests pass.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/DictionaryExtensions.cs src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs
git commit -m "add GetOrAdd to DictionaryExtensions"
```

---

### Task 3: `Merge`

**Files:**
- Modify: `src/CSharpHelperExtensions/DictionaryExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `DictionaryExtensionTest`:

```csharp
[Fact]
public void Merge_NoOverlap_AddsAllEntries()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var other = new Dictionary<string, int> { ["b"] = 2 };
    var result = dict.Merge(other);
    result.ShouldBeSameAs(dict);
    dict.Count.ShouldBe(2);
    dict["b"].ShouldBe(2);
}

[Fact]
public void Merge_DuplicateKey_OverwriteFalse_KeepsExisting()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var other = new Dictionary<string, int> { ["a"] = 99 };
    dict.Merge(other, overwrite: false);
    dict["a"].ShouldBe(1);
}

[Fact]
public void Merge_DuplicateKey_OverwriteTrue_ReplacesExisting()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var other = new Dictionary<string, int> { ["a"] = 99 };
    dict.Merge(other, overwrite: true);
    dict["a"].ShouldBe(99);
}

[Fact]
public void Merge_NullOther_NoOp()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var result = dict.Merge(null!);
    result.ShouldBeSameAs(dict);
    dict.Count.ShouldBe(1);
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:
```bash
dotnet test --filter "FullyQualifiedName~Merge"
```
Expected: FAIL — `Merge` not found.

- [ ] **Step 3: Implement `Merge`**

Add to `DictionaryExtensions`:

```csharp
public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    IDictionary<TKey, TValue>? other,
    bool overwrite = false)
{
    if (other is null)
        return dict;

    foreach (var pair in other)
    {
        if (overwrite || !dict.ContainsKey(pair.Key))
            dict[pair.Key] = pair.Value;
    }

    return dict;
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:
```bash
dotnet test --filter "FullyQualifiedName~Merge"
```
Expected: 4 tests pass.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/DictionaryExtensions.cs src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs
git commit -m "add Merge to DictionaryExtensions"
```

---

### Task 4: `AddRange`

**Files:**
- Modify: `src/CSharpHelperExtensions/DictionaryExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `DictionaryExtensionTest`:

```csharp
[Fact]
public void AddRange_NoOverlap_AddsAllPairs()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var pairs = new List<KeyValuePair<string, int>> { new("b", 2), new("c", 3) };
    var result = dict.AddRange(pairs);
    result.ShouldBeSameAs(dict);
    dict.Count.ShouldBe(3);
}

[Fact]
public void AddRange_DuplicateKey_OverwriteFalse_KeepsExisting()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var pairs = new List<KeyValuePair<string, int>> { new("a", 99) };
    dict.AddRange(pairs, overwrite: false);
    dict["a"].ShouldBe(1);
}

[Fact]
public void AddRange_DuplicateKey_OverwriteTrue_ReplacesExisting()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var pairs = new List<KeyValuePair<string, int>> { new("a", 99) };
    dict.AddRange(pairs, overwrite: true);
    dict["a"].ShouldBe(99);
}

[Fact]
public void AddRange_NullPairs_NoOp()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var result = dict.AddRange(null!);
    result.ShouldBeSameAs(dict);
    dict.Count.ShouldBe(1);
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:
```bash
dotnet test --filter "FullyQualifiedName~AddRange"
```
Expected: FAIL — `AddRange` not found.

- [ ] **Step 3: Implement `AddRange`**

Add to `DictionaryExtensions`:

```csharp
public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    IEnumerable<KeyValuePair<TKey, TValue>>? pairs,
    bool overwrite = false)
{
    if (pairs is null)
        return dict;

    foreach (var pair in pairs)
    {
        if (overwrite || !dict.ContainsKey(pair.Key))
            dict[pair.Key] = pair.Value;
    }

    return dict;
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:
```bash
dotnet test --filter "FullyQualifiedName~AddRange"
```
Expected: 4 tests pass.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/DictionaryExtensions.cs src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs
git commit -m "add AddRange to DictionaryExtensions"
```

---

### Task 5: `RemoveWhere`

**Files:**
- Modify: `src/CSharpHelperExtensions/DictionaryExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `DictionaryExtensionTest`:

```csharp
[Fact]
public void RemoveWhere_RemovesMatchingKeys()
{
    var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    var result = dict.RemoveWhere(k => k == "a" || k == "c");
    result.ShouldBeSameAs(dict);
    dict.Count.ShouldBe(1);
    dict.ContainsKey("b").ShouldBeTrue();
}

[Fact]
public void RemoveWhere_NoMatch_LeavesAllEntries()
{
    var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
    dict.RemoveWhere(k => k == "z");
    dict.Count.ShouldBe(2);
}

[Fact]
public void RemoveWhere_EmptyDict_NoOp()
{
    var dict = new Dictionary<string, int>();
    dict.RemoveWhere(k => true);
    dict.Count.ShouldBe(0);
}

[Fact]
public void RemoveWhere_NullPredicate_Throws()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    Should.Throw<ArgumentNullException>(() => dict.RemoveWhere(null!));
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:
```bash
dotnet test --filter "FullyQualifiedName~RemoveWhere"
```
Expected: FAIL — `RemoveWhere` not found.

- [ ] **Step 3: Implement `RemoveWhere`**

Add to `DictionaryExtensions`:

```csharp
public static IDictionary<TKey, TValue> RemoveWhere<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    Func<TKey, bool> predicate)
{
    ArgumentNullException.ThrowIfNull(predicate);

    var keysToRemove = new List<TKey>();
    foreach (var key in dict.Keys)
    {
        if (predicate(key))
            keysToRemove.Add(key);
    }

    foreach (var key in keysToRemove)
        dict.Remove(key);

    return dict;
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run:
```bash
dotnet test --filter "FullyQualifiedName~RemoveWhere"
```
Expected: 4 tests pass.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/DictionaryExtensions.cs src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs
git commit -m "add RemoveWhere to DictionaryExtensions"
```

---

### Task 6: `AsReadOnly`

**Files:**
- Modify: `src/CSharpHelperExtensions/DictionaryExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `DictionaryExtensionTest`:

```csharp
[Fact]
public void AsReadOnly_ReturnsIReadOnlyDictionary()
{
    var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
    IReadOnlyDictionary<string, int> readOnly = dict.AsReadOnly();
    readOnly.Count.ShouldBe(2);
    readOnly["a"].ShouldBe(1);
}

[Fact]
public void AsReadOnly_ReflectsMutationsToUnderlying()
{
    var dict = new Dictionary<string, int> { ["a"] = 1 };
    var readOnly = dict.AsReadOnly();
    dict["b"] = 2;
    readOnly.Count.ShouldBe(2);
    readOnly["b"].ShouldBe(2);
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:
```bash
dotnet test --filter "FullyQualifiedName~AsReadOnly"
```
Expected: FAIL — `AsReadOnly` not found.

- [ ] **Step 3: Implement `AsReadOnly`**

Add to `DictionaryExtensions`:

```csharp
public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
    this IDictionary<TKey, TValue> dict) =>
    new ReadOnlyDictionary<TKey, TValue>(dict);
```

- [ ] **Step 4: Run tests to verify they pass**

Run:
```bash
dotnet test --filter "FullyQualifiedName~AsReadOnly"
```
Expected: 2 tests pass.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/DictionaryExtensions.cs src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs
git commit -m "add AsReadOnly to DictionaryExtensions"
```

---

### Task 7: Full test run and cleanup

**Files:**
- No new files

- [ ] **Step 1: Run the full test suite**

Run:
```bash
dotnet test --verbosity normal
```
Expected: All tests pass, 0 failures.

- [ ] **Step 2: Run the formatter**

Run:
```bash
dotnet csharpier src/CSharpHelperExtensions/DictionaryExtensions.cs src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs
```
Expected: Files formatted with no errors.

- [ ] **Step 3: Commit if formatter made changes**

```bash
git add src/CSharpHelperExtensions/DictionaryExtensions.cs src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs
git commit -m "apply csharpier formatting to DictionaryExtensions"
```

- [ ] **Step 4: Verify the build is clean**

Run:
```bash
dotnet build
```
Expected: Build succeeded, 0 errors, 0 warnings.
