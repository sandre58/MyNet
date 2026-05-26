// -----------------------------------------------------------------------
// <copyright file="CultureFallbackPoliciesTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Localization.Policies;
using Xunit;

namespace MyNet.Globalization.Tests.Localization;

public sealed class CultureFallbackPoliciesTests
{
    [Fact]
    public void None_AlwaysReturnsNull()
    {
        var culture = CultureInfo.GetCultureInfo("fr-FR");

        Assert.Null(CultureFallbackPolicies.None.GetFallback(culture));
    }

    [Fact]
    public void ParentCulture_WalksUpHierarchy()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");

        var fallback = CultureFallbackPolicies.ParentCulture.GetFallback(culture);

        Assert.Equal("en", fallback!.Name);
    }

    [Fact]
    public void ParentCulture_ForInvariantCulture_ReturnsNull() => Assert.Null(CultureFallbackPolicies.ParentCulture.GetFallback(CultureInfo.InvariantCulture));
}
