// -----------------------------------------------------------------------
// <copyright file="ExtendedCollectionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DynamicData;
using FluentAssertions;
using MyNet.Observable.Collections;
using Xunit;

namespace MyNet.Observable.Tests.Collections;

public sealed class ExtendedCollectionExtensionsTests
{
    [Fact]
    public void ToObservableChangeSet_EmitsUpdatesWhenCollectionChanges()
    {
        using var collection = ExtendedCollection.From([1, 2]);
        var changeSets = new List<IChangeSet<int>>();

        using var subscription = collection.ToObservableChangeSet().Subscribe(onChangeSet);

        changeSets.Should().NotBeEmpty();

        collection.Add(3);

        changeSets.Should().HaveCountGreaterThan(1);
        return;

        void onChangeSet(IChangeSet<int> set) => changeSets.Add(set);
    }
}
