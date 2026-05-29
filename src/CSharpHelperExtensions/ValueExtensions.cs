using System;
using System.Linq;
using Newtonsoft.Json;

namespace CSharpHelperExtensions.Values;

/// <summary>
/// Controls which bounds are included when using <see cref="ValueExtensions.IsBetween{T}"/>.
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
public static class ValueExtensions
{

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
    public static bool IsBetween<T>(this T value, T lower, T upper, BetweenComparison comparison = BetweenComparison.None)
        where T : IComparable<T>
    {
        return comparison switch
        {
            BetweenComparison.ExcludeBoth => (value.CompareTo(lower) > 0) && (value.CompareTo(upper) < 0),
            BetweenComparison.ExcludeLower => (value.CompareTo(lower) > 0) && (value.CompareTo(upper) <= 0),
            BetweenComparison.ExcludeUpper => (value.CompareTo(lower) >= 0) && (value.CompareTo(upper) < 0),
            _ => value.CompareTo(lower) >= 0 && value.CompareTo(upper) <= 0
        };
    }

    /// <summary>
    /// Determines whether a value equals any item in a given set.
    /// Equivalent to SQL's <c>IN</c> operator.
    /// </summary>
    /// <param name="value">The value to look for.</param>
    /// <param name="input">One or more candidate values to match against.</param>
    /// <remarks>
    /// Returns <see langword="false"/> when no candidates are supplied.
    /// Equality is determined by the default equality comparer for <typeparamref name="T"/>.
    /// </remarks>
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
    public static bool In<T>(this T value, params T[] input)
    {
        return input.Contains(value);
    }

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
    /// 42.ToJson()                    // 42
    /// </code>
    /// </example>
    public static string ToJson<T>(this T value, bool indentation = false)
    {
        var formatting = indentation ? Formatting.Indented : Formatting.None;
        return value is null ? null : JsonConvert.SerializeObject(value, formatting);
    }
}

