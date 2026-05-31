# Generic Extensions — Implementation Spec

A complete list of extension methods to implement in `CSharpHelperExtensions`. All members are BCL-only with no domain dependencies.

---

## Extension classes

### `StringExtensions` (`CSharpHelperExtensions.Strings`)

| Member | Purpose |
|---|---|
| `HasValue` | true when not null/whitespace |
| `OrEmpty` | null → `string.Empty` |
| `OrDefault(fallback)` | null/whitespace → fallback |
| `Truncate(maxLength)` | clip to length |
| `TrimToLower` / `TrimToUpper` | trim + case in one call |
| `MaskStart(visibleCount, maskChar)` | `****56` style masking |
| `ToIntOrNull` / `ToDecimalOrNull` / `ToDateTimeOrNull` / `ToGuidOrNull` / `ToBoolOrNull` | typed parse-or-null helpers |
| `Base64Encode` / `Base64Decode` | standard Base64 round-trip |
| `ToBase64Url` / `FromBase64Url` | URL-safe Base64 (no `+/=`) |
| `ToUtf8Bytes` | `Encoding.UTF8.GetBytes` wrapper |
| `ToUtf8Stream` | string → `MemoryStream` |
| `JoinWith(values)` | `string.Join` with fluent receiver |
| `EqualsIgnoreCase` / `ContainsIgnoreCase` / `StartsWithIgnoreCase` / `EndsWithIgnoreCase` | ordinal ignore-case comparisons |
| `RemoveWhitespace` | strip all whitespace |
| `CollapseWhitespace` | runs of spaces → single space |
| `ReplaceMany(pairs)` | chain multiple replacements |
| `RemoveDiacritics` | strip accent marks |
| `ToSlug` | lowercase, diacritics-free, dash-separated |
| `IsNumeric` / `IsAlpha` / `IsAlphaNumeric` | char-set predicates |
| `Reverse` | reverse character order |
| `SplitNonEmpty(separators)` | split + remove empty entries |
| `EnsurePrefix(prefix)` / `EnsureSuffix(suffix)` | add if missing |
| `TrimPrefix(prefix)` / `TrimSuffix(suffix)` | remove if present |

---

### `EnumerableExtensions` (`CSharpHelperExtensions.Enumerable`)

| Member | Purpose |
|---|---|
| `HasAny` | non-null + non-empty check |
| `OrEmpty` | null → empty sequence |
| `AsReadOnlyList` | materialize as `IReadOnlyList<T>` |
| `WhereNotNull` | filter out nulls |
| `JoinAsString(separator)` | `string.Join` on a sequence |
| `ToDictionarySafe(key, value)` | `ToDictionary` that handles duplicate keys |
| `AddIf(condition, item)` | conditional `Add` returning the list |
| `AddRangeIf(condition, items)` | conditional `AddRange` returning the list |
| `None(predicate?)` | `!Any(...)` with clearer intent |
| `IsSingle` / `IsSingle(predicate)` | exactly one element, allocation-safe |
| `IndexOf(predicate)` | first matching index or -1 |
| `WithIndex` | `(index, item)` tuples |
| `Partition(predicate)` | split into matched + rest in one pass |
| `Batch(size)` | null-safe chunking |
| `MinByOrDefault(keySelector)` / `MaxByOrDefault` | `MinBy`/`MaxBy` that return default on empty |
| `ConcatIf(condition, other)` | conditional concat |
| `Yield` | wrap a single item as a sequence |
| `ToHashSetSafe` | null-safe `ToHashSet` |
| `SelectAsync(selector, maxParallel?)` | concurrent projection with optional parallelism cap |
| `WhenAllList` | `Task.WhenAll` returning `IReadOnlyList<T>` |

---

### `DictionaryExtensions` (`CSharpHelperExtensions.Enumerable`)

| Member | Purpose |
|---|---|
| `GetOrAdd(key, factory)` | add-if-missing, return value |
| `GetValueOrDefault(key)` | null-safe value lookup |
| `Merge(other, overwrite?)` | combine two dictionaries |
| `AddRange(pairs, overwrite?)` | bulk add |
| `RemoveWhere(predicate)` | filter-in-place |
| `AsReadOnly` | wrap as `IReadOnlyDictionary` |

---

### `ObjectExtensions` (`CSharpHelperExtensions`)

| Member | Purpose |
|---|---|
| `Pipe(transform)` | `x.Pipe(f)` → `f(x)` |
| `Tap(action)` | run side-effect, return self |
| `When(condition, transform)` | conditional transform |
| `WhenNotNull(transform)` | transform only when not null |
| `In(values)` | `x.In(a,b,c)` membership check |
| `ThrowIfNull(paramName?)` | throw `ArgumentNullException` inline |
| `As<T>` | safe cast (`x as T`) |
| `Is<T>` | type check (`x is T`) |

---

### `GuardExtensions` (`CSharpHelperExtensions`)

| Member | Purpose |
|---|---|
| `ThrowIfNull(paramName?)` | `ArgumentNullException` |
| `ThrowIfEmpty(paramName?)` | `ArgumentException` for empty strings/collections |
| `ThrowIfWhitespace(paramName?)` | `ArgumentException` for whitespace strings |
| `ThrowIf(condition, message)` | general precondition check |

---

### `NullableExtensions` (`CSharpHelperExtensions`)

| Member | Purpose |
|---|---|
| `OrDefault(fallback)` | `T?` → `T` with fallback |
| `Map(transform)` | apply transform when has value |
| `IsZeroOrNull` | true when null or 0 (for numeric Nullable types) |

---

### `BooleanExtensions` (`CSharpHelperExtensions`)

| Member | Purpose |
|---|---|
| `ToYesNo` / `ToYN` / `ToOneZero` | format for reports |
| `Then(ifTrue, ifFalse)` | inline ternary as method |
| `AsBoolean()` | to convert int, small int, 'y', 'n', 'true', 'false', 'yes', 'no' to boolean true or false |

---

### `DecimalExtensions` (`CSharpHelperExtensions`)

| Member | Purpose |
|---|---|
| `RoundToHalf` | round to nearest 0.5 |
| `RoundCurrency` | round to 2 decimal places (MidpointRounding.AwayFromZero) |
| `IsZero` / `IsPositive` / `IsNegative` | sign predicates |

---

### `GuidExtensions` (`CSharpHelperExtensions`)

| Member | Purpose |
|---|---|
| `IsEmpty` | `g == Guid.Empty` |
| `OrNew` | return `Guid.NewGuid()` when empty |
| `ToShortString` | URL-safe base64 form (22 chars) |

---

### `DateTimeExtensions` (`CSharpHelperExtensions.Dates`)

| Member | Purpose |
|---|---|
| `StartOfDay` / `EndOfDay` | midnight / 23:59:59.999 |
| `StartOfWeek(firstDay?)` / `EndOfWeek` | week boundaries |
| `StartOfMonth` / `EndOfMonth` | month boundaries |
| `IsBetween(start, end)` | inclusive range check |
| `ToIsoString` | ISO 8601 (`yyyy-MM-ddTHH:mm:ss`) |
| `ToShortIso` | date-only ISO (`yyyy-MM-dd`) |
| `ToDateOnly` | convert to `DateOnly` |
| `ToUnixTimestamp` / `ToUnixTimestampMs` | seconds / milliseconds since epoch |
| `Age(on?)` | years elapsed with rollover |
| `IsWeekend` / `IsWeekday` | day-of-week predicates |
| `NextBusinessDay` / `AddBusinessDays(n)` | Mon-Fri arithmetic (no holiday awareness) |
| `ClampTo(min, max)` | constrain to range |
| `ToRelativeString(now?)` | "3 minutes ago" / "in 2 days" |
| `TimeSince` / `TimeUntil` | `UtcNow - x` / `x - UtcNow` |
| `EachDayUntil(end)` | enumerate days in a range |

---

### `DateOnlyExtensions` (`CSharpHelperExtensions.Dates`)

| Member | Purpose |
|---|---|
| `StartOfWeek(firstDay?)` / `EndOfWeek` | week boundaries |
| `StartOfMonth` / `EndOfMonth` | month boundaries |
| `IsBetween(start, end)` | inclusive range check |
| `ToIsoString` | `yyyy-MM-dd` |
| `ToDateTime(time?)` | convert to `DateTime` |
| `EachDayUntil(end)` | enumerate days in a range |

---

### `EnumExtensions` (`CSharpHelperExtensions.Enums`)

| Member | Purpose |
|---|---|
| `GetDescription` | read `[Description]` attribute |
| `ToEnum<TEnum>(fallback?)` | string → enum |
| `TryToEnum<TEnum>(out value)` | safe string → enum |
| `All<TEnum>` | all defined values |
| `IsDefinedValue` | `Enum.IsDefined` wrapper |

---

### `TaskExtensions` (`CSharpHelperExtensions.Tasks`)

| Member | Purpose |
|---|---|
| `WithTimeout(timeout)` | throw `TimeoutException` if not done in time |
| `WithCancellation(token)` | make non-cancellable task respect a token |
| `FireAndForget(onError?)` | discard task, log exceptions safely |
| `OrDefault(fallback)` | return fallback on any exception |
| `Then(transform)` | sync continuation |
| `Then(asyncTransform)` | async continuation |
| `NoSync` | shorthand for `.ConfigureAwait(false)` |

---

### `ExceptionExtensions` (`CSharpHelperExtensions.Reflection`)

| Member | Purpose |
|---|---|
| `GetFullMessage` | join full inner-exception chain with ` → ` |
| `IsTransient` | matches well-known retryable patterns (SocketException, TimeoutException, transient HTTP) |
| `Flatten` | unroll `AggregateException` + inner chain |

---

### `TypeExtensions` (`CSharpHelperExtensions.Reflection`)

| Member | Purpose |
|---|---|
| `GetFriendlyName` | readable generic type name (e.g. `Dictionary<string, int>`) |
| `IsNullableValueType` | `Nullable.GetUnderlyingType(t) != null` |
| `ImplementsGeneric(openGeneric)` | e.g. `typeof(List<int>).ImplementsGeneric(typeof(IEnumerable<>))` |
| `GetAttribute<TAttr>` | sugar over `GetCustomAttribute<TAttr>()` |

---

### `StreamExtensions` (`CSharpHelperExtensions.Text`)

| Member | Purpose |
|---|---|
| `ToUtf8MemoryStream` | string → seekable `MemoryStream` |
| `ReadAsStringAsync(token?)` | stream → string |
| `ToByteArrayAsync(token?)` | stream → `byte[]` |
| `ReadAsJsonAsync<T>(options?, token?)` | deserialize stream as JSON |
| `WriteAsJsonAsync<T>(value, options?, token?)` | serialize to stream as JSON |

---

### `JsonExtensions` (`CSharpHelperExtensions.Text`)

Uses `System.Text.Json`.

| Member | Purpose |
|---|---|
| `ToJson(options?)` | serialize to JSON string |
| `FromJson<T>(options?)` | deserialize from JSON string |
| `TryFromJson<T>(out value, options?)` | safe deserialize |
| `WithCamelCase(options)` | add camelCase naming policy |
| `WithEnumStrings(options)` | add string enum converter |
| `WithPrettyPrint(options)` | enable indented output |

---

### `RandomExtensions` (`CSharpHelperExtensions`)

| Member | Purpose |
|---|---|
| `NextOf<T>(list)` | pick a random element from a list |
| `Shuffle<T>(source, random?)` | Fisher-Yates shuffle |

---

### `HttpResponseMessageExtensions` (`CSharpHelperExtensions.Net`)

| Member | Purpose |
|---|---|
| `EnsureSuccessAndReadAsStringAsync` | `EnsureSuccessStatusCode` + `ReadAsStringAsync` |
| `ReadJsonAsync<T>(options?)` | read and deserialize response body as JSON |

---

## Implementation phases

| Phase | Scope |
|---|---|
| **P1** | `StringExtensions`, `EnumerableExtensions`, `ObjectExtensions`, `GuardExtensions`, `TaskExtensions`, `ExceptionExtensions` |
| **P2** | `DictionaryExtensions`, `NullableExtensions`, `BooleanExtensions`, `DecimalExtensions`, `GuidExtensions`, `DateTimeExtensions`, `DateOnlyExtensions`, `EnumExtensions` |
| **P3** | `StreamExtensions`, `JsonExtensions`, `TypeExtensions`, `RandomExtensions`, `HttpResponseMessageExtensions` |

---

## Open questions

1. **NuGet visibility** — publish to nuget.org under an OSS license, or keep on a private feed?
2. **Async-only vs sync overloads** — for `TaskExtensions`, `StreamExtensions`, `HttpResponseMessageExtensions`: async-only, or include sync where BCL supports it?
3. **`Pipe`/`Tap` naming** — ship as-is, or also add `Let`/`Also` aliases (Kotlin-style)? Aliases increase IntelliSense noise.
4. **JSON library** — `JsonExtensions` targets `System.Text.Json`; existing `ToJson` in `GenericExtensions` uses Newtonsoft.Json. Migrate in a breaking v3.0, or keep both?
