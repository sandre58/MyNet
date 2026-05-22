// -----------------------------------------------------------------------
// <copyright file="MetadataAttributeBootstrapperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Behaviors.Metadata.Attributes;
using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Observable.Behaviors.Metadata.Handlers;
using MyNet.Utilities.Metadata;
using Xunit;

namespace MyNet.Observable.Tests.Metadata;

public sealed class MetadataAttributeBootstrapperTests
{
    [Fact]
    public void Apply_UsesRegisteredHandlersAndConfiguresPropertyMetadata()
    {
        MetadataAttributeSupport.RegisterDefaults();
        MetadataAttributeBootstrapper.Apply(typeof(AttributedType));

        var metadata = MetadataRegistry.Get(typeof(AttributedType));

        var display = metadata.GetProperty(nameof(AttributedType.DisplayName));
        Assert.True(display.TryGetFeature<EventReactionFeature>(out var eventFeature));
        Assert.NotNull(eventFeature);
        Assert.Contains(typeof(CultureChangedEvent), eventFeature.Events);

        var ignored = metadata.GetProperty(nameof(AttributedType.InternalValue));
        Assert.True(ignored.TryGetFeature<ModificationTrackingFeature>(out var tracking));
        Assert.NotNull(tracking);
        Assert.True(tracking.Ignore);

        var validated = metadata.GetProperty(nameof(AttributedType.Email));
        Assert.True(validated.TryGetFeature<ValidationDependencyFeature>(out var dependencies));
        Assert.NotNull(dependencies);
        Assert.Contains(nameof(AttributedType.ConfirmEmail), dependencies.Dependents);
    }

    private sealed class AttributedType
    {
        [UpdateOnCultureChanged]
        public string DisplayName { get; set; } = string.Empty;

        [IgnoreModificationTracking]
        public int InternalValue { get; set; }

        [AlsoValidate(nameof(ConfirmEmail))]
        public string Email { get; set; } = string.Empty;

        public string ConfirmEmail { get; set; } = string.Empty;
    }
}
