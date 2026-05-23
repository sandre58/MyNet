// -----------------------------------------------------------------------
// <copyright file="SelectableCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Selection;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Selection;

public sealed class SelectableCollectionTests
{
    [Fact]
    public void FactoryCreate_ShouldDisposeOwnedCollection()
    {
        var selectable = SelectableCollection.From([1, 2, 3]);

        selectable.Dispose();

        selectable.Collection.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void ExternalCollection_ShouldNotDisposeUnderlyingCollection()
    {
        using var collection = ExtendedCollection.From([1, 2]);
        using var selectable = new SelectableCollection<int>(collection);

        selectable.Dispose();

        collection.IsDisposed.Should().BeFalse();
    }
}
