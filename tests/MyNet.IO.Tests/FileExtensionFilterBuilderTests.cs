// -----------------------------------------------------------------------
// <copyright file="FileExtensionFilterBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.IO.FileExtensions;
using Xunit;

namespace MyNet.IO.Tests;

public sealed class FileExtensionFilterBuilderTests
{
    [Fact]
    public void Build_AggregatesAddedGroups()
    {
        var documents = FileExtensionGroup.Create("Documents", "txt", "pdf");
        var images = FileExtensionGroup.Create("Images", "png", "jpg");

        var filter = new FileExtensionFilterBuilder()
            .Add(documents)
            .AddRange([images])
            .Build();

        Assert.Equal(2, filter.Groups.Count);
        Assert.Contains(filter.Groups, g => g.Key == "Documents");
        Assert.Contains(filter.Groups, g => g.Key == "Images");
    }
}
