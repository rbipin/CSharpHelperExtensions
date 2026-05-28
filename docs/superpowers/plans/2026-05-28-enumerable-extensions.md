# EnumerableExtensions Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add 20 new extension methods to `EnumerableExtensions` covering presence checks, materialization, transforms, conditional mutation, predicate queries, splitting/chunking, min/max defaults, and async projection.

**Architecture:** All new methods are added to the existing `EnumerableExtensions` static class in `src/CSharpHelperExtensions/EnumerableExtensions.cs`, keeping the `CSharpHelperExtensions.Enumerable` namespace. Tests extend the existing `EnumerableExtensionTest.cs`. Every method is null-safe on the source sequence. `AddIf`/`AddRangeIf` target `IList<T>` (not `IEnumerable<T>`) because they mutate the collection.

**Tech Stack:** .NET 10, C# 13, xUnit, Shouldly. `Chunk` (BCL .NET 6+) used for `Batch`. `MinBy`/`MaxBy` (BCL .NET 6+) used for min/max helpers. `SemaphoreSlim` used for `SelectAsync` concurrency cap.

---

## File Map

| Action | Path | Responsibility |
|--------|------|----------------|
| Modify | `src/CSharpHelperExtensions/EnumerableExtensions.cs` | All 20 new methods, plus `using System.Threading;` |
| Modify | `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs` | All new tests, plus `using System.Linq;`, `using System.Threading;`, `using System.Threading.Tasks;` |

---

### Task 1: `HasAny`, `OrEmpty`, `None()` — Collection presence shortcuts

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs` (inside the `EnumerableExtensionTest` class):

```csharp
[Fact]
public void HasAny_ReturnsTrue_WhenSequenceHasElements()
{
    new[] { 1, 2, 3 }.HasAny().ShouldBeTrue();
    new[] { (string)null }.HasAny().ShouldBeTrue();
}

[Fact]
public void HasAny_ReturnsFalse_WhenNullOrEmpty()
{
    ((IEnumerable<int>)null).HasAny().ShouldBeFalse();
    Enumerable.Empty<string>().HasAny().ShouldBeFalse();
}

[Fact]
public void OrEmpty_ReturnsOriginal_WhenNotNull()
{
    new[] { 1, 2 }.OrEmpty().ShouldBe(new[] { 1, 2 });
}

[Fact]
public void OrEmpty_ReturnsEmpty_WhenNull()
{
    ((IEnumerable<int>)null).OrEmpty().ShouldBeEmpty();
}

[Fact]
public void None_ReturnsTrue_WhenNullOrEmpty()
{
    ((IEnumerable<int>)null).None().ShouldBeTrue();
    Enumerable.Empty<int>().None().ShouldBeTrue();
}

[Fact]
public void None_ReturnsFalse_WhenSequenceHasElements()
{
    new[] { 1, 2 }.None().ShouldBeFalse();
}
```

Add `using System.Linq;` to the top of `EnumerableExtensionTest.cs` if not present.

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `HasAny`, `OrEmpty`, `None` not defined.

- [ ] **Step 3: Implement the methods**

Add to the `EnumerableExtensions` static class in `EnumerableExtensions.cs`:

```csharp
public static bool HasAny<T>(this IEnumerable<T> source)
    => source != null && source.Any();

public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
    => source ?? Enumerable.Empty<T>();

public static bool None<T>(this IEnumerable<T> source)
    => source is null || !source.Any();
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS including `HasAny_*`, `OrEmpty_*`, `None_Returns*`.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add HasAny, OrEmpty, None"
```

---

### Task 2: `WhereNotNull`, `AsReadOnlyList`, `ToHashSetSafe` — Materialization helpers

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public void WhereNotNull_FiltersNullsFromReferenceSequence()
{
    var result = new[] { "a", null, "b", null, "c" }.WhereNotNull().ToList();
    result.ShouldBe(new[] { "a", "b", "c" });
}

[Fact]
public void WhereNotNull_OnNullSource_ReturnsEmpty()
{
    ((IEnumerable<string>)null).WhereNotNull().ShouldBeEmpty();
}

[Fact]
public void AsReadOnlyList_MaterializesSequenceInOrder()
{
    IReadOnlyList<int> result = new[] { 3, 1, 2 }.AsReadOnlyList();
    result.ShouldBe(new[] { 3, 1, 2 });
}

[Fact]
public void AsReadOnlyList_OnNullSource_ReturnsEmpty()
{
    IReadOnlyList<int> result = ((IEnumerable<int>)null).AsReadOnlyList();
    result.ShouldBeEmpty();
}

[Fact]
public void ToHashSetSafe_DeduplicatesElements()
{
    var result = new[] { 1, 2, 2, 3 }.ToHashSetSafe();
    result.ShouldBe(new HashSet<int> { 1, 2, 3 });
}

[Fact]
public void ToHashSetSafe_OnNullSource_ReturnsEmpty()
{
    ((IEnumerable<int>)null).ToHashSetSafe().ShouldBeEmpty();
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `WhereNotNull`, `AsReadOnlyList`, `ToHashSetSafe` not defined.

- [ ] **Step 3: Implement the methods**

Add to `EnumerableExtensions.cs`:

```csharp
public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    => source is null ? Enumerable.Empty<T>() : source.Where(x => x is not null)!;

public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> source)
    => (source ?? Enumerable.Empty<T>()).ToList();

public static HashSet<T> ToHashSetSafe<T>(this IEnumerable<T> source)
    => source is null ? new HashSet<T>() : source.ToHashSet();
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add WhereNotNull, AsReadOnlyList, ToHashSetSafe"
```

---

### Task 3: `Yield`, `JoinAsString`, `WithIndex` — Sequence transforms

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public void Yield_WrapsValueTypeAsSingleItemSequence()
{
    42.Yield().ToList().ShouldBe(new[] { 42 });
}

[Fact]
public void Yield_WrapsReferenceTypeAsSingleItemSequence()
{
    "hello".Yield().Single().ShouldBe("hello");
}

[Fact]
public void JoinAsString_JoinsWithSeparator()
{
    new[] { "a", "b", "c" }.JoinAsString(", ").ShouldBe("a, b, c");
}

[Fact]
public void JoinAsString_WorksForNonStringTypes()
{
    new[] { 1, 2, 3 }.JoinAsString("-").ShouldBe("1-2-3");
}

[Fact]
public void JoinAsString_OnNullSource_ReturnsEmptyString()
{
    ((IEnumerable<string>)null).JoinAsString(",").ShouldBe(string.Empty);
}

[Fact]
public void WithIndex_ProjectsZeroBasedIndexAndItem()
{
    var result = new[] { "a", "b", "c" }.WithIndex().ToList();
    result[0].ShouldBe((0, "a"));
    result[1].ShouldBe((1, "b"));
    result[2].ShouldBe((2, "c"));
}

[Fact]
public void WithIndex_OnNullSource_ReturnsEmpty()
{
    ((IEnumerable<string>)null).WithIndex().ShouldBeEmpty();
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `Yield`, `JoinAsString`, `WithIndex` not defined.

- [ ] **Step 3: Implement the methods**

Add to `EnumerableExtensions.cs`:

```csharp
public static IEnumerable<T> Yield<T>(this T item)
{
    yield return item;
}

public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
    => string.Join(separator, source ?? Enumerable.Empty<T>());

public static IEnumerable<(int Index, T Item)> WithIndex<T>(this IEnumerable<T> source)
    => (source ?? Enumerable.Empty<T>()).Select((item, i) => (i, item));
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add Yield, JoinAsString, WithIndex"
```

---

### Task 4: `ToDictionarySafe` — Duplicate-key-safe dictionary

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public void ToDictionarySafe_CreatesDictionaryFromSequence()
{
    var result = new[] { ("a", 1), ("b", 2) }
        .ToDictionarySafe(x => x.Item1, x => x.Item2);
    result["a"].ShouldBe(1);
    result["b"].ShouldBe(2);
}

[Fact]
public void ToDictionarySafe_KeepsLastValue_OnDuplicateKey()
{
    var result = new[] { ("a", 1), ("a", 99) }
        .ToDictionarySafe(x => x.Item1, x => x.Item2);
    result["a"].ShouldBe(99);
}

[Fact]
public void ToDictionarySafe_OnNullSource_ReturnsEmptyDictionary()
{
    var result = ((IEnumerable<(string, int)>)null)
        .ToDictionarySafe(x => x.Item1, x => x.Item2);
    result.ShouldBeEmpty();
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `ToDictionarySafe` not defined.

- [ ] **Step 3: Implement the method**

Add to `EnumerableExtensions.cs`:

```csharp
public static Dictionary<TKey, TValue> ToDictionarySafe<TSource, TKey, TValue>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    Func<TSource, TValue> valueSelector)
    where TKey : notnull
{
    var dict = new Dictionary<TKey, TValue>();
    foreach (var item in source ?? Enumerable.Empty<TSource>())
        dict[keySelector(item)] = valueSelector(item);
    return dict;
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS including `ToDictionarySafe_*`.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add ToDictionarySafe"
```

---

### Task 5: `AddIf`, `AddRangeIf` — Conditional list mutation

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public void AddIf_AddsItem_WhenConditionIsTrue()
{
    var list = new List<int> { 1, 2 };
    list.AddIf(true, 3);
    list.ShouldBe(new[] { 1, 2, 3 });
}

[Fact]
public void AddIf_DoesNotAdd_WhenConditionIsFalse()
{
    var list = new List<int> { 1, 2 };
    list.AddIf(false, 3);
    list.ShouldBe(new[] { 1, 2 });
}

[Fact]
public void AddIf_ReturnsSameListInstance()
{
    var list = new List<int>();
    var returned = list.AddIf(true, 1);
    ReferenceEquals(list, returned).ShouldBeTrue();
}

[Fact]
public void AddRangeIf_AddsItems_WhenConditionIsTrue()
{
    var list = new List<int> { 1 };
    list.AddRangeIf(true, new[] { 2, 3 });
    list.ShouldBe(new[] { 1, 2, 3 });
}

[Fact]
public void AddRangeIf_DoesNotAdd_WhenConditionIsFalse()
{
    var list = new List<int> { 1 };
    list.AddRangeIf(false, new[] { 2, 3 });
    list.ShouldBe(new[] { 1 });
}

[Fact]
public void AddRangeIf_ReturnsSameListInstance()
{
    var list = new List<int>();
    var returned = list.AddRangeIf(true, new[] { 1, 2 });
    ReferenceEquals(list, returned).ShouldBeTrue();
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `AddIf`, `AddRangeIf` not defined.

- [ ] **Step 3: Implement the methods**

Add to `EnumerableExtensions.cs`:

```csharp
public static IList<T> AddIf<T>(this IList<T> list, bool condition, T item)
{
    if (condition) list.Add(item);
    return list;
}

public static IList<T> AddRangeIf<T>(this IList<T> list, bool condition, IEnumerable<T> items)
{
    if (condition)
        foreach (var item in items ?? Enumerable.Empty<T>())
            list.Add(item);
    return list;
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS including `AddIf_*`, `AddRangeIf_*`.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add AddIf, AddRangeIf"
```

---

### Task 6: `ConcatIf` — Conditional concatenation

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public void ConcatIf_ConcatenatesOther_WhenConditionIsTrue()
{
    new[] { 1, 2 }.ConcatIf(true, new[] { 3, 4 }).ShouldBe(new[] { 1, 2, 3, 4 });
}

[Fact]
public void ConcatIf_ReturnsSource_WhenConditionIsFalse()
{
    new[] { 1, 2 }.ConcatIf(false, new[] { 3, 4 }).ShouldBe(new[] { 1, 2 });
}

[Fact]
public void ConcatIf_OnNullSource_ReturnsOther_WhenConditionIsTrue()
{
    ((IEnumerable<int>)null).ConcatIf(true, new[] { 1, 2 }).ShouldBe(new[] { 1, 2 });
}

[Fact]
public void ConcatIf_OnNullSource_ReturnsEmpty_WhenConditionIsFalse()
{
    ((IEnumerable<int>)null).ConcatIf(false, new[] { 1, 2 }).ShouldBeEmpty();
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `ConcatIf` not defined.

- [ ] **Step 3: Implement the method**

Add to `EnumerableExtensions.cs`:

```csharp
public static IEnumerable<T> ConcatIf<T>(
    this IEnumerable<T> source, bool condition, IEnumerable<T> other)
{
    var first = source ?? Enumerable.Empty<T>();
    return condition ? first.Concat(other ?? Enumerable.Empty<T>()) : first;
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS including `ConcatIf_*`.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add ConcatIf"
```

---

### Task 7: `None(predicate)`, `IsSingle`, `IsSingle(predicate)`, `IndexOf` — Predicate queries

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public void None_WithPredicate_ReturnsTrue_WhenNoElementMatches()
{
    new[] { 1, 2, 3 }.None(x => x > 10).ShouldBeTrue();
}

[Fact]
public void None_WithPredicate_ReturnsFalse_WhenAnyElementMatches()
{
    new[] { 1, 2, 3 }.None(x => x > 2).ShouldBeFalse();
}

[Fact]
public void None_WithPredicate_ReturnsTrue_WhenSourceIsNull()
{
    ((IEnumerable<int>)null).None(x => x > 0).ShouldBeTrue();
}

[Fact]
public void IsSingle_ReturnsTrue_WhenExactlyOneElement()
{
    new[] { 42 }.IsSingle().ShouldBeTrue();
}

[Fact]
public void IsSingle_ReturnsFalse_WhenEmpty()
{
    Enumerable.Empty<int>().IsSingle().ShouldBeFalse();
}

[Fact]
public void IsSingle_ReturnsFalse_WhenMoreThanOneElement()
{
    new[] { 1, 2 }.IsSingle().ShouldBeFalse();
}

[Fact]
public void IsSingle_ReturnsFalse_WhenNull()
{
    ((IEnumerable<int>)null).IsSingle().ShouldBeFalse();
}

[Fact]
public void IsSingle_WithPredicate_ReturnsTrue_WhenExactlyOneMatches()
{
    new[] { 1, 2, 3 }.IsSingle(x => x > 2).ShouldBeTrue();
}

[Fact]
public void IsSingle_WithPredicate_ReturnsFalse_WhenZeroMatch()
{
    new[] { 1, 2, 3 }.IsSingle(x => x > 10).ShouldBeFalse();
}

[Fact]
public void IsSingle_WithPredicate_ReturnsFalse_WhenMoreThanOneMatch()
{
    new[] { 1, 2, 3 }.IsSingle(x => x > 1).ShouldBeFalse();
}

[Fact]
public void IndexOf_ReturnsFirstMatchingIndex()
{
    new[] { "a", "b", "c" }.IndexOf(x => x == "b").ShouldBe(1);
}

[Fact]
public void IndexOf_ReturnsZero_WhenFirstElementMatches()
{
    new[] { "a", "b", "c" }.IndexOf(x => x == "a").ShouldBe(0);
}

[Fact]
public void IndexOf_ReturnsMinusOne_WhenNoMatch()
{
    new[] { "a", "b", "c" }.IndexOf(x => x == "z").ShouldBe(-1);
}

[Fact]
public void IndexOf_ReturnsMinusOne_WhenSourceIsNull()
{
    ((IEnumerable<string>)null).IndexOf(x => x == "a").ShouldBe(-1);
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `None` (predicate overload), `IsSingle`, `IndexOf` not defined.

- [ ] **Step 3: Implement the methods**

Add to `EnumerableExtensions.cs`:

```csharp
public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    => source is null || !source.Any(predicate);

public static bool IsSingle<T>(this IEnumerable<T> source)
{
    if (source is null) return false;
    using var e = source.GetEnumerator();
    return e.MoveNext() && !e.MoveNext();
}

public static bool IsSingle<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    => source?.Count(predicate) == 1;

public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
{
    if (source is null) return -1;
    int index = 0;
    foreach (var item in source)
    {
        if (predicate(item)) return index;
        index++;
    }
    return -1;
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS including `None_WithPredicate_*`, `IsSingle_*`, `IndexOf_*`.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add None(predicate), IsSingle, IndexOf"
```

---

### Task 8: `Partition`, `Batch` — Splitting and chunking

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public void Partition_SplitsSequenceIntoMatchedAndRest()
{
    var (matched, rest) = new[] { 1, 2, 3, 4, 5 }.Partition(x => x % 2 == 0);
    matched.ShouldBe(new[] { 2, 4 });
    rest.ShouldBe(new[] { 1, 3, 5 });
}

[Fact]
public void Partition_AllMatch_ReturnsEmptyRest()
{
    var (matched, rest) = new[] { 2, 4, 6 }.Partition(x => x % 2 == 0);
    matched.ShouldBe(new[] { 2, 4, 6 });
    rest.ShouldBeEmpty();
}

[Fact]
public void Partition_NoneMatch_ReturnsEmptyMatched()
{
    var (matched, rest) = new[] { 1, 3, 5 }.Partition(x => x % 2 == 0);
    matched.ShouldBeEmpty();
    rest.ShouldBe(new[] { 1, 3, 5 });
}

[Fact]
public void Partition_OnNullSource_ReturnsTwoEmptyLists()
{
    var (matched, rest) = ((IEnumerable<int>)null).Partition(x => x > 0);
    matched.ShouldBeEmpty();
    rest.ShouldBeEmpty();
}

[Fact]
public void Batch_SplitsSequenceIntoChunksOfGivenSize()
{
    var result = new[] { 1, 2, 3, 4, 5 }.Batch(2).ToList();
    result.Count.ShouldBe(3);
    result[0].ShouldBe(new[] { 1, 2 });
    result[1].ShouldBe(new[] { 3, 4 });
    result[2].ShouldBe(new[] { 5 });
}

[Fact]
public void Batch_OnNullSource_ReturnsEmpty()
{
    ((IEnumerable<int>)null).Batch(3).ShouldBeEmpty();
}

[Fact]
public void Batch_WhenSizeLargerThanSequence_ReturnsSingleChunk()
{
    var result = new[] { 1, 2 }.Batch(10).ToList();
    result.Count.ShouldBe(1);
    result[0].ShouldBe(new[] { 1, 2 });
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `Partition`, `Batch` not defined.

- [ ] **Step 3: Implement the methods**

Add to `EnumerableExtensions.cs`:

```csharp
public static (IReadOnlyList<T> Matched, IReadOnlyList<T> Rest) Partition<T>(
    this IEnumerable<T> source, Func<T, bool> predicate)
{
    var matched = new List<T>();
    var rest = new List<T>();
    foreach (var item in source ?? Enumerable.Empty<T>())
    {
        if (predicate(item)) matched.Add(item);
        else rest.Add(item);
    }
    return (matched, rest);
}

public static IEnumerable<IReadOnlyList<T>> Batch<T>(this IEnumerable<T> source, int size)
    => (source ?? Enumerable.Empty<T>()).Chunk(size).Select(c => (IReadOnlyList<T>)c);
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS including `Partition_*`, `Batch_*`.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add Partition, Batch"
```

---

### Task 9: `MinByOrDefault`, `MaxByOrDefault` — Min/Max with null-safe default

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs`

- [ ] **Step 1: Write the failing tests**

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public void MinByOrDefault_ReturnsElementWithSmallestKey()
{
    new[] { 3, 1, 2 }.MinByOrDefault(x => x).ShouldBe(1);
}

[Fact]
public void MinByOrDefault_ReturnsDefault_WhenSourceIsNull()
{
    ((IEnumerable<int>)null).MinByOrDefault(x => x).ShouldBe(0);
}

[Fact]
public void MinByOrDefault_ReturnsNull_WhenSourceIsEmpty_ReferenceType()
{
    Enumerable.Empty<string>().MinByOrDefault(x => x).ShouldBeNull();
}

[Fact]
public void MaxByOrDefault_ReturnsElementWithLargestKey()
{
    new[] { 3, 1, 2 }.MaxByOrDefault(x => x).ShouldBe(3);
}

[Fact]
public void MaxByOrDefault_ReturnsDefault_WhenSourceIsNull()
{
    ((IEnumerable<int>)null).MaxByOrDefault(x => x).ShouldBe(0);
}

[Fact]
public void MaxByOrDefault_ReturnsNull_WhenSourceIsEmpty_ReferenceType()
{
    Enumerable.Empty<string>().MaxByOrDefault(x => x).ShouldBeNull();
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `MinByOrDefault`, `MaxByOrDefault` not defined.

- [ ] **Step 3: Implement the methods**

Add to `EnumerableExtensions.cs`. Note: BCL `MinBy`/`MaxBy` already return `default` on empty sequences, so null-source handling is all we need to add.

```csharp
public static T? MinByOrDefault<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    => source is null ? default : source.MinBy(keySelector);

public static T? MaxByOrDefault<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    => source is null ? default : source.MaxBy(keySelector);
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS including `MinByOrDefault_*`, `MaxByOrDefault_*`.

- [ ] **Step 5: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add MinByOrDefault, MaxByOrDefault"
```

---

### Task 10: `SelectAsync`, `WhenAllList` — Async projection

**Files:**

- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs` (add `using System.Threading;`)
- Modify: `src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs` (add `using System.Threading;`, `using System.Threading.Tasks;`)

- [ ] **Step 1: Write the failing tests**

Add these usings at the top of `EnumerableExtensionTest.cs` if not present:

```csharp
using System.Threading;
using System.Threading.Tasks;
```

Add to `EnumerableExtensionTest.cs`:

```csharp
[Fact]
public async Task SelectAsync_ProjectsEachElementConcurrently()
{
    var result = await new[] { 1, 2, 3 }
        .SelectAsync(async x => { await Task.Yield(); return x * 2; });
    result.ShouldBe(new[] { 2, 4, 6 });
}

[Fact]
public async Task SelectAsync_OnNullSource_ReturnsEmpty()
{
    var result = await ((IEnumerable<int>)null)
        .SelectAsync(async x => x * 2);
    result.ShouldBeEmpty();
}

[Fact]
public async Task SelectAsync_WithMaxParallel_CapsConcurrency()
{
    int concurrent = 0;
    int maxSeen = 0;

    await Enumerable.Range(1, 10).ToList().SelectAsync(async x =>
    {
        var c = Interlocked.Increment(ref concurrent);
        Interlocked.Exchange(ref maxSeen, Math.Max(maxSeen, c));
        await Task.Delay(20);
        Interlocked.Decrement(ref concurrent);
        return x;
    }, maxParallel: 3);

    maxSeen.ShouldBeLessThanOrEqualTo(3);
}

[Fact]
public async Task WhenAllList_ReturnsAllTaskResults_AsReadOnlyList()
{
    IEnumerable<Task<int>> tasks = new[]
    {
        Task.FromResult(1),
        Task.FromResult(2),
        Task.FromResult(3)
    };
    IReadOnlyList<int> result = await tasks.WhenAllList();
    result.ShouldBe(new[] { 1, 2, 3 });
}

[Fact]
public async Task WhenAllList_OnNullSource_ReturnsEmpty()
{
    var result = await ((IEnumerable<Task<int>>)null).WhenAllList();
    result.ShouldBeEmpty();
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: compile error — `SelectAsync`, `WhenAllList` not defined.

- [ ] **Step 3: Add the using directive**

Add `using System.Threading;` to `EnumerableExtensions.cs` (after `using System.Threading.Tasks;`).

- [ ] **Step 4: Implement the methods**

Add to `EnumerableExtensions.cs`:

```csharp
public static async Task<IReadOnlyList<TResult>> SelectAsync<T, TResult>(
    this IEnumerable<T> source,
    Func<T, Task<TResult>> selector,
    int? maxParallel = null)
{
    if (source is null) return Array.Empty<TResult>();

    if (maxParallel is null)
        return await Task.WhenAll(source.Select(selector));

    using var semaphore = new SemaphoreSlim(maxParallel.Value);
    var tasks = source.Select(async item =>
    {
        await semaphore.WaitAsync();
        try { return await selector(item); }
        finally { semaphore.Release(); }
    });
    return await Task.WhenAll(tasks);
}

public static async Task<IReadOnlyList<T>> WhenAllList<T>(this IEnumerable<Task<T>> tasks)
    => await Task.WhenAll(tasks ?? Enumerable.Empty<Task<T>>());
```

- [ ] **Step 5: Run tests to verify they pass**

```bash
dotnet test --filter "ClassName~EnumerableExtensionTest" --verbosity normal
```

Expected: all tests PASS including `SelectAsync_*`, `WhenAllList_*`.

- [ ] **Step 6: Run the full test suite to check for regressions**

```bash
dotnet test --verbosity normal
```

Expected: all tests PASS with no failures.

- [ ] **Step 7: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs src/CSharpHelperExtensions.Test/EnumerableExtensionTest.cs
git commit -m "feat(enumerable): add SelectAsync, WhenAllList"
```

---

## Self-Review

### Spec Coverage

| Spec member | Task |
|-------------|------|
| `HasAny` | Task 1 |
| `OrEmpty` | Task 1 |
| `None(predicate?)` | Task 1 (no-arg), Task 7 (predicate) |
| `WhereNotNull` | Task 2 |
| `AsReadOnlyList` | Task 2 |
| `ToHashSetSafe` | Task 2 |
| `Yield` | Task 3 |
| `JoinAsString(separator)` | Task 3 |
| `WithIndex` | Task 3 |
| `ToDictionarySafe(key, value)` | Task 4 |
| `AddIf(condition, item)` | Task 5 |
| `AddRangeIf(condition, items)` | Task 5 |
| `ConcatIf(condition, other)` | Task 6 |
| `IsSingle` / `IsSingle(predicate)` | Task 7 |
| `IndexOf(predicate)` | Task 7 |
| `Partition(predicate)` | Task 8 |
| `Batch(size)` | Task 8 |
| `MinByOrDefault(keySelector)` | Task 9 |
| `MaxByOrDefault(keySelector)` | Task 9 |
| `SelectAsync(selector, maxParallel?)` | Task 10 |
| `WhenAllList` | Task 10 |

All 21 spec members (counting `None` overloads separately) are covered. No gaps.
