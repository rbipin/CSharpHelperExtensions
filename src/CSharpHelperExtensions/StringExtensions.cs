using System;
using System.ComponentModel;

namespace CSharpHelperExtensions.Strings;

public static class StringExtensions
{
    /// <summary>
    /// Converts a string to the specified nullable value type using its registered <see cref="TypeConverter"/>.
    /// Returns <see langword="null"/> when the input is <see langword="null"/>, empty, or whitespace.
    /// </summary>
    /// <param name="input">The string to convert. Accepts <see langword="null"/>.</param>
    /// <typeparam name="T">
    /// The target value type (e.g. <see cref="int"/>, <see cref="DateTime"/>, <see cref="Guid"/>).
    /// Must be a struct; the return type is <typeparamref name="T"/>?.
    /// </typeparam>
    /// <returns>
    /// The converted <typeparamref name="T"/> value wrapped in a nullable,
    /// or <see langword="null"/> if <paramref name="input"/> is <see langword="null"/>, empty, or whitespace.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown when <paramref name="input"/> is not in a valid format for <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when the <see cref="TypeConverter"/> for <typeparamref name="T"/> does not support
    /// conversion from a string.
    /// </exception>
    /// <example>
    /// <code>
    /// "42".ToNullable&lt;int&gt;()                  // (int?)42
    /// "".ToNullable&lt;int&gt;()                    // null
    /// "   ".ToNullable&lt;int&gt;()                 // null (whitespace)
    /// "2024-01-15".ToNullable&lt;DateTime&gt;()    // parsed DateTime (result is culture-dependent)
    /// "not-a-number".ToNullable&lt;int&gt;()       // throws FormatException
    /// </code>
    /// </example>
    public static T? ToNullable<T>(this string input) where T : struct
    {
        if (input.IsNullOrEmpty())
            return null;
        var converter = TypeDescriptor.GetConverter(typeof(T));
        return (T?)converter.ConvertFrom(input);
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="input"/> is not
    /// <see langword="null"/> or whitespace-only.
    /// Equivalent to <c>!string.IsNullOrWhiteSpace(input)</c>.
    /// </summary>
    /// <param name="input">The string to test.</param>
    /// <returns><see langword="true"/> if the string has content; otherwise <see langword="false"/>.</returns>
    public static bool HasValue(this string input) => !string.IsNullOrWhiteSpace(input);

    /// <summary>
    /// Returns <see cref="string.Empty"/> when <paramref name="input"/> is <see langword="null"/>;
    /// otherwise returns <paramref name="input"/> unchanged (including whitespace-only strings).
    /// Use <see cref="OrDefault"/> if you also want whitespace-only strings replaced.
    /// </summary>
    /// <param name="input">The string to coalesce.</param>
    /// <returns><paramref name="input"/> or <see cref="string.Empty"/> if it was <see langword="null"/>.</returns>
    public static string OrEmpty(this string input) => input ?? string.Empty;

    /// <summary>
    /// Returns <paramref name="fallback"/> when <paramref name="input"/> is <see langword="null"/>
    /// or whitespace-only; otherwise returns <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The string to test.</param>
    /// <param name="fallback">The value to return when <paramref name="input"/> is absent.</param>
    /// <returns><paramref name="input"/> if it has content; otherwise <paramref name="fallback"/>.</returns>
    public static string OrDefault(this string input, string fallback)
        => string.IsNullOrWhiteSpace(input) ? fallback : input;

    /// <summary>
    /// Returns the first <paramref name="maxLength"/> characters of <paramref name="input"/>.
    /// Returns <see cref="string.Empty"/> when <paramref name="input"/> is <see langword="null"/>.
    /// Returns <paramref name="input"/> unchanged if it is shorter than <paramref name="maxLength"/>.
    /// </summary>
    /// <param name="input">The string to truncate.</param>
    /// <param name="maxLength">Maximum number of characters to keep. Must be ≥ 0.</param>
    /// <returns>The truncated string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxLength"/> is negative.</exception>
    public static string Truncate(this string input, int maxLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
        if (input == null) return string.Empty;
        return input.Length <= maxLength ? input : input[..maxLength];
    }

    /// <summary>
    /// Returns the characters of <paramref name="input"/> in reverse order.
    /// Returns <see cref="string.Empty"/> when <paramref name="input"/> is <see langword="null"/> or empty.
    /// </summary>
    /// <param name="input">The string to reverse.</param>
    /// <returns>The reversed string.</returns>
    public static string Reverse(this string input)
    {
        if (input.IsNullOrEmpty()) return string.Empty;
        var chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}
