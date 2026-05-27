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
        var conv = TypeDescriptor.GetConverter(typeof(T));
        return (T?)conv.ConvertFrom(input);
    }

    public static bool HasValue(this string input) => !string.IsNullOrWhiteSpace(input);

    public static string OrEmpty(this string input) => input ?? string.Empty;

    public static string OrDefault(this string input, string fallback)
        => string.IsNullOrWhiteSpace(input) ? fallback : input;
}
