// -----------------------------------------------------------------------
// <copyright file="LabeledValueTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class LabeledValueTests
{
    [Fact]
    public void Constructor_WithResourceKey_UsesLocalizedStringLabel()
    {
        var labeled = new LabeledValue<int>(42, "Items.Count");

        Assert.Equal(42, labeled.Value);
        Assert.Equal("Items.Count", labeled.ToString());
    }

    [Fact]
    public void Constructor_WithDisplayName_UsesProvidedLabel()
    {
        var label = new LocalizedString("Custom.Key");
        var labeled = new LabeledValue<string>("value", label);

        Assert.Equal("value", labeled.Value);
        Assert.Same(label, labeled.DisplayName);
    }
}
