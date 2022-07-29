using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace DryExtensions
{
    public enum BetweenComparison
    {
        None,
        ExcludeBoth,
        ExcludeLower,
        ExcludeUpper
    }
    public static class GenericExtensions
    {

        /// <summary>
        /// Check if a value is in between two comparable values
        /// default comparison type is None and it will include the lower and upper bounds in the comparison
        /// </summary>
        /// <param name="value">value to compare</param>
        /// <param name="lower">lower bound value</param>
        /// <param name="upper">upper bound value</param>
        /// <param name="comparison"></param>
        /// <typeparam name="T">type</typeparam>
        /// <returns>true/ false</returns>
        public static bool IsBetween<T>(this T value, T lower, T upper, BetweenComparison comparison = BetweenComparison.None)
            where T : IComparable<T>
        {
            return comparison switch
            {
                BetweenComparison.ExcludeBoth => (value.CompareTo(lower) > 0) && (value.CompareTo(upper) < 0),
                BetweenComparison.ExcludeLower => (value.CompareTo(lower) > 0) && (value.CompareTo(upper) <= 0),
                BetweenComparison.ExcludeUpper => (value.CompareTo(lower) >= 0) && (value.CompareTo(upper) < 0),
                _ => (lower.CompareTo(value) <= 0) && (value.CompareTo(upper) <= 0)
            };
        }

        /// <summary>
        /// Check if the value is part of a list of items
        /// </summary>
        /// <param name="value">Item to check</param>
        /// <param name="input">Item to check against</param>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>true or false</returns>
        public static bool In<T>(this T value, params T[] input)
        {
            return input is { } && input.Contains(value);
        }

        /// <summary>
        /// Check if a string is null, empty or has whitespace
        /// By default the 
        /// </summary>
        /// <param name="value">string to check</param>
        /// <param name="checkForWhitespace">Should consider whitespace as empty or not, default is true</param>
        /// <returns>true or false</returns>
        public static bool IsNullOrEmpty(this string value, bool checkForWhitespace = true)
        {
            if (checkForWhitespace)
            {
                value = value?.Trim();
            }

            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Convert an object to JSON representation
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="indentation">add indentation or not</param>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>JSON String</returns>
        public static string ToJson<T>(this T value, bool indentation = false) where T : class
        {
            var formatting = indentation ? Formatting.Indented : Formatting.None;
            return value == null ? null : JsonConvert.SerializeObject(value, formatting);
        }
    }
}