using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
        if (enumerable.Count() != value.Count())
        {
            return false;
        }
        return value.All(item => enumerable.Contains(item));
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
        values ??= new List<T>();
        enumerable ??= new List<T>();
        if (ReferenceEquals(enumerable, values))
        {
            return true;
        }
        if (values.Count() != enumerable.Count())
        {
            return false;
        }
        return comparison switch
        {
            Compare.InOrder => CompareItemsInOrder(enumerable, values),
            Compare.NoOrder => enumerable.All(item => values.Contains(item)),
            _ => false,
        };
    }

    private static bool CompareItemsInOrder<T>(IEnumerable<T> list1, IEnumerable<T> list2)
    {
        var firstList = list1.ToList();
        var secondList = list2.ToList();
        for (int index = 0; index <= firstList.Count - 1; index++)
        {
            var areEqual = firstList[index].Equals(secondList[index]);
            if (!areEqual)
            {
                return false;
            }
        }
        return true;
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
        var list = value?.ToList();
        if (list is null || !list.Any())
        {
            return null;
        }

        return list.Where(item =>
            {
                if (item is string itemStr)
                {
                    return !string.IsNullOrWhiteSpace(itemStr);
                }

                return item is not null;
            })
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
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> values)
    {
        var enumerable = values?.ToArray();
        return enumerable == null || !enumerable.Any() || enumerable.All(item => item is null);
    }

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
    public static Task ForEach<T>(this IEnumerable<T> values, Func<T, Task> execute)
    {
        var collection = values?.ToList() ?? new List<T>();
        return Task.WhenAll(collection.Select(item => execute(item)));
    }

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
        var collection = values?.ToList() ?? new List<T>();
        foreach (var item in collection)
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
    /// If <see langword="null"/> or empty, <paramref name="initialValue"/> is ignored and
    /// <see langword="default"/>(<typeparamref name="TOut"/>) is returned.
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
        TOut initialValue = default
    )
    {
        var collection = values?.ToList() ?? new List<TIn>();
        var result = default(TOut);
        var temp = initialValue;
        foreach (var item in collection)
        {
            result = execute(item, temp);
            temp = result;
        }

        return result;
    }

    /// <summary>
    /// Reduces a sequence to a single accumulated value by repeatedly applying a reducer function
    /// that also receives the current element's zero-based index.
    /// </summary>
    /// <param name="values">
    /// The sequence to reduce.
    /// If <see langword="null"/> or empty, <paramref name="initialValue"/> is ignored and
    /// <see langword="default"/>(<typeparamref name="TOut"/>) is returned.
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
        TOut initialValue = default
    )
    {
        var collection = values?.ToList() ?? new List<TIn>();
        var result = default(TOut);
        var temp = initialValue;
        for (int counter = 0; counter <= collection.Count - 1; counter++)
        {
            var item = collection[counter];
            result = execute(item, temp, counter);
            temp = result;
        }

        return result;
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
    public static bool HasAny<T>(this IEnumerable<T> source)
        => source != null && source.Any();

    /// <summary>
    /// Returns the sequence unchanged if non-null, or an empty sequence if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to evaluate.</param>
    /// <returns>
    /// <paramref name="source"/> if it is not <see langword="null"/>; otherwise <see cref="Enumerable.Empty{T}"/>.
    /// </returns>
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
        => source ?? System.Linq.Enumerable.Empty<T>();

    /// <summary>
    /// Returns <see langword="true"/> if the sequence is <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to check.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="source"/> is <see langword="null"/> or empty;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool None<T>(this IEnumerable<T> source)
        => source is null || !source.Any();

    /// <summary>
    /// Filters out <see langword="null"/> elements from a sequence of reference types.
    /// Returns an empty sequence if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type (must be a reference type).</typeparam>
    /// <param name="source">The sequence to filter.</param>
    /// <returns>A sequence containing only non-null elements.</returns>
#nullable enable
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
        => source is null ? System.Linq.Enumerable.Empty<T>() : source.Where(x => x is not null).Cast<T>();
#nullable restore

    /// <summary>
    /// Materializes the sequence into an <see cref="IReadOnlyList{T}"/>, preserving order.
    /// Returns an empty list if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to materialize.</param>
    /// <returns>An <see cref="IReadOnlyList{T}"/> containing all elements in order.</returns>
    public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> source)
        => (source ?? System.Linq.Enumerable.Empty<T>()).ToList();

    /// <summary>
    /// Converts the sequence to a <see cref="HashSet{T}"/>, deduplicating elements.
    /// Returns an empty set if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to convert.</param>
    /// <returns>A <see cref="HashSet{T}"/> containing the distinct elements.</returns>
    public static HashSet<T> ToHashSetSafe<T>(this IEnumerable<T> source)
        => source is null ? new HashSet<T>() : source.ToHashSet();

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
    public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
        => string.Join(separator, source ?? System.Linq.Enumerable.Empty<T>());

    /// <summary>
    /// Projects each element of a sequence into a tuple of its zero-based index and the element itself.
    /// Returns an empty sequence if the source is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The sequence to index.</param>
    /// <returns>A sequence of <c>(Index, Item)</c> tuples.</returns>
    public static IEnumerable<(int Index, T Item)> WithIndex<T>(this IEnumerable<T> source)
        => (source ?? System.Linq.Enumerable.Empty<T>()).Select((item, i) => (i, item));

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
        Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var item in source ?? System.Linq.Enumerable.Empty<TSource>())
            dict[keySelector(item)] = valueSelector(item);
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
        if (condition) list.Add(item);
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
            foreach (var item in items ?? System.Linq.Enumerable.Empty<T>())
                list.Add(item);
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
        this IEnumerable<T> source, bool condition, IEnumerable<T> other)
    {
        var first = source ?? System.Linq.Enumerable.Empty<T>();
        return condition ? first.Concat(other ?? System.Linq.Enumerable.Empty<T>()) : first;
    }
}
