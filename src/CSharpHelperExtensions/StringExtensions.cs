using System;
using System.ComponentModel;

namespace CSharpHelperExtensions.Strings;

public static class StringExtensions
{
    /// <summary>
    /// Converts a string to the specified nullable value type using its registered <see cref="TypeConverter"/>.
    /// Returns <see langword="null"/> when the input is <see langword="null"/>, empty, or whitespace.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <typeparam name="T">
    /// The target value type (e.g. <see cref="int"/>, <see cref="DateTime"/>, <see cref="Guid"/>).
    /// Must be a struct; the return type is <typeparamref name="T"/>?.
    /// </typeparam>
    /// <returns>
    /// The converted <typeparamref name="T"/> value wrapped in a nullable,
    /// or <see langword="null"/> if <paramref name="input"/> is <see langword="null"/>, empty, or whitespace.
    /// </returns>
    /// <exception cref="Exception">
    /// Rethrows any exception thrown by the underlying <see cref="TypeConverter"/> when the string
    /// cannot be parsed as <typeparamref name="T"/> (e.g. <see cref="FormatException"/>
    /// or <see cref="NotSupportedException"/>).
    /// </exception>
    /// <example>
    /// <code>
    /// "42".ToNullable&lt;int&gt;()                  // (int?)42
    /// "".ToNullable&lt;int&gt;()                    // null
    /// "   ".ToNullable&lt;int&gt;()                 // null (whitespace)
    /// "2024-01-15".ToNullable&lt;DateTime&gt;()    // (DateTime?)new DateTime(2024, 1, 15)
    /// "not-a-number".ToNullable&lt;int&gt;()       // throws FormatException
    /// </code>
    /// </example>
    public static T? ToNullable<T>(this string input) where T : struct
    {
        try
        {
            if (input.IsNullOrEmpty())
            {
                return null;
            }

            var conv = TypeDescriptor.GetConverter(typeof(T));
            return (T?)conv.ConvertFrom(input);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

