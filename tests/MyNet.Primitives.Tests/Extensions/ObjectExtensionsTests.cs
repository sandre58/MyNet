// -----------------------------------------------------------------------
// <copyright file="ObjectExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Primitives.Tests.Extensions;

public sealed class ObjectExtensionsTests
{
    [Fact]
    public void CastIn_ReturnsTypedValue()
    {
        object value = 42;
        Assert.Equal(42, value.CastIn<int>());
    }

    [Fact]
    public void IfNull_ExecutesWhenNull()
    {
        object? value = null;
        var executed = false;
        value.IfNull(() => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void IfIs_ExecutesForMatchingType()
    {
        object value = "hello";
        string? captured = null;
        value.IfIs<string>(s => captured = s);
        Assert.Equal("hello", captured);
    }

    [Fact]
    public void OrThrow_Null_Throws()
    {
        const string? value = null;
        Assert.Throws<ArgumentNullException>(() => value.OrThrow());
    }

    [Fact]
    public void Or_ReturnsFallbackWhenNull()
    {
        const string? value = null;
        Assert.Equal("fallback", value.Or("fallback"));
    }

    [Fact]
    public void IfNotNull_ExecutesWhenNotNull()
    {
        const string value = "x";
        var executed = false;
        value.IfNotNull(_ => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void ConvertTo_AppliesSelectorWhenNotNull()
    {
        int? value = 5;
        Assert.Equal(10, value.ConvertTo(x => x * 2));
        int? nullValue = null;
        Assert.Null(nullValue.ConvertTo(x => x * 2));
    }
}
