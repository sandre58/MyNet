// -----------------------------------------------------------------------
// <copyright file="PropertyBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyNet.Metadata.Tests;

public sealed class PropertyBuilderTests
{
    [Fact]
    public void Feature_ConfiguresCustomFeatureInstance()
    {
        var property = MetadataRegistry.For<SampleModel>()
            .Property(x => x.Name)
            .Feature<CustomFeature>(f => f.Multiplier = 3)
            .Metadata;

        Assert.True(property.TryGetFeature<CustomFeature>(out var feature));
        Assert.Equal(3, feature.Multiplier);
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used for testing the PropertyBuilder.Feature method")]
    private sealed class SampleModel
    {
        public string Name { get; } = string.Empty;
    }

    private sealed class CustomFeature
    {
        public int Multiplier { get; set; } = 1;
    }
}
