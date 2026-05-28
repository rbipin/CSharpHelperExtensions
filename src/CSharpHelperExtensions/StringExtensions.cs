using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpHelperExtensions.Strings;

public static class StringExtensions
{
    private static readonly Regex CollapseRegex = new(@"\s+", RegexOptions.Compiled);
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

    /// <summary>Converts <paramref name="input"/> to its UTF-8 byte representation.</summary>
    /// <param name="input">The string to encode. Returns an empty array when <see langword="null"/>.</param>
    /// <returns>UTF-8 encoded bytes.</returns>
    public static byte[] ToUtf8Bytes(this string input)
        => Encoding.UTF8.GetBytes(input ?? string.Empty);

    /// <summary>Converts <paramref name="input"/> to a seekable <see cref="MemoryStream"/> using UTF-8 encoding.</summary>
    /// <param name="input">The string to stream. Returns an empty stream when <see langword="null"/>.</param>
    /// <returns>A <see cref="MemoryStream"/> containing the UTF-8 bytes.</returns>
    public static MemoryStream ToUtf8Stream(this string input)
        => new(Encoding.UTF8.GetBytes(input ?? string.Empty));

    /// <summary>
    /// Joins <paramref name="values"/> using <paramref name="separator"/> as the delimiter.
    /// Fluent wrapper around <see cref="string.Join(string, IEnumerable{string})"/>.
    /// </summary>
    /// <param name="separator">The separator string (this). A <see langword="null"/> separator is treated as empty.</param>
    /// <param name="values">The strings to join.</param>
    /// <returns>The joined string.</returns>
    public static string JoinWith(this string separator, IEnumerable<string> values)
        => string.Join(separator ?? string.Empty, values ?? []);

    /// <summary>
    /// Splits <paramref name="input"/> on <paramref name="separators"/> and removes empty entries.
    /// Returns an empty array when <paramref name="input"/> is <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="input">The string to split.</param>
    /// <param name="separators">One or more separator characters.</param>
    /// <returns>Non-empty segments after splitting.</returns>
    public static string[] SplitNonEmpty(this string input, params char[] separators)
    {
        if (input.IsNullOrEmpty()) return [];
        return input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>Removes all whitespace characters from <paramref name="input"/>.</summary>
    /// <param name="input">The string to process. Returns <see cref="string.Empty"/> when <see langword="null"/>.</param>
    /// <returns>The string with all whitespace removed.</returns>
    public static string RemoveWhitespace(this string input)
    {
        if (input == null) return string.Empty;
        return string.Concat(input.Where(c => !char.IsWhiteSpace(c)));
    }

    /// <summary>
    /// Trims leading/trailing whitespace and collapses all internal whitespace runs to a single space.
    /// </summary>
    /// <param name="input">The string to process. Returns <see cref="string.Empty"/> when <see langword="null"/> or whitespace.</param>
    /// <returns>The collapsed string.</returns>
    public static string CollapseWhitespace(this string input)
    {
        if (input.IsNullOrEmpty()) return string.Empty;
        return CollapseRegex.Replace(input.Trim(), " ");
    }

    /// <summary>
    /// Applies a sequence of find-and-replace operations to <paramref name="input"/> in order.
    /// </summary>
    /// <param name="input">The string to modify. Returns <see cref="string.Empty"/> when <see langword="null"/>.</param>
    /// <param name="pairs">Replacement pairs: each <c>OldValue</c> is replaced by <c>NewValue</c>, applied sequentially.</param>
    /// <returns>The resulting string after all replacements.</returns>
    public static string ReplaceMany(this string input, IEnumerable<(string OldValue, string NewValue)> pairs)
    {
        if (input == null) return string.Empty;
        var result = input;
        foreach (var (oldValue, newValue) in pairs)
            result = result.Replace(oldValue, newValue);
        return result;
    }

    /// <summary>
    /// Removes diacritical marks (accent characters) from <paramref name="input"/>.
    /// For example, <c>"café"</c> becomes <c>"cafe"</c>.
    /// </summary>
    /// <param name="input">The string to process. Returns <see cref="string.Empty"/> when <see langword="null"/> or whitespace.</param>
    /// <returns>The string with diacritics removed.</returns>
    public static string RemoveDiacritics(this string input)
    {
        if (input.IsNullOrEmpty()) return string.Empty;
        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="input"/> is non-empty and all characters are digits (0–9).</summary>
    /// <param name="input">The string to test.</param>
    public static bool IsNumeric(this string input)
        => !input.IsNullOrEmpty() && input.All(char.IsDigit);

    /// <summary>Returns <see langword="true"/> if <paramref name="input"/> is non-empty and all characters are Unicode letters.</summary>
    /// <param name="input">The string to test.</param>
    public static bool IsAlpha(this string input)
        => !input.IsNullOrEmpty() && input.All(char.IsLetter);

    /// <summary>Returns <see langword="true"/> if <paramref name="input"/> is non-empty and all characters are Unicode letters or digits.</summary>
    /// <param name="input">The string to test.</param>
    public static bool IsAlphaNumeric(this string input)
        => !input.IsNullOrEmpty() && input.All(char.IsLetterOrDigit);

    /// <summary>
    /// Converts <paramref name="input"/> to a URL-friendly slug: lowercase, diacritics removed,
    /// non-alphanumeric characters replaced with a single dash.
    /// </summary>
    /// <param name="input">The string to slugify. Returns <see cref="string.Empty"/> when <see langword="null"/> or whitespace.</param>
    /// <returns>The slug string, e.g. <c>"Hello World!"</c> → <c>"hello-world"</c>.</returns>
    public static string ToSlug(this string input)
    {
        if (input.IsNullOrEmpty()) return string.Empty;
        var clean = input.RemoveDiacritics().ToLowerInvariant();
        var sb = new StringBuilder(clean.Length);
        var lastWasDash = false;
        foreach (var c in clean)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
                lastWasDash = false;
            }
            else if (!lastWasDash && sb.Length > 0)
            {
                sb.Append('-');
                lastWasDash = true;
            }
        }
        if (sb.Length > 0 && sb[^1] == '-')
            sb.Length--;
        return sb.ToString();
    }
}
