// -----------------------------------------------------------------------
// <copyright file="CultureExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Xunit;

namespace MyNet.Primitives.Tests.Extensions;

public sealed class CultureExtensionsTests
{
    [Fact]
    public void IsCurrent_NullCulture_ReturnsTrue()
    {
        CultureInfo? culture = null;
        Assert.True(culture.IsCurrent());
    }

    [Fact]
    public void OrCurrent_Null_ReturnsCurrentCulture()
    {
        CultureInfo? culture = null;
        Assert.Equal(CultureInfo.CurrentCulture, culture.OrCurrent());
    }

    [Fact]
    public void OrInvariant_Null_ReturnsInvariantCulture()
    {
        CultureInfo? culture = null;
        Assert.Equal(CultureInfo.InvariantCulture, culture.OrInvariant());
    }
}
