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
