// -----------------------------------------------------------------------
// <copyright file="TemplateTransformTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Utilities.Text.Templating;
using Xunit;

namespace MyNet.Utilities.Tests.Text;

public class TemplateTransformTests
{
    [Fact]
    public void Render_WithQuantityAndCustomArguments_FormatsUsingCulture()
    {
        var options = new TextTemplateOptionsBuilder()
            .WithQuantity(1234.5m)
            .WithArgument("price", 9876.54m)
            .Build();

        var result = new TemplateTransform(options).Apply("Q={quantity:N1} / P={price:N2}", new("fr-FR"));

        var normalized = result.Replace('\u202F', ' ').Replace('\u00A0', ' ');
        Assert.Equal("Q=1 234,5 / P=9 876,54", normalized);
    }

    [Fact]
    public void Render_UnknownPlaceholder_KeepsOriginalToken()
    {
        var options = new TextTemplateOptionsBuilder().Build();

        var result = new TemplateTransform(options).Apply("Hello {unknown}", new("en-US"));

        Assert.Equal("Hello {unknown}", result);
    }

    [Fact]
    public void Render_NullTemplate_ReturnsEmptyString()
    {
        var options = new TextTemplateOptionsBuilder().Build();

        var result = new TemplateTransform(options).Apply(null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Render_NullOptions_Throws() => Assert.Throws<ArgumentNullException>(() => new TemplateTransform(null!).Apply("{quantity}", CultureInfo.InvariantCulture));

    [Fact]
    public void Render_NullCulture_Throws()
    {
        var options = new TextTemplateOptionsBuilder().Build();

        Assert.Throws<ArgumentNullException>(() => new TemplateTransform(options).Apply("{quantity}", null!));
    }
}
