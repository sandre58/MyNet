// -----------------------------------------------------------------------
// <copyright file="ComparableExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Primitives.Tests.Comparison;

public class ComparableExtensionsTests
{
    [Theory]
    [InlineData(ComparableOperator.EqualsTo, true)]
    [InlineData(ComparableOperator.NotEqualsTo, false)]
    [InlineData(ComparableOperator.LessThan, false)]
    [InlineData(ComparableOperator.GreaterThan, false)]
    [InlineData(ComparableOperator.LessEqualThan, true)]
    [InlineData(ComparableOperator.GreaterEqualThan, true)]
    public void Compare_SameValue_UsesExpectedSimpleOperatorResult(ComparableOperator sign, bool expected)
    {
        IComparable left = 10;
        IComparable right = 10;

        Assert.Equal(expected, left.Compare(right, sign));
    }

    [Fact]
    public void Compare_DifferentValue_UsesExpectedSimpleOperatorResult()
    {
        IComparable left = 10;
        IComparable right = 5;

        Assert.True(left.Compare(right, ComparableOperator.GreaterThan));
        Assert.True(left.Compare(right, ComparableOperator.NotEqualsTo));
        Assert.False(left.Compare(right, ComparableOperator.EqualsTo));
        Assert.False(left.Compare(right, ComparableOperator.LessThan));
    }

    [Fact]
    public void Compare_SimpleOperator_WhenAnyValueIsNull_ReturnsFalse()
    {
        IComparable? left = null;
        IComparable right = 5;

        Assert.False(left.Compare(right, ComparableOperator.EqualsTo));
        Assert.False(right.Compare(null, ComparableOperator.EqualsTo));
    }

    [Fact]
    public void Compare_SimpleOperator_WithInvalidOperator_ThrowsArgumentException()
    {
        IComparable left = 10;

        Assert.Throws<ArgumentException>(() => left.Compare(5, (ComparableOperator)999));
    }

    [Theory]
    [InlineData(ComplexComparableOperator.IsBetween, true)]
    [InlineData(ComplexComparableOperator.IsNotBetween, false)]
    [InlineData(ComplexComparableOperator.EqualsTo, false)]
    [InlineData(ComplexComparableOperator.NotEqualsTo, true)]
    [InlineData(ComplexComparableOperator.LessThan, true)]
    [InlineData(ComplexComparableOperator.GreaterThan, true)]
    [InlineData(ComplexComparableOperator.LessEqualThan, true)]
    [InlineData(ComplexComparableOperator.GreaterEqualThan, true)]
    public void Compare_ComplexOperator_UsesExpectedResult(ComplexComparableOperator sign, bool expected)
    {
        IComparable value = 5;

        Assert.Equal(expected, value.Compare(3, 7, sign));
    }

    [Fact]
    public void Compare_ComplexOperator_WhenAnyValueIsNull_ReturnsFalse()
    {
        IComparable? value = null;

        Assert.False(value.Compare(3, 7, ComplexComparableOperator.IsBetween));
        Assert.False(((IComparable?)5).Compare(null, 7, ComplexComparableOperator.IsBetween));
        Assert.False(((IComparable?)5).Compare(3, null, ComplexComparableOperator.IsBetween));
    }

    [Fact]
    public void Compare_ComplexOperator_WithInvalidOperator_ThrowsArgumentException()
    {
        IComparable value = 5;

        Assert.Throws<ArgumentException>(() => value.Compare(3, 7, (ComplexComparableOperator)999));
    }

    [Theory]
    [InlineData(ComplexComparableOperator.IsBetween, true)]
    [InlineData(ComplexComparableOperator.IsNotBetween, false)]
    [InlineData(ComplexComparableOperator.EqualsTo, false)]
    [InlineData(ComplexComparableOperator.NotEqualsTo, true)]
    [InlineData(ComplexComparableOperator.LessThan, true)]
    [InlineData(ComplexComparableOperator.GreaterThan, true)]
    [InlineData(ComplexComparableOperator.LessEqualThan, true)]
    [InlineData(ComplexComparableOperator.GreaterEqualThan, true)]
    public void Compare_GenericOverload_UsesExpectedResult(ComplexComparableOperator sign, bool expected)
    {
        IComparable<int> value = 5;

        Assert.Equal(expected, value.Compare(3, 7, sign));
    }

    [Fact]
    public void Compare_GenericOverload_WithInvalidOperator_ThrowsArgumentException()
    {
        IComparable<int> value = 5;

        Assert.Throws<ArgumentException>(() => value.Compare(3, 7, (ComplexComparableOperator)999));
    }

    [Fact]
    public void CompareNullableTo_HandlesNullAndValueInputs()
    {
        IComparable<int> value = 7;
        IComparable<int>? nullValue = null;

        Assert.Equal(0, value.CompareNullableTo(7));
        Assert.True(value.CompareNullableTo(6) > 0);
        Assert.True(value.CompareNullableTo(8) < 0);
        Assert.True(value.CompareNullableTo(null) > 0);
        Assert.True(nullValue.CompareNullableTo(1) < 0);
        Assert.Equal(0, nullValue.CompareNullableTo(null));
    }

    [Fact]
    public void MaxAndMin_ReturnExpectedValues()
    {
        IComparable<int> value = 7;
        IComparable<int>? nullValue = null;

        Assert.Equal(10, value.Max(10));
        Assert.Equal(3, value.Min(3));
        Assert.Equal(10, nullValue.Max(10));
        Assert.Equal(3, nullValue.Min(3));
    }

    [Fact]
    public void MinMax_ReturnsOrderedBounds()
    {
        IComparable<int> value = 7;

        Assert.Equal(new(3, 7), value.MinMax(3));
        Assert.Equal(new(7, 10), value.MinMax(10));
    }

    [Fact]
    public void MinMax_OptionalOverload_HandlesNullsAndOrdering()
    {
        IComparable<int> value = 7;
        IComparable<int>? nullValue = null;

        Assert.Equal(new(3, 7), value.MinMax((int?)3));
        Assert.Equal(new(null, 7), value.MinMax(null));
        Assert.Equal(new(null, 3), nullValue.MinMax((int?)3));
        Assert.Equal(new(null, null), nullValue.MinMax(null));
    }

    [Fact]
    public void SafeClamp_ReturnsExpectedValueOrThrowsForNull()
    {
        IComparable<int> value = 7;
        IComparable<int>? nullValue = null;

        Assert.Equal(7, value.SafeClamp(1, 10));
        Assert.Equal(8, value.SafeClamp(8, 10));
        Assert.Equal(6, value.SafeClamp(1, 6));
        Assert.Throws<InvalidCastException>(() => nullValue.SafeClamp(1, 10));
    }

    [Fact]
    public void RangeChecks_UseExclusiveAndInclusiveSemantics()
    {
        IComparable<int> value = 7;
        IComparable<int>? nullValue = null;

        Assert.True(value.IsBetween(1, 10));
        Assert.False(value.IsBetween(7, 10));
        Assert.False(value.IsBetween(1, 7));
        Assert.True(value.InRange(7, 10));
        Assert.True(value.InRange(1, 7));
        Assert.False(value.InRange(8, 10));
        Assert.False(nullValue.IsBetween(1, 10));
        Assert.False(nullValue.InRange(1, 10));
    }
}
