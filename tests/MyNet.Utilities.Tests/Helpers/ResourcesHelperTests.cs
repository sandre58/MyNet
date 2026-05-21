// -----------------------------------------------------------------------
// <copyright file="ResourcesHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Helpers;
using Xunit;

namespace MyNet.Utilities.Tests.Helpers;

public class ResourcesHelperTests
{
    [Fact]
    public void OpenEmbeddedResource_WithNullAssembly_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => ResourcesHelper.OpenEmbeddedResource(null!, "DataResources.resources"));

    [Fact]
    public void OpenEmbeddedResource_WithNullSuffix_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => ResourcesHelper.OpenEmbeddedResource(typeof(ResourcesHelperTests).Assembly, null!));

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void OpenEmbeddedResource_WithWhitespaceSuffix_ThrowsArgumentException(string suffix)
        => Assert.Throws<ArgumentException>(() => ResourcesHelper.OpenEmbeddedResource(typeof(ResourcesHelperTests).Assembly, suffix));

    [Fact]
    public void OpenEmbeddedResource_WhenResourceNotFound_ThrowsInvalidOperationException()
        => Assert.Throws<InvalidOperationException>(() => ResourcesHelper.OpenEmbeddedResource(typeof(ResourcesHelperTests).Assembly, "Missing.resource"));

    [Fact]
    public void OpenEmbeddedResource_WhenMultipleResourcesMatch_ThrowsInvalidOperationException()
        => Assert.Throws<InvalidOperationException>(() => ResourcesHelper.OpenEmbeddedResource(typeof(ResourcesHelperTests).Assembly, ".resources"));

    [Fact]
    public void OpenEmbeddedResource_WhenUniqueResourceMatch_ReturnsStream()
    {
        using var stream = ResourcesHelper.OpenEmbeddedResource(typeof(ResourcesHelperTests).Assembly, "OtherDataResources.resources", StringComparison.Ordinal);

        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
    }
}
