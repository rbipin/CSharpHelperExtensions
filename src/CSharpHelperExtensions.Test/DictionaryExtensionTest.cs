using System;
using System.Collections.Generic;
using CSharpHelperExtensions.Dictionaries;
using Shouldly;
using Xunit;

namespace CSharpHelperExtensions.Test;

public class DictionaryExtensionTest
{
    [Fact]
    public void GetOrAdd_KeyExists_ReturnsExistingValue()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var factoryCalled = false;
        var result = dict.GetOrAdd("a", _ => { factoryCalled = true; return 99; });
        result.ShouldBe(1);
        factoryCalled.ShouldBeFalse();
    }

    [Fact]
    public void GetOrAdd_KeyMissing_InvokesFactoryAddsAndReturnsValue()
    {
        var dict = new Dictionary<string, int>();
        var result = dict.GetOrAdd("b", key => 42);
        result.ShouldBe(42);
        dict["b"].ShouldBe(42);
    }

    [Fact]
    public void GetOrAdd_NullFactory_Throws()
    {
        var dict = new Dictionary<string, int>();
        Should.Throw<ArgumentNullException>(() => dict.GetOrAdd("a", null!));
    }

    [Fact]
    public void GetOrAdd_NullKey_Throws()
    {
        var dict = new Dictionary<string, int>();
        Should.Throw<ArgumentNullException>(() => dict.GetOrAdd(null!, _ => 42));
    }

    [Fact]
    public void Merge_NoOverlap_AddsAllEntries()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var other = new Dictionary<string, int> { ["b"] = 2 };
        var result = dict.Merge(other);
        result.ShouldBeSameAs(dict);
        dict.Count.ShouldBe(2);
        dict["b"].ShouldBe(2);
    }

    [Fact]
    public void Merge_DuplicateKey_OverwriteFalse_KeepsExisting()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var other = new Dictionary<string, int> { ["a"] = 99 };
        dict.Merge(other, overwrite: false);
        dict["a"].ShouldBe(1);
    }

    [Fact]
    public void Merge_DuplicateKey_OverwriteTrue_ReplacesExisting()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var other = new Dictionary<string, int> { ["a"] = 99 };
        dict.Merge(other, overwrite: true);
        dict["a"].ShouldBe(99);
    }

    [Fact]
    public void Merge_NullOther_NoOp()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var result = dict.Merge(null!);
        result.ShouldBeSameAs(dict);
        dict.Count.ShouldBe(1);
    }

    [Fact]
    public void AddRange_NoOverlap_AddsAllPairs()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var pairs = new List<KeyValuePair<string, int>> { new("b", 2), new("c", 3) };
        var result = dict.AddRange(pairs);
        result.ShouldBeSameAs(dict);
        dict.Count.ShouldBe(3);
    }

    [Fact]
    public void AddRange_DuplicateKey_OverwriteFalse_KeepsExisting()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var pairs = new List<KeyValuePair<string, int>> { new("a", 99) };
        dict.AddRange(pairs, overwrite: false);
        dict["a"].ShouldBe(1);
    }

    [Fact]
    public void AddRange_DuplicateKey_OverwriteTrue_ReplacesExisting()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var pairs = new List<KeyValuePair<string, int>> { new("a", 99) };
        dict.AddRange(pairs, overwrite: true);
        dict["a"].ShouldBe(99);
    }

    [Fact]
    public void AddRange_NullPairs_NoOp()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        var result = dict.AddRange(null!);
        result.ShouldBeSameAs(dict);
        dict.Count.ShouldBe(1);
    }

    [Fact]
    public void RemoveWhere_RemovesMatchingKeys()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
        var result = dict.RemoveWhere(k => k == "a" || k == "c");
        result.ShouldBeSameAs(dict);
        dict.Count.ShouldBe(1);
        dict.ContainsKey("b").ShouldBeTrue();
    }

    [Fact]
    public void RemoveWhere_NoMatch_LeavesAllEntries()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        dict.RemoveWhere(k => k == "z");
        dict.Count.ShouldBe(2);
    }

    [Fact]
    public void RemoveWhere_EmptyDict_NoOp()
    {
        var dict = new Dictionary<string, int>();
        dict.RemoveWhere(k => true);
        dict.Count.ShouldBe(0);
    }

    [Fact]
    public void RemoveWhere_NullPredicate_Throws()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };
        Should.Throw<ArgumentNullException>(() => dict.RemoveWhere(null!));
    }
}
