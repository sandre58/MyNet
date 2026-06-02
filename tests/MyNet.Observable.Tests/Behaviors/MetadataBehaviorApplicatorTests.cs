// -----------------------------------------------------------------------
// <copyright file="MetadataBehaviorApplicatorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyNet.Globalization.Facade;
using MyNet.Metadata;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class MetadataBehaviorApplicatorTests
{
    [Fact]
    public void Apply_RegistersCultureChangedBehaviorForUpdateOnCultureChangedProperties()
    {
        MetadataRegistry.For<CultureAwareOwner>()
            .Property(x => x.Label)
            .UpdateOnCultureChanged();

        var sut = new CultureAwareOwner();

        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        GlobalizationServices.Current.SetCulture("en-US");
        GlobalizationServices.Current.SetCulture("fr-FR");

        Assert.Contains(nameof(CultureAwareOwner.Label), changedProperties);
    }

    private sealed class CultureAwareOwner : ObservableObject
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Required for metadata registration.")]
        public string Label => "Label";
    }
}
