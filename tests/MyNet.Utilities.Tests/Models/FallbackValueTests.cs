// -----------------------------------------------------------------------
// <copyright file="FallbackValueTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Utilities.Tests.Models;

public class FallbackValueTests
{
    [Fact]
    public void Value_UsesFallback_WhenNoOverride()
    {
        var fallback = new FallbackValue<int>(() => 10);

        Assert.Equal(10, fallback.Value);
    }

    [Fact]
    public void SetOverride_OverridesFallbackValue()
    {
        var fallback = new FallbackValue<int>(() => 10);

        fallback.SetOverride(42);

        Assert.Equal(42, fallback.Value);
    }

    [Fact]
    public void ResetOverride_RestoresFallbackValue()
    {
        var fallback = new FallbackValue<int>(() => 10);
        fallback.SetOverride(42);

        fallback.ResetOverride();

        Assert.Equal(10, fallback.Value);
    }

    [Fact]
    public void Value_AllowsNullFallbackValues()
    {
        var fallback = new FallbackValue<string?>(() => null);

        Assert.Null(fallback.Value);
    }
}
