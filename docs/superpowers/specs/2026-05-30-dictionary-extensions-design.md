# DictionaryExtensions — Design Spec

**Date:** 2026-05-30  
**Phase:** P2 (per `docs/plan.md`)  
**Status:** Approved

---

## Overview

Add a `DictionaryExtensions` static class providing five fluent extension methods for `IDictionary<TKey, TValue>`. These fill gaps in the BCL for common dictionary operations: conditional add, bulk add, merging, key-based filtering, and read-only wrapping.

---

## File Layout

| Path | Purpose |
|---|---|
| `src/CSharpHelperExtensions/DictionaryExtensions.cs` | Implementation |
| `src/CSharpHelperExtensions.Test/DictionaryExtensionTest.cs` | xUnit tests using Shouldly |

---

## Namespace

`CSharpHelperExtensions.Dictionaries`

Callers import with:
```csharp
using CSharpHelperExtensions.Dictionaries;
```

---

## Methods

### `GetOrAdd`

```csharp
public static TValue GetOrAdd<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    TKey key,
    Func<TKey, TValue> factory)
```

If `key` is already present, returns the existing value. Otherwise invokes `factory(key)`, adds the result, and returns it.

**Throws:** `ArgumentNullException` if `key` or `factory` is null.

---

### `Merge`

```csharp
public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    IDictionary<TKey, TValue> other,
    bool overwrite = false)
```

Adds all entries from `other` into `dict`. When `overwrite` is `false` (default), duplicate keys are skipped. When `true`, existing values are overwritten. Returns `dict` for fluent chaining.

**Null behaviour:** silently no-ops if `other` is null.

---

### `AddRange`

```csharp
public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    IEnumerable<KeyValuePair<TKey, TValue>> pairs,
    bool overwrite = false)
```

Bulk-adds `pairs` into `dict`. Duplicate-key behaviour matches `Merge`: skip by default, overwrite when `true`. Returns `dict` for fluent chaining.

**Null behaviour:** silently no-ops if `pairs` is null.

---

### `RemoveWhere`

```csharp
public static IDictionary<TKey, TValue> RemoveWhere<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    Func<TKey, bool> predicate)
```

Removes all entries whose key satisfies `predicate`. Mutates `dict` in-place. Returns `dict` for fluent chaining.

**Throws:** `ArgumentNullException` if `predicate` is null.

---

### `AsReadOnly`

```csharp
public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
    this IDictionary<TKey, TValue> dict)
```

Wraps `dict` in a `ReadOnlyDictionary<TKey, TValue>`. The wrapper reflects subsequent mutations to the underlying dict (it is a live view, not a copy).

---

## Error Handling Summary

| Method | Null `dict` | Other null args |
|---|---|---|
| `GetOrAdd` | — (C# extension, caller handles) | `ArgumentNullException` for `key` or `factory` |
| `Merge` | — | silently no-op if `other` is null |
| `AddRange` | — | silently no-op if `pairs` is null |
| `RemoveWhere` | — | `ArgumentNullException` for `predicate` |
| `AsReadOnly` | — | no other args |

---

## Testing

One `[Fact]` per behaviour per method. Coverage targets:

- `GetOrAdd`: key exists (returns existing, factory not called), key missing (factory called, value added and returned), null key throws, null factory throws
- `Merge`: no overlap (all added), overlap with `overwrite=false` (existing kept), overlap with `overwrite=true` (existing replaced), null `other` (no-op), fluent return is same instance
- `AddRange`: same cases as `Merge` using a list of `KeyValuePair`s, null `pairs` no-op
- `RemoveWhere`: keys matching predicate removed, non-matching keys kept, empty dict, null predicate throws, fluent return is same instance
- `AsReadOnly`: returned value is `IReadOnlyDictionary`, reflects mutations to underlying dict

---

## Out of Scope

- `GetValueOrDefault` — skipped; BCL already provides this on `Dictionary<TKey, TValue>` (.NET 2.0+)
- Thread safety — no concurrent dictionary support; callers use `ConcurrentDictionary` directly
