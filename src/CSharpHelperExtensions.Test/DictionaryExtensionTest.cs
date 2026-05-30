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
}
