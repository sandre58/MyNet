// -----------------------------------------------------------------------
// <copyright file="MetadataFeatureExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Utilities.Metadata;
using Xunit;

namespace MyNet.Observable.Tests.Metadata;

public sealed class MetadataFeatureExtensionsTests
{
    [Fact]
    public void PropertyBuilderExtensions_ConfigureExpectedFeatures()
    {
        MetadataRegistry.For<SampleModel>()
            .Property(x => x.Name)
            .UpdateOnCultureChanged()
            .UpdateOnTimeZoneChanged()
            .IgnoreModificationTracking()
            .Validates(nameof(SampleModel.Other));

        var metadata = MetadataRegistry.Get(typeof(SampleModel));
        var property = metadata.GetProperty(nameof(SampleModel.Name));

        Assert.True(property.TryGetFeature<EventReactionFeature>(out var events));
        Assert.NotNull(events);
        Assert.Contains(typeof(CultureChangedEvent), events.Events);
        Assert.Contains(typeof(TimeZoneChangedEvent), events.Events);

        Assert.True(property.TryGetFeature<ModificationTrackingFeature>(out var tracking));
        Assert.NotNull(tracking);
        Assert.True(tracking.Ignore);

        Assert.True(property.TryGetFeature<ValidationDependencyFeature>(out var dependencies));
        Assert.NotNull(dependencies);
        Assert.Contains(nameof(SampleModel.Other), dependencies.Dependents);
    }

    [Fact]
    public void PropertyBuilder_ForwardPropertyChanged_ConfiguresForwardingFeature()
    {
        MetadataRegistry.For<ForwardingModel>()
            .Property(x => x.Wrapper)
            .ForwardPropertyChanged(concatenatePropertyName: false);

        var property = MetadataRegistry.Get(typeof(ForwardingModel)).GetProperty(nameof(ForwardingModel.Wrapper));

        Assert.True(property.TryGetFeature<PropertyChangedForwardingFeature>(out var feature));
        Assert.False(feature.ConcatenatePropertyName);
    }

    [Fact]
    public void PropertyBuilder_ReactTo_AddsCustomEvent()
    {
        MetadataRegistry.For<EventModel>()
            .Property(x => x.Label)
            .ReactTo(typeof(CustomRefreshEvent));

        var property = MetadataRegistry.Get(typeof(EventModel)).GetProperty(nameof(EventModel.Label));

        Assert.True(property.TryGetFeature<EventReactionFeature>(out var feature));
        Assert.Contains(typeof(CustomRefreshEvent), feature.Events);
    }

    [Fact]
    public void TypeMetadataExtensions_ApplyToAllSpecifiedProperties()
    {
        var metadata = MetadataRegistry.Get(typeof(SecondModel));

        metadata
            .UpdateOnCultureChanged(nameof(SecondModel.First), nameof(SecondModel.Second))
            .IgnoreModificationTracking(nameof(SecondModel.Second));

        var first = metadata.GetProperty(nameof(SecondModel.First));
        var second = metadata.GetProperty(nameof(SecondModel.Second));

        Assert.True(first.TryGetFeature<EventReactionFeature>(out var firstEvents));
        Assert.NotNull(firstEvents);
        Assert.Contains(typeof(CultureChangedEvent), firstEvents.Events);

        Assert.True(second.TryGetFeature<EventReactionFeature>(out var secondEvents));
        Assert.NotNull(secondEvents);
        Assert.Contains(typeof(CultureChangedEvent), secondEvents.Events);

        Assert.True(second.TryGetFeature<ModificationTrackingFeature>(out var secondTracking));
        Assert.NotNull(secondTracking);
        Assert.True(secondTracking.Ignore);
    }

    private sealed class SampleModel
    {
        public string Name { get; } = string.Empty;

        public string Other { get; set; } = string.Empty;
    }

    private sealed class SecondModel
    {
        public string First { get; set; } = string.Empty;

        public string Second { get; set; } = string.Empty;
    }

    private sealed class ForwardingModel
    {
        public object Wrapper { get; } = new();
    }

    private sealed class EventModel
    {
        public string Label { get; } = string.Empty;
    }

    private sealed class CustomRefreshEvent;
}
