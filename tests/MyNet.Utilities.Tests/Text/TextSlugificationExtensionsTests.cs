// -----------------------------------------------------------------------
// <copyright file="TextSlugificationExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Text.Slugification;
using Xunit;

namespace MyNet.Utilities.Tests.Text;

public class TextSlugificationExtensionsTests
{
    [Fact]
    public void Slugify_CreatesLowercaseSlug()
    {
        var result = "Hello World From .NET".Slugify();

        Assert.Equal("hello-world-from-net", result);
    }

    [Fact]
    public void Slugify_RemovesDiacriticsByDefault()
    {
        var result = "Crème brûlée".Slugify();

        Assert.Equal("creme-brulee", result);
    }

    [Fact]
    public void Slugify_UsesCustomSeparator()
    {
        var result = "hello world".Slugify(new TextSlugifierOptions { Separator = '_' });

        Assert.Equal("hello_world", result);
    }

    [Fact]
    public void Slugify_SnakeCasePreset_UsesUnderscore()
    {
        var result = "Hello world from .NET".Slugify(Slugifier.SnakeCase);

        Assert.Equal("hello_world_from_net", result);
    }

    [Fact]
    public void Slugify_PreserveCasePreset_KeepOriginalCase()
    {
        var result = "Hello World".Slugify(Slugifier.PreserveCase);

        Assert.Equal("Hello-World", result);
    }
}
