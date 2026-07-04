// -----------------------------------------------------------------------
// <copyright file="ObservablePropertyTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyNet.Observable;
using MyNet.Observable.Metadata;
using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class ObservablePropertyTests
{
    [Fact]
    public void GeneratedProperty_UsesSetPropertyPipeline()
    {
        var sut = new GeneratedPropertySample();
        var events = new List<string>();

        sut.PropertyChanging += (_, e) => events.Add("changing:" + e.PropertyName);
        sut.PropertyChanged += (_, e) => events.Add("changed:" + e.PropertyName);

        sut.Title = "hello";

        Assert.Equal(["changing:Title", "changed:Title"], events);
        Assert.Equal("hello", sut.Title);
    }
}

[SuppressMessage("ReSharper", "ReplaceWithFieldKeyword", Justification = "Testing the source generator")]
internal sealed partial class GeneratedPropertySample : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;
}
