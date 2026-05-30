using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable enable
namespace CSharpHelperExtensions.Dictionaries;

public static class DictionaryExtensions
{
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
