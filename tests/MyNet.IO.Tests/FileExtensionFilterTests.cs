// -----------------------------------------------------------------------
// <copyright file="FileExtensionFilterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using MyNet.IO.FileExtensions;
using Xunit;

namespace MyNet.IO.Tests;

public sealed class FileExtensionFilterTests
{
    [Fact]
    public void AllExtensions_ReturnsDistinctValuesAcrossGroups()
    {
        var documents = FileExtensionGroup.Create("Documents", "txt", "pdf");
        var images = FileExtensionGroup.Create("Images", "png", "jpg");
        var filter = new FileExtensionFilter([documents, images]);

        var extensions = filter.AllExtensions.ToArray();

        Assert.Equal(4, extensions.Length);
        Assert.Contains(".txt", extensions);
        Assert.Contains(".jpg", extensions);
    }

    [Fact]
    public void ToFilterString_FormatsGroupsForFileDialog()
    {
        var group = FileExtensionGroup.Create("Images", "png", "jpg");
        var filter = new FileExtensionFilter([group]);

        var result = filter.ToFilterString(key => $"[{key}]");

        Assert.Equal("[Images] (*.png;*.jpg)", result);
    }
}
