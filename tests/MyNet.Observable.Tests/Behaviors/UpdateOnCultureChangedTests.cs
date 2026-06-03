// -----------------------------------------------------------------------
// <copyright file="UpdateOnCultureChangedTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MyNet.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Facade;
using MyNet.Metadata;
using MyNet.Observable.Behaviors;
using MyNet.Observable.Behaviors.Metadata.Attributes;
using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class UpdateOnCultureChangedTests
{
    [Fact]
    public void Attribute_ConfiguresEventReactionFeatureInMetadata()
    {
        var metadata = MetadataRegistry.Get(typeof(AttributeCultureAwareViewModel)).GetProperty(nameof(AttributeCultureAwareViewModel.CultureName));

        Assert.True(metadata.TryGetFeature<EventReactionFeature>(out var feature));
        Assert.Contains(typeof(CultureChangedEvent), feature.Events);
    }

    [Fact]
    public void Attribute_RegistersCultureChangedBehaviorOnConstruction()
    {
        var sut = new AttributeCultureAwareViewModel();

        Assert.True(sut.Behaviors.Has<CultureChangedBehavior>());
    }

    [Fact]
    public void Attribute_RaisesPropertyChanged_WhenCultureChangesViaGlobalizationServices()
    {
        var sut = new AttributeCultureAwareViewModel();
        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        GlobalizationServices.Current.SetCulture("en-US");
        GlobalizationServices.Current.SetCulture("fr-FR");

        Assert.Contains(nameof(AttributeCultureAwareViewModel.CultureName), changedProperties);
    }

    [Fact]
    public void Attribute_RaisesPropertyChanged_ForEachConfiguredProperty()
    {
        var sut = new MultiCultureAwareViewModel();
        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        GlobalizationServices.Current.SetCulture("en-US");
        GlobalizationServices.Current.SetCulture("fr-FR");

        Assert.Contains(nameof(MultiCultureAwareViewModel.CultureName), changedProperties);
        Assert.Contains(nameof(MultiCultureAwareViewModel.CultureDisplayName), changedProperties);
    }

    [Fact]
    public void Attribute_OnAbstractBase_RaisesPropertyChanged_WhenCultureChanges()
    {
        var sut = new DerivedAttributeCultureAwareViewModel();
        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        GlobalizationServices.Current.SetCulture("en-US");
        GlobalizationServices.Current.SetCulture("fr-FR");

        Assert.Contains(nameof(AbstractAttributeCultureAwareViewModelBase.CultureName), changedProperties);
    }

    [Fact]
    public void Attribute_RaisesPropertyChanged_WhenCultureChangesViaConfiguredGlobalizationService()
    {
        var cultureService = new CultureService(CultureInfo.GetCultureInfo("en-US"));
        var globalization = new GlobalizationService(cultureService, new Globalization.DateTime.TimeZoneService());
        GlobalizationServices.Configure(globalization);

        var sut = new AttributeCultureAwareViewModel();
        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        cultureService.SetCulture(CultureInfo.GetCultureInfo("fr-FR"));

        Assert.Contains(nameof(AttributeCultureAwareViewModel.CultureName), changedProperties);
    }

    [Fact]
    public void Attribute_DoesNotRaisePropertyChanged_WhenCultureChangesOnUnwiredCultureService()
    {
        var wiredCultureService = new CultureService(CultureInfo.GetCultureInfo("en-US"));
        var unwiredCultureService = new CultureService(CultureInfo.GetCultureInfo("en-US"));
        var globalization = new GlobalizationService(wiredCultureService, new Globalization.DateTime.TimeZoneService());
        GlobalizationServices.Configure(globalization);

        var sut = new AttributeCultureAwareViewModel();
        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        unwiredCultureService.SetCulture(CultureInfo.GetCultureInfo("fr-FR"));

        Assert.DoesNotContain(nameof(AttributeCultureAwareViewModel.CultureName), changedProperties);
    }
}

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Instance properties required for metadata and binding tests.")]
internal sealed class AttributeCultureAwareViewModel : ObservableObject
{
    [UpdateOnCultureChanged]
    public string CultureName => GlobalizationServices.Current.CurrentCulture.Name;
}

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Instance properties required for metadata and binding tests.")]
internal abstract class AbstractAttributeCultureAwareViewModelBase : ObservableObject
{
    [UpdateOnCultureChanged]
    public string CultureName => GlobalizationServices.Current.CurrentCulture.Name;
}

internal sealed class DerivedAttributeCultureAwareViewModel : AbstractAttributeCultureAwareViewModelBase;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Instance properties required for metadata and binding tests.")]
internal sealed class MultiCultureAwareViewModel : ObservableObject
{
    [UpdateOnCultureChanged]
    public string CultureName => GlobalizationServices.Current.CurrentCulture.Name;

    [UpdateOnCultureChanged]
    public string CultureDisplayName => GlobalizationServices.Current.CurrentCulture.DisplayName;
}
