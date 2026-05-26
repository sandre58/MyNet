// -----------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Metadata.Tests;

public sealed class TypeExtensionsTests
{
    [Fact]
    public void GetMetadata_ReturnsRegistryEntry()
    {
        var metadata = typeof(TypeExtensionsTests).GetMetadata();

        Assert.Same(MetadataRegistry.Get(typeof(TypeExtensionsTests)), metadata);
    }
}
