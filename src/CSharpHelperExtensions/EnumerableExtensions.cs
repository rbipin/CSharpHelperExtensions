using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CSharpHelperExtensions.Enumerable;

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
    NoOrder,
}

public static class EnumerableExtensions
{
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
    /// <remarks>
    /// Duplicate handling: the check uses <see cref="IEnumerable{T}.Contains"/> internally,
    /// so sequences with repeated elements may produce unexpected results.
    /// For example, <c>new[] { 1, 2, 2 }.ContainsOnly(1, 1, 2)</c> returns <see langword="true"/>
    /// because counts match and every item in the expected set appears somewhere in the source.
    /// </remarks>
    /// <example>
    /// <code>
    /// new[] { 1, 2, 3 }.ContainsOnly(3, 1, 2)   // true  (order doesn't matter)
    /// new[] { 1, 2, 3 }.ContainsOnly(1, 2)       // false (extra element in source)
    /// new[] { 1, 2 }.ContainsOnly(1, 2, 3)       // false (missing element in source)
    /// </code>
    /// </example>
    public static bool ContainsOnly<T>(this IEnumerable<T> enumerable, params T[] value)
    {
        if (value.IsNullOrEmpty() || enumerable.IsNullOrEmpty())
        {
            return false;
        }

        var list = enumerable.ToList();
        if (list.Count != value.Length)
        {
            return false;
        }

        var set = new HashSet<T>(list);
        return value.All(set.Contains);
    }

    /// <summary>
    /// Determines whether two sequences contain the same elements.
    /// Use <paramref name="comparison"/> to choose between order-sensitive and order-insensitive equality.
    /// Both sequences being <see langword="null"/> is treated as equal,
    /// and a <see langword="null"/> sequence is considered equal to an empty sequence.
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
    /// <remarks>
    /// When using <see cref="Compare.NoOrder"/>, equality is checked via <c>Contains</c> on each element,
    /// so sequences with duplicate elements may compare as equal even if multiplicities differ.
    /// For example, <c>new[] { 1, 1, 2 }.AreEqual(new[] { 1, 2, 2 })</c> returns <see langword="true"/>
    /// because both have count 3 and every element of the first appears in the second.
    /// Use <see cref="Compare.InOrder"/> for strict positional equality.
    /// </remarks>
    /// <example>
    /// <code>
    /// new[] { 1, 2, 3 }.AreEqual(new[] { 3, 1, 2 })                       // true  (NoOrder)
    /// new[] { 1, 2, 3 }.AreEqual(new[] { 3, 1, 2 }, Compare.InOrder)      // false (order differs)
    /// new[] { 1, 2, 3 }.AreEqual(new[] { 1, 2, 3 }, Compare.InOrder)      // true
    /// ((IEnumerable&lt;int&gt;)null).AreEqual(null)                             // true  (both null)
    /// ((IEnumerable&lt;int&gt;)null).AreEqual(new List&lt;int&gt;())                  // true  (null == empty)
    /// </code>
    /// </example>
    public static bool AreEqual<T>(
        this IEnumerable<T> enumerable,
        IEnumerable<T> values,
        Compare comparison = Compare.NoOrder
    )
    {
        if (ReferenceEquals(enumerable, null) && ReferenceEquals(values, null))
        {
            return true;
        }

        if (ReferenceEquals(enumerable, values))
        {
            return true;
        }

        var left = enumerable?.ToList() ?? [];
        var right = values?.ToList() ?? [];

        if (left.Count != right.Count)
        {
            return false;
        }

        if (comparison == Compare.InOrder)
        {
            return left.SequenceEqual(right);
        }

        var rightSet = new HashSet<T>(right);
        return left.All(rightSet.Contains);
    }

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
    public static IEnumerable<T> CleanNullOrEmptyItems<T>(this IEnumerable<T> value)
    {
        if (value is null)
        {
            return [];
        }
        return value
            .Where(item => item is string s ? !string.IsNullOrWhiteSpace(s) : item is not null)
            .ToList();
    }

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
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> values) =>
        values is null || !values.Any(item => item is not null);

    /// <summary>
    /// Executes an action on each element of the sequence and returns the original sequence unchanged.
    /// Useful for chaining side-effect operations in a fluent pipeline.
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
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> values, Action<T> execute)
    {
        var collection = values?.ToList() ?? new List<T>();
        foreach (var item in collection)
        {
            execute(item);
        }
        return collection;
    }

    /// <summary>
    /// Invokes an async action on each element of a sequence and waits for all operations
    /// to complete concurrently.
    /// </summary>
    /// <param name="values">The source sequence. A null sequence is treated as empty.</param>
    /// <param name="execute">An async action applied to each element.</param>
    /// <typeparam name="T">The type of elements in the source sequence.</typeparam>
    /// <returns>
    /// A <see cref="Task"/> that completes when all async actions have finished.
    /// </returns>
    public static Task ForEach<T>(this IEnumerable<T> values, Func<T, Task> execute) =>
        Task.WhenAll(values.OrEmpty().Select(execute));

    /// <summary>
    /// Asynchronously projects each element of a sequence using the given async transform
    /// and yields results lazily as they complete.
    /// </summary>
    /// <param name="values">The source sequence. A null sequence is treated as empty.</param>
    /// <param name="execute">An async transform applied to each element in order.</param>
    /// <typeparam name="T">The type of elements in the source sequence.</typeparam>
    /// <typeparam name="K">The type of elements in the result sequence.</typeparam>
    /// <returns>
    /// An <see cref="IAsyncEnumerable{K}"/> of transformed values, yielded sequentially
    /// as each async operation completes.
    /// </returns>
    public static async IAsyncEnumerable<K> ForEach<T, K>(
        this IEnumerable<T> values,
        Func<T, Task<K>> execute
    )
    {
        foreach (var item in values.OrEmpty())
        {
            yield return await execute(item);
        }
    }

    /// <summary>
    /// Reduces a sequence to a single accumulated value by repeatedly applying a reducer function.
    /// Equivalent to JavaScript's <c>Array.prototype.reduce()</c>.
    /// </summary>
    /// <param name="values">
    /// The sequence to reduce.
    /// If <see langword="null"/> or empty, returns <paramref name="initialValue"/> unchanged.
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
    public static TOut Reduce<TIn, TOut>(
        this IEnumerable<TIn> values,
        Func<TIn, TOut, TOut> execute,
        TOut initialValue = default!
    )
    {
        var acc = initialValue;
        foreach (var item in values.OrEmpty())
        {
            acc = execute(item, acc);
        }
        return acc;
    }

    /// <summary>
    /// Reduces a sequence to a single accumulated value by repeatedly applying a reducer function
    /// that also receives the current element's zero-based index.
    /// </summary>
    /// <param name="values">
    /// The sequence to reduce.
    /// If <see langword="null"/> or empty, returns <paramref name="initialValue"/> unchanged.
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
    public static TOut Reduce<TIn, TOut>(
        this IEnumerable<TIn> values,
        Func<TIn, TOut, int, TOut> execute,
        TOut initialValue = default!
    )
    {
        var collection = values?.ToList() ?? [];
        var acc = initialValue;
        for (int i = 0; i < collection.Count; i++)
        {
            acc = execute(collection[i], acc, i);
        }
        return acc;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the sequence is non-null and contains at least one element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to check.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="source"/> is not <see langword="null"/> and has at least one element;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool HasAny<T>(this IEnumerable<T> source) => source != null && source.Any();

    /// <summary>
    /// Returns the sequence unchanged if non-null, or an empty sequence if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to evaluate.</param>
    /// <returns>
    /// <paramref name="source"/> if it is not <see langword="null"/>; otherwise <see cref="Enumerable.Empty{T}"/>.
    /// </returns>
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source) =>
        source ?? System.Linq.Enumerable.Empty<T>();

    /// <summary>
    /// Returns <see langword="true"/> if the sequence is <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to check.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="source"/> is <see langword="null"/> or empty;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool None<T>(this IEnumerable<T> source) => source is null || !source.Any();

    /// <summary>
    /// Returns <see langword="true"/> if no element in the sequence satisfies the predicate,
    /// or if the sequence is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to check.</param>
    /// <param name="predicate">A function to test each element.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="source"/> is <see langword="null"/> or no element
    /// matches <paramref name="predicate"/>; otherwise <see langword="false"/>.
    /// </returns>
    public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
        source is null || !source.Any(predicate);

    /// <summary>
    /// Returns <see langword="true"/> if the sequence contains exactly one element.
    /// Returns <see langword="false"/> if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to check.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="source"/> has exactly one element;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsSingle<T>(this IEnumerable<T> source)
    {
        if (source is null)
        {
            return false;
        }
        using var e = source.GetEnumerator();
        return e.MoveNext() && !e.MoveNext();
    }

    /// <summary>
    /// Returns <see langword="true"/> if exactly one element in the sequence satisfies the predicate.
    /// Returns <see langword="false"/> if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to check.</param>
    /// <param name="predicate">A function to test each element.</param>
    /// <returns>
    /// <see langword="true"/> if exactly one element matches <paramref name="predicate"/>;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsSingle<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
        source?.Count(predicate) == 1;

    /// <summary>
    /// Returns the zero-based index of the first element in the sequence that satisfies the predicate,
    /// or <c>-1</c> if no element matches or the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to search.</param>
    /// <param name="predicate">A function to test each element.</param>
    /// <returns>
    /// The index of the first matching element, or <c>-1</c> if none is found or source is <see langword="null"/>.
    /// </returns>
    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        if (source is null)
        {
            return -1;
        }
        int index = 0;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    /// <summary>
    /// Filters out <see langword="null"/> elements from a sequence of reference types.
    /// Returns an empty sequence if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type (must be a reference type).</typeparam>
    /// <param name="source">The sequence to filter.</param>
    /// <returns>A sequence containing only non-null elements.</returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class =>
        source is null
            ? System.Linq.Enumerable.Empty<T>()
            : source.Where(x => x is not null).Cast<T>();

    /// <summary>
    /// Materializes the sequence into an <see cref="IReadOnlyList{T}"/>, preserving order.
    /// Returns an empty list if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to materialize.</param>
    /// <returns>An <see cref="IReadOnlyList{T}"/> containing all elements in order.</returns>
    public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> source) =>
        (source ?? System.Linq.Enumerable.Empty<T>()).ToList();

    /// <summary>
    /// Converts the sequence to a <see cref="HashSet{T}"/>, deduplicating elements.
    /// Returns an empty set if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to convert.</param>
    /// <returns>A <see cref="HashSet{T}"/> containing the distinct elements.</returns>
    public static HashSet<T> ToHashSetSafe<T>(this IEnumerable<T> source) =>
        source is null ? new HashSet<T>() : source.ToHashSet();

    /// <summary>
    /// Wraps a single value in an <see cref="IEnumerable{T}"/> containing only that item.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="item">The value to wrap.</param>
    /// <returns>A sequence containing exactly one element: <paramref name="item"/>.</returns>
    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }

    /// <summary>
    /// Concatenates the elements of a sequence into a single string using the specified separator.
    /// Returns <see cref="string.Empty"/> if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence whose elements to join.</param>
    /// <param name="separator">The string to use as a separator between elements.</param>
    /// <returns>A string of all elements joined by <paramref name="separator"/>, or <see cref="string.Empty"/> if source is <see langword="null"/>.</returns>
    public static string JoinAsString<T>(this IEnumerable<T> source, string separator) =>
        string.Join(separator, source ?? System.Linq.Enumerable.Empty<T>());

    /// <summary>
    /// Projects each element of a sequence into a tuple of its zero-based index and the element itself.
    /// Returns an empty sequence if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to index.</param>
    /// <returns>A sequence of <c>(Index, Item)</c> tuples.</returns>
    public static IEnumerable<(int Index, T Item)> WithIndex<T>(this IEnumerable<T> source) =>
        (source ?? System.Linq.Enumerable.Empty<T>()).Select((item, i) => (i, item));

    /// <summary>
    /// Converts a sequence to a <see cref="Dictionary{TKey, TValue}"/> using the specified key and value selectors.
    /// Returns an empty dictionary if the source is <see langword="null"/>.
    /// When duplicate keys are encountered, the last value for that key is retained.
    /// </summary>
    /// <typeparam name="TSource">The element type of the input sequence.</typeparam>
    /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
    /// <param name="source">The sequence to convert to a dictionary.</param>
    /// <param name="keySelector">A function to extract the key from each element.</param>
    /// <param name="valueSelector">A function to extract the value from each element.</param>
    /// <returns>
    /// A <see cref="Dictionary{TKey, TValue}"/> containing the projected key-value pairs,
    /// or an empty dictionary if source is <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// Unlike <see cref="Enumerable.ToDictionary{TSource, TKey, TValue}"/>,
    /// this method does not throw an <see cref="ArgumentException"/> on duplicate keys.
    /// Instead, the last occurrence of a duplicate key overwrites previous values.
    /// This is similar to dictionary initialization with repeated keys.
    /// </remarks>
    /// <example>
    /// <code>
    /// var pairs = new[] { ("a", 1), ("b", 2), ("a", 99) };
    /// var dict = pairs.ToDictionarySafe(x => x.Item1, x => x.Item2);
    /// // dict["a"] == 99  (last value wins)
    /// // dict["b"] == 2
    /// </code>
    /// </example>
    public static Dictionary<TKey, TValue> ToDictionarySafe<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector
    )
        where TKey : notnull
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var item in source ?? System.Linq.Enumerable.Empty<TSource>())
        {
            dict[keySelector(item)] = valueSelector(item);
        }
        return dict;
    }

    /// <summary>
    /// Conditionally adds an item to a list and returns the same list instance for chaining.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list to modify.</param>
    /// <param name="condition">If <see langword="true"/>, the item is added; otherwise the list is unchanged.</param>
    /// <param name="item">The item to add if the condition is <see langword="true"/>.</param>
    /// <returns>The same <paramref name="list"/> instance for method chaining.</returns>
    /// <example>
    /// <code>
    /// var list = new List&lt;int&gt; { 1, 2 };
    /// list.AddIf(true, 3);   // list is now [1, 2, 3]
    /// list.AddIf(false, 4);  // list is still [1, 2, 3]
    /// </code>
    /// </example>
    public static IList<T> AddIf<T>(this IList<T> list, bool condition, T item)
    {
        if (condition)
        {
            list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// Conditionally adds a range of items to a list and returns the same list instance for chaining.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list to modify.</param>
    /// <param name="condition">If <see langword="true"/>, the items are added; otherwise the list is unchanged.</param>
    /// <param name="items">The items to add if the condition is <see langword="true"/>.
    /// If <see langword="null"/>, treated as empty and no items are added.</param>
    /// <returns>The same <paramref name="list"/> instance for method chaining.</returns>
    /// <example>
    /// <code>
    /// var list = new List&lt;int&gt; { 1 };
    /// list.AddRangeIf(true, new[] { 2, 3 });   // list is now [1, 2, 3]
    /// list.AddRangeIf(false, new[] { 4, 5 }); // list is still [1, 2, 3]
    /// </code>
    /// </example>
    public static IList<T> AddRangeIf<T>(this IList<T> list, bool condition, IEnumerable<T> items)
    {
        if (condition)
        {
            foreach (var item in items ?? System.Linq.Enumerable.Empty<T>())
            {
                list.Add(item);
            }
        }
        return list;
    }

    /// <summary>
    /// Concatenates another sequence to the source sequence if a condition is <see langword="true"/>,
    /// otherwise returns the source sequence unchanged.
    /// If the source sequence is <see langword="null"/>, an empty sequence is used.
    /// If the other sequence is <see langword="null"/>, an empty sequence is used.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence. If <see langword="null"/>, treated as empty.</param>
    /// <param name="condition">If <see langword="true"/>, <paramref name="other"/> is concatenated; otherwise only <paramref name="source"/> is returned.</param>
    /// <param name="other">The sequence to concatenate if the condition is <see langword="true"/>. If <see langword="null"/>, treated as empty.</param>
    /// <returns>
    /// If <paramref name="condition"/> is <see langword="true"/>, returns <paramref name="source"/> concatenated with <paramref name="other"/>.
    /// Otherwise, returns <paramref name="source"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// new[] { 1, 2 }.ConcatIf(true, new[] { 3, 4 })    // [1, 2, 3, 4]
    /// new[] { 1, 2 }.ConcatIf(false, new[] { 3, 4 })   // [1, 2]
    /// ((IEnumerable&lt;int&gt;)null).ConcatIf(true, new[] { 1, 2 })   // [1, 2]
    /// ((IEnumerable&lt;int&gt;)null).ConcatIf(false, new[] { 1, 2 })  // empty
    /// </code>
    /// </example>
    public static IEnumerable<T> ConcatIf<T>(
        this IEnumerable<T> source,
        bool condition,
        IEnumerable<T> other
    )
    {
        var first = source ?? System.Linq.Enumerable.Empty<T>();
        return condition ? first.Concat(other ?? System.Linq.Enumerable.Empty<T>()) : first;
    }

    /// <summary>
    /// Splits a sequence into two lists based on a predicate: elements that match go into
    /// <c>Matched</c>, all others go into <c>Rest</c>.
    /// Returns two empty lists if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to partition. If <see langword="null"/>, treated as empty.</param>
    /// <param name="predicate">A function that returns <see langword="true"/> for elements to include in <c>Matched</c>.</param>
    /// <returns>
    /// A tuple of two read-only lists: <c>Matched</c> containing elements that satisfy the predicate,
    /// and <c>Rest</c> containing all other elements, both in original order.
    /// </returns>
    public static (IReadOnlyList<T> Matched, IReadOnlyList<T> Remaining) Partition<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate
    )
    {
        var matched = new List<T>();
        var rest = new List<T>();
        foreach (var item in source ?? System.Linq.Enumerable.Empty<T>())
        {
            if (predicate(item))
            {
                matched.Add(item);
            }
            else
            {
                rest.Add(item);
            }
        }
        return (matched, rest);
    }

    /// <summary>
    /// Splits a sequence into chunks of at most <paramref name="size"/> elements each.
    /// The last chunk may contain fewer elements if the sequence length is not evenly divisible.
    /// Returns an empty sequence if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to batch. If <see langword="null"/>, treated as empty.</param>
    /// <param name="size">The maximum number of elements per chunk.</param>
    /// <returns>A sequence of read-only lists, each containing at most <paramref name="size"/> elements.</returns>
    public static IEnumerable<IReadOnlyList<T>> Batch<T>(this IEnumerable<T> source, int size) =>
        (source ?? System.Linq.Enumerable.Empty<T>()).Chunk(size).Select(c => (IReadOnlyList<T>)c);

    /// <summary>
    /// Returns the element with the smallest key value, or <see langword="default"/>(<typeparamref name="T"/>)
    /// if the source is <see langword="null"/>.
    /// For empty sequences, returns <see langword="default"/>(<typeparamref name="T"/>).
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TKey">The key type used for comparison.</typeparam>
    /// <param name="source">The sequence to search. If <see langword="null"/>, <see langword="default"/>(<typeparamref name="T"/>) is returned.</param>
    /// <param name="keySelector">A function to extract the comparison key from each element.</param>
    /// <returns>The element with the smallest key, or <see langword="default"/>(<typeparamref name="T"/>) if source is <see langword="null"/> or empty.</returns>
    /// <example>
    /// <code>
    /// new[] { 3, 1, 2 }.MinByOrDefault(x => x)  // 1
    /// ((IEnumerable&lt;int&gt;)null).MinByOrDefault(x => x)  // 0 (default for int)
    /// System.Linq.Enumerable.Empty&lt;string&gt;().MinByOrDefault(x => x)  // null
    /// </code>
    /// </example>
    public static T? MinByOrDefault<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector
    ) => source is null ? default : source.MinBy(keySelector);

    /// <summary>
    /// Returns the element with the largest key value, or <see langword="default"/>(<typeparamref name="T"/>)
    /// if the source is <see langword="null"/>.
    /// For empty sequences, returns <see langword="default"/>(<typeparamref name="T"/>).
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TKey">The key type used for comparison.</typeparam>
    /// <param name="source">The sequence to search. If <see langword="null"/>, <see langword="default"/>(<typeparamref name="T"/>) is returned.</param>
    /// <param name="keySelector">A function to extract the comparison key from each element.</param>
    /// <returns>The element with the largest key, or <see langword="default"/>(<typeparamref name="T"/>) if source is <see langword="null"/> or empty.</returns>
    /// <example>
    /// <code>
    /// new[] { 3, 1, 2 }.MaxByOrDefault(x => x)  // 3
    /// ((IEnumerable&lt;int&gt;)null).MaxByOrDefault(x => x)  // 0 (default for int)
    /// System.Linq.Enumerable.Empty&lt;string&gt;().MaxByOrDefault(x => x)  // null
    /// </code>
    /// </example>
    public static T? MaxByOrDefault<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector
    ) => source is null ? default : source.MaxBy(keySelector);

    /// <summary>
    /// Projects each element of a sequence to a <typeparamref name="TResult"/> using an async selector,
    /// running projections concurrently and collecting all results into a read-only list.
    /// Returns an empty list if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type of the input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the projected result.</typeparam>
    /// <param name="source">The sequence to project. If <see langword="null"/>, returns an empty list.</param>
    /// <param name="selector">An async function to apply to each element.</param>
    /// <param name="maxParallel">
    /// Optional cap on the number of concurrent async operations.
    /// When <see langword="null"/> (the default), all projections are started concurrently.
    /// </param>
    /// <returns>
    /// A <see cref="Task{T}"/> that completes with an <see cref="IReadOnlyList{TResult}"/> containing
    /// all projected results in source order.
    /// </returns>
    public static async Task<IReadOnlyList<TResult>> SelectAsync<T, TResult>(
        this IEnumerable<T> source,
        Func<T, Task<TResult>> selector,
        int? maxParallel = null
    )
    {
        if (source is null)
        {
            return [];
        }

        if (maxParallel is null)
        {
            return await Task.WhenAll(source.Select(selector));
        }

        using var semaphore = new SemaphoreSlim(maxParallel.Value);
        var tasks = source.Select(async item =>
        {
            await semaphore.WaitAsync();
            try
            {
                return await selector(item);
            }
            finally
            {
                semaphore.Release();
            }
        });
        return await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Awaits all tasks in the sequence and returns the results as an <see cref="IReadOnlyList{T}"/>.
    /// Returns an empty list if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The result type of each task.</typeparam>
    /// <param name="tasks">The sequence of tasks to await. If <see langword="null"/>, returns an empty list.</param>
    /// <returns>
    /// A <see cref="Task{T}"/> that completes with an <see cref="IReadOnlyList{T}"/> containing all task results.
    /// </returns>
    public static async Task<IReadOnlyList<T>> WhenAllList<T>(this IEnumerable<Task<T>> tasks) =>
        await Task.WhenAll(tasks ?? []);
}
