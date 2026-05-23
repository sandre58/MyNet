// -----------------------------------------------------------------------
// <copyright file="TypeMetadataExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Metadata;
using Xunit;

namespace MyNet.Utilities.Tests.Metadata;

public sealed class TypeMetadataExtensionsTests
{
    [Fact]
    public void WithFeature_ReturnsMatchingPropertyNames()
    {
        var metadata = new TypeMetadata();
        var first = metadata.GetProperty("First");
        var second = metadata.GetProperty("Second");

        first.SetFeature(new SampleFeature { Tag = "a" });
        second.SetFeature(new SampleFeature { Tag = "b" });

        var names = metadata.WithFeature<SampleFeature>(f => f.Tag == "b");

        Assert.Equal(["Second"], names);
    }

    [Fact]
    public void TryGetFeature_ReturnsFalse_WhenPropertyOrFeatureMissing()
    {
        var metadata = new TypeMetadata();

        Assert.False(metadata.TryGetFeature<SampleFeature>("Missing", out _));
        Assert.False(metadata.TryGetFeature<SampleFeature>("Empty", out _));

        metadata.GetProperty("Empty");
        Assert.False(metadata.TryGetFeature<SampleFeature>("Empty", out _));
    }

    [Fact]
    public void GetFeatureOrDefault_ReturnsFeature_WhenPresent()
    {
        var metadata = new TypeMetadata();
        var property = metadata.GetProperty("Name");
        var expected = new SampleFeature { Tag = "ok" };
        property.SetFeature(expected);

        Assert.Same(expected, metadata.GetFeatureOrDefault<SampleFeature>("Name"));
        Assert.Null(metadata.GetFeatureOrDefault<SampleFeature>("Other"));
    }

    private sealed class SampleFeature
    {
        public string Tag { get; init; } = string.Empty;
    }
}
