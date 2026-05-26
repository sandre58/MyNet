// -----------------------------------------------------------------------
// <copyright file="TemplateTransformQuantityTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Text.Templating;
using Xunit;

namespace MyNet.Text.Tests;

public sealed class TemplateTransformQuantityTests
{
    [Fact]
    public void Apply_WithPrefixQuantity_RendersQuantityBeforeText()
    {
        var options = new TextTemplateOptionsBuilder()
            .PrefixWithQuantity(3, quantitySeparator: " x ")
            .Build();

        var result = new TemplateTransform(options).Apply("items", CultureInfo.InvariantCulture);

        Assert.Equal("3 x items", result);
    }

    [Fact]
    public void Apply_WithSuffixQuantity_RendersQuantityAfterText()
    {
        var options = new TextTemplateOptionsBuilder()
            .SuffixWithQuantity(2, quantitySeparator: ": ")
            .Build();

        var result = new TemplateTransform(options).Apply("items", CultureInfo.InvariantCulture);

        Assert.Equal("items: 2", result);
    }

    [Fact]
    public void Apply_WithPlaceholderOnly_DoesNotDuplicateQuantity()
    {
        var options = new TextTemplateOptionsBuilder()
            .WithQuantity(5)
            .Build();

        var result = new TemplateTransform(options).Apply("{quantity} files", CultureInfo.InvariantCulture);

        Assert.Equal("5 files", result);
    }
}
