// -----------------------------------------------------------------------
// <copyright file="MetadataApplicatorsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using MyNet.Observable.Behaviors;
using MyNet.Observable.Behaviors.Metadata;
using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Utilities.Metadata;
using Xunit;

namespace MyNet.Observable.Tests.Metadata;

public sealed class MetadataApplicatorsTests
{
    [Fact]
    public void Apply_ConfiguresExpectedFeatures()
    {
        var metadata = new PropertyMetadata();

        MetadataApplicators.ApplyUpdateOnCultureChanged(metadata);
        MetadataApplicators.ApplyUpdateOnTimeZoneChanged(metadata);
        MetadataApplicators.ApplyIgnoreModificationTracking(metadata);
        MetadataApplicators.ApplyAlsoValidate(metadata, nameof(Sample.Confirm));
        MetadataApplicators.ApplyForwardProperty(metadata, concatenatePropertyName: false);

        Assert.True(metadata.TryGetFeature<EventReactionFeature>(out var events));
        Assert.Contains(typeof(CultureChangedEvent), events!.Events);
        Assert.Contains(typeof(TimeZoneChangedEvent), events.Events);

        Assert.True(metadata.TryGetFeature<ModificationTrackingFeature>(out var tracking));
        Assert.True(tracking!.Ignore);

        Assert.True(metadata.TryGetFeature<ValidationDependencyFeature>(out var validation));
        Assert.Contains(nameof(Sample.Confirm), validation!.Dependents);

        Assert.True(metadata.TryGetFeature<PropertyChangedForwardingFeature>(out var forwarding));
        Assert.False(forwarding!.ConcatenatePropertyName);
    }

    [Fact]
    public void ApplyForwardProperty_RegistersForwardingBehaviorOnObservableObject()
    {
        var typeMetadata = MetadataRegistry.Get(typeof(ForwardingOwner));
        MetadataApplicators.ApplyForwardProperty(typeMetadata.GetProperty(nameof(ForwardingOwner.Wrapper)));

        var owner = new ForwardingOwner();
        Assert.True(owner.HasBehavior<PropertyChangedForwardingBehavior>(nameof(ForwardingOwner.Wrapper)));

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        owner.Wrapper.Name = "x";
        Assert.Contains($"{nameof(ForwardingOwner.Wrapper)}.{nameof(WrapperChild.Name)}", changed);
    }

    private sealed class Sample
    {
        public string Confirm { get; set; } = string.Empty;
    }

    private sealed class ForwardingOwner : ObservableObject
    {
        public WrapperChild Wrapper { get; set; } = new();
    }

    private sealed class WrapperChild : INotifyPropertyChanged
    {
        private string _name = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                PropertyChanged?.Invoke(this, new(nameof(Name)));
            }
        }
    }
}
