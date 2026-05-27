using System;
using System.Collections.Generic;
using System.Linq;

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
    NoOrder
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
    public static bool AreEqual<T>(this IEnumerable<T> enumerable, IEnumerable<T> values,
    Compare comparison = Compare.NoOrder)
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
            _ => false
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
        }).ToList();
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

        return values;
    }

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
    public static TOut Reduce<TIn, TOut>(this IEnumerable<TIn> values, Func<TIn, TOut, TOut> execute, TOut initialValue = default)
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
    public static TOut Reduce<TIn, TOut>(this IEnumerable<TIn> values, Func<TIn, TOut, int, TOut> execute, TOut initialValue = default)
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
}

