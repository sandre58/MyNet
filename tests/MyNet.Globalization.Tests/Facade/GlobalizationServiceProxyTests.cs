// -----------------------------------------------------------------------
// <copyright file="GlobalizationServiceProxyTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Culture;
using MyNet.Globalization.Facade;
using Xunit;

namespace MyNet.Globalization.Tests.Facade;

[Collection("UseCultureSequential")]
public sealed class GlobalizationServiceProxyTests
{
    [Fact]
    public void Current_IsStableProxyInstance()
    {
        var first = GlobalizationServices.Current;
        var second = GlobalizationServices.Current;

        Assert.Same(first, second);
    }

    [Fact]
    public void Configure_ForwardsConfiguredServiceWithoutChangingCurrentReference()
    {
        // UtilitiesLocalizationFixture already called UseGlobalization/UseLocalization with test resources.
        var before = GlobalizationServices.Current;
        var notifications = 0;
        before.CultureChanged += (_, _) => notifications++;

        var targetCulture = before.CurrentCulture.Name == SupportedCultures.French.Name
            ? SupportedCultures.English
            : SupportedCultures.French;

        before.SetCulture(targetCulture);

        Assert.Same(before, GlobalizationServices.Current);
        Assert.Equal(1, notifications);
        Assert.Equal(targetCulture.Name, GlobalizationServices.Current.CurrentCulture.Name);
    }
}
