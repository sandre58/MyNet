// -----------------------------------------------------------------------
// <copyright file="DictionaryExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Xunit;

namespace MyNet.Utilities.Tests.Collections;

public class DictionaryExtensionsTests
{
    #region GetOrCreate

    [Fact]
    public void GetOrCreate_KeyExists_ReturnExistingValue()
    {
        var dict = new Dictionary<string, int> { ["a"] = 42 };
        var result = dict.GetOrCreate("a", () => 99);
        Assert.Equal(42, result);
        Assert.Single(dict);
    }

    [Fact]
    public void GetOrCreate_KeyMissing_AddsAndReturnsNewValue()
    {
        var dict = new Dictionary<string, int>();
        var result = dict.GetOrCreate("b", () => 7);
        Assert.Equal(7, result);
        Assert.True(dict.ContainsKey("b"));
        Assert.Equal(7, dict["b"]);
    }

    #endregion

    #region GetOrDefault

    [Fact]
    public void GetOrDefault_KeyExists_ReturnExistingValue()
    {
        var dict = new Dictionary<string, int> { ["x"] = 100 };
        var result = dict.GetOrDefault("x");
        Assert.Equal(100, result);
    }

    [Fact]
    public void GetOrDefault_KeyMissing_ReturnsDefault()
    {
        var dict = new Dictionary<string, int>();
        var result = dict.GetOrDefault("z");
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetOrDefault_KeyMissing_ReturnsProvidedDefault()
    {
        var dict = new Dictionary<string, string>();
        var result = dict.GetOrDefault("z", "fallback");
        Assert.Equal("fallback", result);
    }

    #endregion

    #region Merge

    [Fact]
    public void Merge_TwoDictionaries_CombinesAll()
    {
        var d1 = new Dictionary<string, int> { ["a"] = 1 };
        var d2 = new Dictionary<string, int> { ["b"] = 2 };
        var result = d1.Merge(d2);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result["a"]);
        Assert.Equal(2, result["b"]);
    }

    [Fact]
    public void Merge_OverlappingKeys_LastValueWins()
    {
        var d1 = new Dictionary<string, int> { ["a"] = 1 };
        var d2 = new Dictionary<string, int> { ["a"] = 99 };
        var result = d1.Merge(d2);

        Assert.Single(result);
        Assert.Equal(99, result["a"]);
    }

    [Fact]
    public void Merge_Enumerable_CombinesMultiple()
    {
        var list = new List<IDictionary<string, int>>
        {
            new Dictionary<string, int> { ["a"] = 1 },
            new Dictionary<string, int> { ["b"] = 2 },
            new Dictionary<string, int> { ["c"] = 3 }
        };

        var result = list.Merge();

        Assert.Equal(3, result.Count);
    }

    #endregion
}
