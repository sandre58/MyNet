// -----------------------------------------------------------------------
// <copyright file="CollectionStatisticsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Statistics;
using Xunit;

namespace MyNet.Observable.Tests.Collections;

public sealed class CollectionStatisticsTests
{
    [Fact]
    public void Statistics_FilteredPercentage_ShouldReflectFilteredOverSource()
    {
        using var collection = ExtendedCollection.From([1, 2, 3, 4]);
        using var statistics = collection.Statistics(x => x);

        collection.SetFilter(new ExpressionFilter<int>(x => x % 2 == 0));

        collection.Count.Should().Be(2);
        collection.SourceCount.Should().Be(4);
        statistics.FilteredPercentage.Should().Be(0.5);
    }

    [Fact]
    public void Statistics_WhenCountChanges_ShouldRaiseFilteredPercentagePropertyChanged()
    {
        using var collection = ExtendedCollection.From([1, 2, 3]);
        using var statistics = collection.Statistics(x => x);

        var percentageChanges = 0;
        statistics.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CollectionStatistics<,>.FilteredPercentage))
                percentageChanges++;
        };

        collection.SetFilter(new ExpressionFilter<int>(x => x > 1));

        percentageChanges.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Statistics_ShouldAggregateFilteredItems()
    {
        using var collection = ExtendedCollection.From(
        [
            new(1),
            new(2),
            new MetricItem(10)
        ]);

        collection.SetFilter(new ExpressionFilter<MetricItem>(x => x.Value < 5));
        using var statistics = collection.Statistics(x => x.Value);

        statistics.Sum.Should().Be(3);
        statistics.Average.Should().Be(1.5);
        statistics.Min.Should().Be(1);
        statistics.Max.Should().Be(2);
    }

    [Fact]
    public void Statistics_WhenFilterChanges_ShouldRecalculate()
    {
        using var collection = ExtendedCollection.From([new(1), new MetricItem(5)]);
        using var statistics = collection.Statistics(x => x.Value);

        statistics.Sum.Should().Be(6);

        var sumChanges = 0;
        statistics.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(statistics.Sum))
                sumChanges++;
        };

        collection.SetFilter(new ExpressionFilter<MetricItem>(x => x.Value < 3));

        sumChanges.Should().BeGreaterThan(0);
        statistics.Sum.Should().Be(1);
    }

    [Fact]
    public void Statistics_TimeSpan_ShouldAggregateFilteredItems()
    {
        using var collection = ExtendedCollection.From(
        [
            new(TimeSpan.FromMinutes(1)),
            new DurationItem(TimeSpan.FromMinutes(3))
        ]);

        collection.SetFilter(new ExpressionFilter<DurationItem>(x => x.Duration < TimeSpan.FromMinutes(2)));
        using var statistics = collection.Statistics(x => x.Duration);

        statistics.Sum.Should().Be(TimeSpan.FromMinutes(1));
        statistics.Min.Should().Be(TimeSpan.FromMinutes(1));
        statistics.Max.Should().Be(TimeSpan.FromMinutes(1));
    }

    private sealed record MetricItem(int Value);

    private sealed record DurationItem(TimeSpan Duration);
}
