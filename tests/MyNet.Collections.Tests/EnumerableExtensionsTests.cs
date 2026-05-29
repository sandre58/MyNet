// -----------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyNet.Primitives;
using Xunit;

namespace MyNet.Collections.Tests;

public sealed class EnumerableExtensionsTests
{
    [Fact]
    public void NotNull_FiltersNullElements()
    {
        IEnumerable<string?> source = ["a", null, "b", null, "c"];

        var result = source.NotNull().ToList();

        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void NotNullOrEmpty_FiltersBlankStrings()
    {
        string?[] values = ["a", string.Empty, " ", null, "b"];
        var source = values.Where(v => v is not null).Select(v => v!);

        var result = source.NotNullOrEmpty().ToList();

        Assert.Equal(["a", "b"], result);
    }

    [Fact]
    public void ToObservableCollection_ReusesExistingInstance()
    {
        var existing = new ObservableCollection<int> { 1, 2 };

        var result = existing.ToObservableCollection();

        Assert.Same(existing, result);
    }

    [Fact]
    public void ToObservableCollection_CreatesNewCollectionFromSequence()
    {
        var result = new[] { 1, 2, 3 }.ToObservableCollection();

        Assert.Equal([1, 2, 3], result);
    }

    [Fact]
    public void ForEach_InvokesActionForEachElement()
    {
        var values = new List<int>();
        var source = new[] { 1, 2, 3 };

        source.ForEach(values.Add);

        Assert.Equal([1, 2, 3], values);
    }

    [Fact]
    public void ForEach_WithIndex_InvokesActionWithIndex()
    {
        var indexes = new List<int>();
        var source = new[] { 10, 20, 30 };

        source.ForEach((_, index) => indexes.Add(index));

        Assert.Equal([0, 1, 2], indexes);
    }

    [Fact]
    public void Sum_AggregatesTimeSpans()
    {
        var source = new[]
        {
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(2),
            TimeSpan.FromMinutes(3)
        };

        var total = source.Sum(x => x);

        Assert.Equal(TimeSpan.FromMinutes(6), total);
    }

    [Fact]
    public void Rotate_PositiveOffset_MovesElementsToEnd()
    {
        var source = new[] { 1, 2, 3, 4 };

        var result = source.Rotate(1).ToList();

        Assert.Equal([2, 3, 4, 1], result);
    }

    [Fact]
    public void Rotate_NegativeOffset_RotatesBackward()
    {
        var source = new[] { 1, 2, 3, 4 };

        var result = source.Rotate(-1).ToList();

        Assert.Equal([4, 1, 2, 3], result);
    }

    [Fact]
    public void Rotate_EmptySequence_ReturnsEmpty() => Assert.Empty(Array.Empty<int>().Rotate(3));

    [Fact]
    public void AverageOrDefault_IntSequence_ReturnsExpectedAverage()
    {
        Assert.Equal(2.0, new[] { 1, 2, 3 }.AverageOrDefault());
        Assert.Equal(0.0, Array.Empty<int>().AverageOrDefault());
    }

    [Fact]
    public void AverageOrDefault_DoubleSequence_ReturnsExpectedAverage()
    {
        Assert.Equal(2.0, new[] { 1.0, 2.0, 3.0 }.AverageOrDefault());
        Assert.Equal(7.0, Array.Empty<double>().AverageOrDefault(x => x, 7.0));
    }

    [Fact]
    public void AverageOrDefault_DecimalSequence_ReturnsExpectedAverage()
    {
        Assert.Equal(2.0m, new[] { 1.0m, 2.0m, 3.0m }.AverageOrDefault());
        Assert.Equal(4.0m, Array.Empty<decimal>().AverageOrDefault(x => x, 4.0m));
    }

    [Fact]
    public void AverageOrDefault_WithSelector_ReturnsProjectedAverage()
    {
        var source = new[] { "10", "20", "30" };

        var average = source.AverageOrDefault(int.Parse, defaultValue: 99);

        Assert.Equal(20, average);
    }

    [Fact]
    public void MaxOrDefault_EmptySequence_ReturnsDefault()
    {
        var source = Array.Empty<int>();

        var max = source.MaxOrDefault(x => x, defaultValue: 42);

        Assert.Equal(42, max);
    }

    [Fact]
    public void MaxOrDefault_NonEmptySequence_ReturnsMaximum()
    {
        var source = new[] { 1, 5, 3 };

        var max = source.MaxOrDefault(x => x);

        Assert.Equal(5, max);
    }

    [Fact]
    public void MinOrDefault_EmptySequence_ReturnsDefault()
    {
        var source = Array.Empty<int>();

        var min = source.MinOrDefault(x => x, defaultValue: 99);

        Assert.Equal(99, min);
    }

    [Fact]
    public void MinOrDefault_NonEmptySequence_ReturnsMinimum()
    {
        var source = new[] { 1, 5, 3 };

        var min = source.MinOrDefault(x => x);

        Assert.Equal(1, min);
    }

    [Fact]
    public void RoundRobin_GeneratesPairingsForEvenCount()
    {
        var source = new[] { 1, 2, 3, 4 };

        var rounds = source.RoundRobin().ToList();

        Assert.Equal(3, rounds.Count);
        Assert.All(rounds, round => Assert.Equal(2, round.Count()));
    }

    [Fact]
    public void GetByIdOrDefault_FindsMatchingItem()
    {
        var source = new[]
        {
            new IdentifiableItem(1, "first"),
            new IdentifiableItem(2, "second")
        };

        var item = source.GetByIdOrDefault(2);

        Assert.NotNull(item);
        Assert.Equal("second", item.Name);
    }

    [Fact]
    public void GetByIdOrDefault_WhenMissing_ReturnsNull()
    {
        var source = new[] { new IdentifiableItem(1, "first") };

        Assert.Null(source.GetByIdOrDefault(99));
    }

    [Fact]
    public void GetById_WhenFound_ReturnsItem()
    {
        var source = new[] { new IdentifiableItem(7, "found") };

        var item = source.GetById(7);

        Assert.Equal("found", item.Name);
    }

    [Fact]
    public void GetById_WhenMissing_ThrowsKeyNotFoundException()
    {
        var source = new[] { new IdentifiableItem(1, "first") };

        Assert.Throws<KeyNotFoundException>(() => source.GetById(42));
    }

    [Fact]
    public void HasId_ReturnsTrueWhenPresent()
    {
        var source = new[] { new IdentifiableItem(3, "third") };

        Assert.True(source.HasId(3));
        Assert.False(source.HasId(4));
    }

    private sealed record IdentifiableItem(int Id, string Name) : IIdentifiable<int>;
}
