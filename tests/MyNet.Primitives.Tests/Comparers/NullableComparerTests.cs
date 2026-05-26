// -----------------------------------------------------------------------
// <copyright file="NullableComparerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Comparers;
using Xunit;

namespace MyNet.Primitives.Tests.Comparers;

public class NullableComparerTests
{
    [Fact]
    public void Compare_BothValuesAreNull_ReturnsZero()
    {
        // Arrange
        var comparer = new NullableComparer<int>();

        // Act
        var result = comparer.Compare(null, null);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Compare_OneValueIsNull_ReturnsOne()
    {
        // Arrange
        var comparer = new NullableComparer<int>();
        int? value = 5;

        // Act
        var result = comparer.Compare(value, null);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Compare_OtherValueIsNull_ReturnsNegativeOne()
    {
        // Arrange
        var comparer = new NullableComparer<int>();
        int? value = 5;

        // Act
        var result = comparer.Compare(null, value);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void Compare_BothValuesAreNotNull_ReturnsComparisonResult()
    {
        // Arrange
        var comparer = new NullableComparer<int>();
        int? value1 = 5;
        int? value2 = 10;

        // Act
        var result = comparer.Compare(value1, value2);

        // Assert
        Assert.True(result < 0); // value1 is less than value2
    }
}
