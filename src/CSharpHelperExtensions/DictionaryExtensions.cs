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

    /// <summary>
    /// Merges all entries from <paramref name="other"/> into this dictionary.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="dict">The target dictionary.</param>
    /// <param name="other">The source dictionary to merge from. A <see langword="null"/> value is silently ignored.</param>
    /// <param name="overwrite">When <see langword="true"/>, existing keys are overwritten. Defaults to <see langword="false"/> (skip duplicates).</param>
    /// <returns>The original <paramref name="dict"/> for fluent chaining.</returns>
    public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        IDictionary<TKey, TValue>? other,
        bool overwrite = false)
    {
        if (other is null)
            return dict;

        foreach (var pair in other)
        {
            if (overwrite || !dict.ContainsKey(pair.Key))
                dict[pair.Key] = pair.Value;
        }

        return dict;
    }

    /// <summary>
    /// Bulk-adds <paramref name="pairs"/> into this dictionary.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="dict">The target dictionary.</param>
    /// <param name="pairs">The key-value pairs to add. A <see langword="null"/> value is silently ignored.</param>
    /// <param name="overwrite">When <see langword="true"/>, existing keys are overwritten. Defaults to <see langword="false"/> (skip duplicates).</param>
    /// <returns>The original <paramref name="dict"/> for fluent chaining.</returns>
    public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        IEnumerable<KeyValuePair<TKey, TValue>>? pairs,
        bool overwrite = false)
    {
        if (pairs is null)
            return dict;

        foreach (var pair in pairs)
        {
            if (overwrite || !dict.ContainsKey(pair.Key))
                dict[pair.Key] = pair.Value;
        }

        return dict;
    }

    /// <summary>
    /// Removes all entries whose key satisfies <paramref name="predicate"/>.
    /// Mutates the dictionary in-place.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="dict">The dictionary to filter.</param>
    /// <param name="predicate">A function that returns <see langword="true"/> for keys to remove.</param>
    /// <returns>The original <paramref name="dict"/> for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static IDictionary<TKey, TValue> RemoveWhere<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        Func<TKey, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var keysToRemove = new List<TKey>();
        foreach (var key in dict.Keys)
        {
            if (predicate(key))
                keysToRemove.Add(key);
        }

        foreach (var key in keysToRemove)
            dict.Remove(key);

        return dict;
    }

    /// <summary>
    /// Wraps this dictionary in a read-only view.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="dict">The dictionary to wrap.</param>
    /// <returns>
    /// A <see cref="System.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/> that reflects
    /// subsequent mutations to the underlying dictionary (live view, not a copy).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dict"/> is <see langword="null"/>.</exception>
    public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
        this IDictionary<TKey, TValue> dict)
    {
        ArgumentNullException.ThrowIfNull(dict);
        return new ReadOnlyDictionary<TKey, TValue>(dict);
    }
}
