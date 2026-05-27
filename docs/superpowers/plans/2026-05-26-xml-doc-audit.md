# XML Documentation Audit & Update Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Audit and rewrite all XML documentation comments on public members in the three extension-method source files so that library consumers get accurate, complete, useful IntelliSense and generated docs.

**Architecture:** Pure documentation pass — no logic changes. Each source file is updated in one commit. Build verification confirms no XML-doc compiler errors.

**Tech Stack:** C# 13 / .NET 10, xUnit, `dotnet build`

---

## Issues Found

| Location | Problem |
|---|---|
| `BetweenComparison` enum | No XML docs on type or any value |
| `IsBetween` | `<param name="comparison">` is blank; `<returns>` is "true/ false" |
| `In` | `input` param says "Item to check against" — it's a `params` array |
| `IsNullOrEmpty` (string) | Summary ends mid-sentence: "By default the " |
| `ToJson` | `indentation` param has no description; null-return case undocumented |
| `Compare` enum | No XML docs on type or any value |
| `ContainsOnly` | All params and `<returns>` empty |
| `AreEqual` | Entire `<summary>` is blank; all params and `<returns>` empty |
| `CleanNullOrEmptyItems` | Null-input return behavior not documented |
| `IsNullOrEmpty<T>` | Param tag says `value` but parameter is `values` |
| `ForEach` | All params and `<returns>` empty |
| `Reduce` (no index) | `initialValue` description truncated; `<returns>` empty |
| `Reduce` (with index) | Entire `<summary>` blank; all params and `<returns>` empty |
| `ToNullable<T>` | Summary too brief; all params and `<returns>` empty; no `<exception>` tag |

---

## Files Modified

- `src/CSharpHelperExtensions/GenericExtensions.cs`
- `src/CSharpHelperExtensions/EnumerableExtensions.cs`
- `src/CSharpHelperExtensions/StringExtensions.cs`

---

## Task 1: Update GenericExtensions.cs

**Files:**
- Modify: `src/CSharpHelperExtensions/GenericExtensions.cs`

- [ ] **Step 1: Apply updated XML docs**

Replace the entire XML-doc block for `BetweenComparison`, `IsBetween`, `In`, `IsNullOrEmpty`, and `ToJson` with the following. Leave all method bodies untouched.

**`BetweenComparison` enum** — add summary above the enum declaration and a doc comment on each value:

```csharp
/// <summary>
/// Controls which bounds are included when using <see cref="GenericExtensions.IsBetween{T}"/>.
/// </summary>
public enum BetweenComparison
{
    /// <summary>Inclusive on both ends: lower ≤ value ≤ upper. This is the default.</summary>
    None,
    /// <summary>Exclusive on both ends: lower &lt; value &lt; upper.</summary>
    ExcludeBoth,
    /// <summary>Exclusive lower bound, inclusive upper: lower &lt; value ≤ upper.</summary>
    ExcludeLower,
    /// <summary>Inclusive lower bound, exclusive upper: lower ≤ value &lt; upper.</summary>
    ExcludeUpper
}
```

**`IsBetween<T>`:**

```csharp
/// <summary>
/// Determines whether a value falls within the range defined by <paramref name="lower"/> and <paramref name="upper"/>.
/// </summary>
/// <param name="value">The value to test.</param>
/// <param name="lower">The lower bound of the range.</param>
/// <param name="upper">The upper bound of the range.</param>
/// <param name="comparison">
/// Controls which bounds are included in the comparison.
/// Defaults to <see cref="BetweenComparison.None"/>, which is inclusive on both ends (lower ≤ value ≤ upper).
/// </param>
/// <typeparam name="T">Any type that implements <see cref="IComparable{T}"/>.</typeparam>
/// <returns>
/// <see langword="true"/> if <paramref name="value"/> satisfies the range check;
/// otherwise <see langword="false"/>.
/// </returns>
/// <example>
/// <code>
/// 5.IsBetween(1, 10)                                    // true  (inclusive both ends)
/// 1.IsBetween(1, 10)                                    // true  (lower bound included)
/// 1.IsBetween(1, 10, BetweenComparison.ExcludeLower)    // false (lower bound excluded)
/// 10.IsBetween(1, 10, BetweenComparison.ExcludeUpper)   // false (upper bound excluded)
/// 10.IsBetween(1, 10, BetweenComparison.ExcludeBoth)    // false (both bounds excluded)
/// </code>
/// </example>
```

**`In<T>`:**

```csharp
/// <summary>
/// Determines whether a value equals any item in a given set.
/// Equivalent to SQL's <c>IN</c> operator.
/// </summary>
/// <param name="value">The value to look for.</param>
/// <param name="input">One or more candidate values to match against.</param>
/// <typeparam name="T">The type of the value and candidates.</typeparam>
/// <returns>
/// <see langword="true"/> if <paramref name="value"/> matches any item in <paramref name="input"/>;
/// otherwise <see langword="false"/>.
/// </returns>
/// <example>
/// <code>
/// "admin".In("admin", "superadmin")   // true
/// "guest".In("admin", "superadmin")   // false
/// 3.In(1, 2, 3, 4)                    // true
/// </code>
/// </example>
```

**`IsNullOrEmpty(string)`:**

```csharp
/// <summary>
/// Returns <see langword="true"/> if the string is <see langword="null"/>, empty,
/// or consists only of whitespace characters.
/// Delegates to <see cref="string.IsNullOrWhiteSpace"/>.
/// </summary>
/// <remarks>
/// This overload operates on <see cref="string"/>.
/// For collections, use
/// <see cref="CSharpHelperExtensions.Enumerable.EnumerableExtensions.IsNullOrEmpty{T}(IEnumerable{T})"/>
/// (requires the <c>CSharpHelperExtensions.Enumerable</c> namespace).
/// </remarks>
/// <param name="value">The string to check.</param>
/// <returns>
/// <see langword="true"/> if <paramref name="value"/> is <see langword="null"/>, empty, or whitespace-only;
/// otherwise <see langword="false"/>.
/// </returns>
/// <example>
/// <code>
/// ((string)null).IsNullOrEmpty()   // true
/// "".IsNullOrEmpty()               // true
/// "   ".IsNullOrEmpty()            // true  (whitespace only)
/// "hello".IsNullOrEmpty()          // false
/// </code>
/// </example>
```

**`ToJson<T>`:**

```csharp
/// <summary>
/// Serializes an object to its JSON string representation using Newtonsoft.Json.
/// </summary>
/// <param name="value">
/// The object to serialize. Returns <see langword="null"/> when this argument is <see langword="null"/>.
/// </param>
/// <param name="indentation">
/// When <see langword="true"/>, the JSON output is pretty-printed with indentation.
/// Defaults to <see langword="false"/> (compact, single-line output).
/// </param>
/// <typeparam name="T">The type of the object to serialize. Must be a reference type.</typeparam>
/// <returns>
/// A JSON string representing <paramref name="value"/>,
/// or <see langword="null"/> if <paramref name="value"/> is <see langword="null"/>.
/// </returns>
/// <example>
/// <code>
/// var obj = new { Name = "Alice", Age = 30 };
/// obj.ToJson()                   // {"Name":"Alice","Age":30}
/// obj.ToJson(indentation: true)
/// // {
/// //   "Name": "Alice",
/// //   "Age": 30
/// // }
/// ((object)null).ToJson()        // null
/// </code>
/// </example>
```

- [ ] **Step 2: Build and verify**

```bash
cd /Users/bipin/repo/CSharpHelperExtensions && dotnet build src/CSharpHelperExtensions/CSharpHelperExtensions.csproj
```

Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add src/CSharpHelperExtensions/GenericExtensions.cs
git commit -m "docs: improve XML documentation on GenericExtensions public members"
```

---

## Task 2: Update EnumerableExtensions.cs

**Files:**
- Modify: `src/CSharpHelperExtensions/EnumerableExtensions.cs`

- [ ] **Step 1: Apply updated XML docs**

**`Compare` enum** — add summary above the enum declaration and a doc comment on each value:

```csharp
/// <summary>
/// Controls how two sequences are compared by <see cref="EnumerableExtensions.AreEqual{T}"/>.
/// </summary>
public enum Compare
{
    /// <summary>Elements must appear in the same positional order in both sequences.</summary>
    InOrder,
    /// <summary>
    /// Sequences are equal if they contain the same elements regardless of order.
    /// This is the default.
    /// </summary>
    NoOrder
}
```

**`ContainsOnly<T>`:**

```csharp
/// <summary>
/// Returns <see langword="true"/> if the sequence contains exactly the specified items —
/// no more, no fewer — regardless of order.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="enumerable">The sequence to inspect.</param>
/// <param name="value">The exact set of expected items.</param>
/// <returns>
/// <see langword="true"/> if <paramref name="enumerable"/> has the same count as <paramref name="value"/>
/// and every item in <paramref name="value"/> appears in <paramref name="enumerable"/>.
/// Returns <see langword="false"/> if either argument is <see langword="null"/> or empty,
/// or if the element sets differ.
/// </returns>
/// <example>
/// <code>
/// new[] { 1, 2, 3 }.ContainsOnly(3, 1, 2)   // true  (order doesn't matter)
/// new[] { 1, 2, 3 }.ContainsOnly(1, 2)       // false (extra element in source)
/// new[] { 1, 2 }.ContainsOnly(1, 2, 3)       // false (missing element in source)
/// </code>
/// </example>
```

**`AreEqual<T>`:**

```csharp
/// <summary>
/// Determines whether two sequences contain the same elements.
/// Use <paramref name="comparison"/> to choose between order-sensitive and order-insensitive equality.
/// Both sequences being <see langword="null"/> is treated as equal.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="enumerable">The first sequence.</param>
/// <param name="values">The second sequence to compare against.</param>
/// <param name="comparison">
/// Controls whether element order matters.
/// Defaults to <see cref="Compare.NoOrder"/> (order-insensitive).
/// </param>
/// <returns>
/// <see langword="true"/> if both sequences are equal under the chosen <paramref name="comparison"/> mode;
/// <see langword="false"/> if their counts differ or any element does not match.
/// </returns>
/// <example>
/// <code>
/// new[] { 1, 2, 3 }.AreEqual(new[] { 3, 1, 2 })                       // true  (NoOrder)
/// new[] { 1, 2, 3 }.AreEqual(new[] { 3, 1, 2 }, Compare.InOrder)      // false (order differs)
/// new[] { 1, 2, 3 }.AreEqual(new[] { 1, 2, 3 }, Compare.InOrder)      // true
/// ((IEnumerable&lt;int&gt;)null).AreEqual(null)                             // true  (both null)
/// </code>
/// </example>
```

**`CleanNullOrEmptyItems<T>`:**

```csharp
/// <summary>
/// Returns a new sequence with all <see langword="null"/> elements removed.
/// When <typeparamref name="T"/> is <see cref="string"/>, empty strings and whitespace-only strings
/// are also removed.
/// </summary>
/// <param name="value">
/// The sequence to clean.
/// Returns <see langword="null"/> if the input is <see langword="null"/> or empty.
/// </param>
/// <typeparam name="T">
/// The element type. String sequences get additional empty/whitespace filtering.
/// </typeparam>
/// <returns>
/// A cleaned <see cref="IEnumerable{T}"/> with invalid elements removed,
/// or <see langword="null"/> if the input is <see langword="null"/> or contains no items.
/// </returns>
/// <example>
/// <code>
/// new[] { "hello", null, "", "  ", "world" }.CleanNullOrEmptyItems()
///     // ["hello", "world"]
///
/// new int?[] { 1, null, 2, null, 3 }.CleanNullOrEmptyItems()
///     // [1, 2, 3]
/// </code>
/// </example>
```

**`IsNullOrEmpty<T>`:**

```csharp
/// <summary>
/// Returns <see langword="true"/> if the sequence is <see langword="null"/>, contains no elements,
/// or contains only <see langword="null"/> items.
/// </summary>
/// <param name="values">The sequence to check.</param>
/// <typeparam name="T">The element type.</typeparam>
/// <returns>
/// <see langword="true"/> if <paramref name="values"/> is <see langword="null"/>, empty,
/// or every element is <see langword="null"/>; otherwise <see langword="false"/>.
/// </returns>
/// <example>
/// <code>
/// ((IEnumerable&lt;int&gt;)null).IsNullOrEmpty()         // true
/// new List&lt;string&gt;().IsNullOrEmpty()               // true
/// new[] { (string)null, null }.IsNullOrEmpty()          // true  (all-null items)
/// new[] { 1, 2, 3 }.IsNullOrEmpty()                    // false
/// </code>
/// </example>
```

**`ForEach<T>`:**

```csharp
/// <summary>
/// Executes an action on each element of the sequence and returns the original sequence unchanged.
/// Useful for chaining side-effectful operations in a fluent pipeline.
/// </summary>
/// <param name="values">
/// The sequence to iterate.
/// If <see langword="null"/>, the action is not invoked and <see langword="null"/> is returned.
/// </param>
/// <param name="execute">The action to run for each element.</param>
/// <typeparam name="T">The element type.</typeparam>
/// <returns>The original <paramref name="values"/> reference (not a copy).</returns>
/// <example>
/// <code>
/// var log = new List&lt;string&gt;();
/// new[] { "a", "b", "c" }
///     .ForEach(item => log.Add(item.ToUpper()))
///     .ForEach(item => Console.WriteLine(item));
/// // log == ["A", "B", "C"]
/// // original sequence ["a", "b", "c"] is printed to console
/// </code>
/// </example>
```

**`Reduce<TIn, TOut>` (without index):**

```csharp
/// <summary>
/// Reduces a sequence to a single accumulated value by repeatedly applying a reducer function.
/// Equivalent to JavaScript's <c>Array.prototype.reduce()</c>.
/// </summary>
/// <param name="values">
/// The sequence to reduce.
/// If <see langword="null"/> or empty, returns the default value of <typeparamref name="TOut"/>.
/// </param>
/// <param name="execute">
/// The reducer function. Receives the current element and the current accumulated value,
/// and returns the new accumulated value.
/// </param>
/// <param name="initialValue">
/// The starting value for the accumulator before the first element is processed.
/// Defaults to <see langword="default"/>(<typeparamref name="TOut"/>).
/// </param>
/// <typeparam name="TIn">The element type of the input sequence.</typeparam>
/// <typeparam name="TOut">The type of the accumulated result.</typeparam>
/// <returns>The final accumulated value after all elements have been processed.</returns>
/// <example>
/// <code>
/// // Sum integers
/// new[] { 1, 2, 3, 4 }.Reduce((item, acc) => acc + item, initialValue: 0)   // 10
///
/// // Build a comma-separated string
/// new[] { "a", "b", "c" }
///     .Reduce((item, acc) => acc == "" ? item : acc + ", " + item, "")        // "a, b, c"
/// </code>
/// </example>
```

**`Reduce<TIn, TOut>` (with index):**

```csharp
/// <summary>
/// Reduces a sequence to a single accumulated value by repeatedly applying a reducer function
/// that also receives the current element's zero-based index.
/// </summary>
/// <param name="values">
/// The sequence to reduce.
/// If <see langword="null"/> or empty, returns the default value of <typeparamref name="TOut"/>.
/// </param>
/// <param name="execute">
/// The reducer function. Receives the current element, the current accumulated value,
/// and the zero-based index of the current element; returns the new accumulated value.
/// </param>
/// <param name="initialValue">
/// The starting value for the accumulator before the first element is processed.
/// Defaults to <see langword="default"/>(<typeparamref name="TOut"/>).
/// </param>
/// <typeparam name="TIn">The element type of the input sequence.</typeparam>
/// <typeparam name="TOut">The type of the accumulated result.</typeparam>
/// <returns>The final accumulated value after all elements have been processed.</returns>
/// <example>
/// <code>
/// // Build indexed labels
/// new[] { "apple", "banana", "cherry" }
///     .Reduce((item, acc, index) => acc + $"{index}: {item}\n", "")
///     // "0: apple\n1: banana\n2: cherry\n"
/// </code>
/// </example>
```

- [ ] **Step 2: Build and verify**

```bash
cd /Users/bipin/repo/CSharpHelperExtensions && dotnet build src/CSharpHelperExtensions/CSharpHelperExtensions.csproj
```

Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add src/CSharpHelperExtensions/EnumerableExtensions.cs
git commit -m "docs: improve XML documentation on EnumerableExtensions public members"
```

---

## Task 3: Update StringExtensions.cs

**Files:**
- Modify: `src/CSharpHelperExtensions/StringExtensions.cs`

- [ ] **Step 1: Apply updated XML docs**

**`ToNullable<T>`:**

```csharp
/// <summary>
/// Converts a string to the specified nullable value type using its registered <see cref="TypeConverter"/>.
/// Returns <see langword="null"/> when the input is <see langword="null"/>, empty, or whitespace.
/// </summary>
/// <param name="input">The string to convert.</param>
/// <typeparam name="T">
/// The target value type (e.g. <see cref="int"/>, <see cref="DateTime"/>, <see cref="Guid"/>).
/// Must be a struct; the return type is <typeparamref name="T"/>?.
/// </typeparam>
/// <returns>
/// The converted <typeparamref name="T"/> value wrapped in a nullable,
/// or <see langword="null"/> if <paramref name="input"/> is <see langword="null"/>, empty, or whitespace.
/// </returns>
/// <exception cref="Exception">
/// Rethrows any exception thrown by the underlying <see cref="TypeConverter"/> when the string
/// cannot be parsed as <typeparamref name="T"/> (e.g. <see cref="FormatException"/>
/// or <see cref="NotSupportedException"/>).
/// </exception>
/// <example>
/// <code>
/// "42".ToNullable&lt;int&gt;()                  // (int?)42
/// "".ToNullable&lt;int&gt;()                    // null
/// "   ".ToNullable&lt;int&gt;()                 // null (whitespace)
/// "2024-01-15".ToNullable&lt;DateTime&gt;()    // (DateTime?)new DateTime(2024, 1, 15)
/// "not-a-number".ToNullable&lt;int&gt;()       // throws FormatException
/// </code>
/// </example>
```

- [ ] **Step 2: Build and verify**

```bash
cd /Users/bipin/repo/CSharpHelperExtensions && dotnet build src/CSharpHelperExtensions/CSharpHelperExtensions.csproj
```

Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add src/CSharpHelperExtensions/StringExtensions.cs
git commit -m "docs: improve XML documentation on StringExtensions.ToNullable"
```

---

## Task 4: Full solution build and test

- [ ] **Step 1: Build full solution**

```bash
cd /Users/bipin/repo/CSharpHelperExtensions && dotnet build
```

Expected: `Build succeeded. 0 Error(s) 0 Warning(s)`

- [ ] **Step 2: Run all tests**

```bash
cd /Users/bipin/repo/CSharpHelperExtensions && dotnet test --verbosity normal
```

Expected: All tests pass. Test output should show no failures.
