using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable enable
namespace CSharpHelperExtensions.Dictionaries;

public static class DictionaryExtensions
{
    /// <summary>
    /// Returns the value for <paramref name="key"/> if it exists; otherwise invokes
    /// <paramref name="factory"/>, adds the result to the dictionary, and returns it.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="dict">The dictionary to operate on.</param>
    /// <param name="key">The key to look up or add.</param>
    /// <param name="factory">A function that produces the value when the key is missing. Receives the key as its argument.</param>
    /// <returns>The existing or newly added value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="factory"/> is <see langword="null"/>.</exception>
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        TKey key,
        Func<TKey, TValue> factory)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(factory);

        if (dict.TryGetValue(key, out var existing))
            return existing;

        var value = factory(key);
        dict[key] = value;
        return value;
    }
}
