using System;
using System.ComponentModel;
using System.Text;

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

    /// <summary>Trims whitespace then converts to lowercase.</summary>
    /// <param name="input">The string to transform. Accepts <see langword="null"/>.</param>
    /// <returns>The trimmed, lowercased string, or <see cref="string.Empty"/> if <paramref name="input"/> is <see langword="null"/>.</returns>
    public static string TrimToLower(this string input)
        => input?.Trim().ToLowerInvariant() ?? string.Empty;

    /// <summary>Trims whitespace then converts to uppercase.</summary>
    /// <param name="input">The string to transform. Accepts <see langword="null"/>.</param>
    /// <returns>The trimmed, upper-cased string, or <see cref="string.Empty"/> if <paramref name="input"/> is <see langword="null"/>.</returns>
    public static string TrimToUpper(this string input)
        => input?.Trim().ToUpperInvariant() ?? string.Empty;

    /// <summary>Returns <see langword="true"/> if both strings are equal using ordinal case-insensitive comparison.</summary>
    /// <param name="input">The source string.</param>
    /// <param name="other">The string to compare to.</param>
    public static bool EqualsIgnoreCase(this string input, string other)
        => string.Equals(input, other, StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns <see langword="true"/> if <paramref name="input"/> contains <paramref name="value"/> using ordinal case-insensitive comparison.</summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="value">The substring to search for.</param>
    public static bool ContainsIgnoreCase(this string input, string value)
    {
        if (input == null || value == null) return false;
        return input.Contains(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="input"/> starts with <paramref name="value"/> using ordinal case-insensitive comparison.</summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="value">The prefix to look for.</param>
    public static bool StartsWithIgnoreCase(this string input, string value)
    {
        if (input == null || value == null) return false;
        return input.StartsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="input"/> ends with <paramref name="value"/> using ordinal case-insensitive comparison.</summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="value">The suffix to look for.</param>
    public static bool EndsWithIgnoreCase(this string input, string value)
    {
        if (input == null || value == null) return false;
        return input.EndsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Masks all but the last <paramref name="visibleCount"/> characters of <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The string to mask. Accepts <see langword="null"/>.</param>
    /// <param name="visibleCount">Number of characters to leave visible at the end.</param>
    /// <param name="maskChar">Character used for masking. Defaults to <c>'*'</c>.</param>
    /// <returns>The masked string, or <see cref="string.Empty"/> if <paramref name="input"/> is <see langword="null"/> or empty.</returns>
    /// <example>
    /// <code>
    /// "123456".MaskStart(2)       // "****56"
    /// "123456".MaskStart(2, '#')  // "####56"
    /// "AB".MaskStart(2)           // "AB"  (nothing to mask)
    /// </code>
    /// </example>
    public static string MaskStart(this string input, int visibleCount, char maskChar = '*')
    {
        if (input.IsNullOrEmpty()) return string.Empty;
        if (visibleCount >= input.Length) return input;
        var maskLength = input.Length - visibleCount;
        return new string(maskChar, maskLength) + input[maskLength..];
    }

    /// <summary>Parses <paramref name="input"/> as <see cref="int"/>. Returns <see langword="null"/> if the string is not a valid integer.</summary>
    /// <param name="input">The string to parse.</param>
    public static int? ToIntOrNull(this string input)
        => int.TryParse(input, out var v) ? v : null;

    /// <summary>Parses <paramref name="input"/> as <see cref="decimal"/>. Returns <see langword="null"/> if the string is not a valid decimal number.</summary>
    /// <param name="input">The string to parse.</param>
    public static decimal? ToDecimalOrNull(this string input)
        => decimal.TryParse(input, out var v) ? v : null;

    /// <summary>Parses <paramref name="input"/> as <see cref="DateTime"/>. Returns <see langword="null"/> if the string is not a valid date/time.</summary>
    /// <param name="input">The string to parse.</param>
    public static DateTime? ToDateTimeOrNull(this string input)
        => DateTime.TryParse(input, out var v) ? v : null;

    /// <summary>Parses <paramref name="input"/> as <see cref="Guid"/>. Returns <see langword="null"/> if the string is not a valid GUID.</summary>
    /// <param name="input">The string to parse.</param>
    public static Guid? ToGuidOrNull(this string input)
        => Guid.TryParse(input, out var v) ? v : null;

    /// <summary>
    /// Parses <paramref name="input"/> as <see cref="bool"/>. Returns <see langword="null"/> if the string is not a valid boolean
    /// (<c>"true"</c> and <c>"false"</c> are accepted, case-insensitive; other values return <see langword="null"/>).
    /// </summary>
    /// <param name="input">The string to parse.</param>
    public static bool? ToBoolOrNull(this string input)
        => bool.TryParse(input, out var v) ? v : null;

    /// <summary>Encodes <paramref name="input"/> as a standard Base64 string using UTF-8 encoding.</summary>
    /// <param name="input">The string to encode.</param>
    /// <returns>The Base64-encoded string, or <see langword="null"/> if <paramref name="input"/> is <see langword="null"/>.</returns>
    public static string Base64Encode(this string input)
    {
        if (input == null) return null;
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }

    /// <summary>Decodes a standard Base64 string back to a UTF-8 string.</summary>
    /// <param name="input">The Base64-encoded string to decode.</param>
    /// <returns>The decoded string, or <see langword="null"/> if <paramref name="input"/> is <see langword="null"/>.</returns>
    public static string Base64Decode(this string input)
    {
        if (input == null) return null;
        return Encoding.UTF8.GetString(Convert.FromBase64String(input));
    }

    /// <summary>
    /// Encodes <paramref name="input"/> as a URL-safe Base64 string (no <c>+</c>, <c>/</c>, or <c>=</c> padding).
    /// Use <see cref="FromBase64Url"/> to decode.
    /// </summary>
    /// <param name="input">The string to encode.</param>
    /// <returns>The URL-safe Base64 string, or <see langword="null"/> if <paramref name="input"/> is <see langword="null"/>.</returns>
    public static string ToBase64Url(this string input)
    {
        if (input == null) return null;
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(input))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    /// <summary>
    /// Decodes a URL-safe Base64 string (produced by <see cref="ToBase64Url"/>) back to a UTF-8 string.
    /// </summary>
    /// <param name="input">The URL-safe Base64 string to decode.</param>
    /// <returns>The decoded string, or <see langword="null"/> if <paramref name="input"/> is <see langword="null"/>.</returns>
    public static string FromBase64Url(this string input)
    {
        if (input == null) return null;
        var padded = input.Replace('-', '+').Replace('_', '/');
        padded = (padded.Length % 4) switch
        {
            2 => padded + "==",
            3 => padded + "=",
            _ => padded
        };
        return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
    }
}
