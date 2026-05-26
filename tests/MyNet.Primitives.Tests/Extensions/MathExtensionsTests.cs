// -----------------------------------------------------------------------
// <copyright file="MathExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MyNet.Primitives.Tests.Extensions;

public class MathExtensionsTests
{
    #region int extensions

    [Theory]
    [InlineData(0, true)]
    [InlineData(2, true)]
    [InlineData(-4, true)]
    [InlineData(1, false)]
    [InlineData(3, false)]
    public void IsEven_ReturnsExpected(int value, bool expected)
        => Assert.Equal(expected, value.IsEven());

    [Theory]
    [InlineData(1, true)]
    [InlineData(3, true)]
    [InlineData(0, false)]
    [InlineData(2, false)]
    public void IsOdd_ReturnsExpected(int value, bool expected)
        => Assert.Equal(expected, value.IsOdd());

    [Fact]
    public void Repeat_ExecutesActionNTimes()
    {
        var count = 0;
        5.Repeat(_ => count++);
        Assert.Equal(5, count);
    }

    [Fact]
    public void Repeat_PassesIndexToAction()
    {
        var indices = new List<int>();
        3.Repeat(indices.Add);
        Assert.Equal([0, 1, 2], indices);
    }

    [Fact]
    public void Repeat_ZeroTimes_DoesNotExecute()
    {
        var count = 0;
        0.Repeat(_ => count++);
        Assert.Equal(0, count);
    }

    [Fact]
    public void Repeat_NegativeValue_ThrowsArgumentOutOfRangeException()
        => Assert.Throws<ArgumentOutOfRangeException>(() => (-1).Repeat(_ => { }));

    [Fact]
    public void Repeat_NullAction_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => 3.Repeat(null!));

    [Fact]
    public void Range_Int_WithDefaultArgs_ReturnsZeroToValue()
    {
        var result = 4.Range().ToList();
        Assert.Equal([0, 1, 2, 3, 4], result);
    }

    [Fact]
    public void Range_Int_WithMinAndStep_ReturnsExpectedSequence()
    {
        var result = 10.Range(2, 3).ToList();
        Assert.Equal([2, 5, 8], result);
    }

    [Fact]
    public void Range_Int_NegativeStep_ReturnsDescending()
    {
        var result = (-3).Range(0, -1).ToList();
        Assert.Equal([0, -1, -2, -3], result);
    }

    [Fact]
    public void Range_Int_ZeroStep_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => 5.Range(0, 0).ToList());

    [Theory]
    [InlineData(5, 1, 10, 5)]
    [InlineData(0, 1, 10, 1)]
    [InlineData(15, 1, 10, 10)]
    [InlineData(5, 10, 1, 5)] // min > max: should be swapped
    public void SafeClamp_Int_ReturnsExpected(int value, int min, int max, int expected)
        => Assert.Equal(expected, value.SafeClamp(min, max));

    #endregion

    #region double extensions

    [Fact]
    public void IsCloseTo_SameValue_ReturnsTrue()
        => Assert.True(1.0.IsCloseTo(1.0));

    [Fact]
    public void IsCloseTo_VeryClose_ReturnsTrue()
        => Assert.True(1.0.IsCloseTo(1.0 + MathExtensions.DblEpsilon));

    [Fact]
    public void IsCloseTo_FarApart_ReturnsFalse()
        => Assert.False(1.0.IsCloseTo(2.0));

    [Fact]
    public void IsCloseTo_NaN_ReturnsFalse()
    {
        Assert.False(double.NaN.IsCloseTo(double.NaN));
        Assert.False(1.0.IsCloseTo(double.NaN));
    }

    [Fact]
    public void IsCloseTo_Infinity_ReturnsTrueForSameInfinity()
    {
        Assert.True(double.PositiveInfinity.IsCloseTo(double.PositiveInfinity));
        Assert.False(double.PositiveInfinity.IsCloseTo(double.NegativeInfinity));
    }

    [Fact]
    public void IsOne_ReturnsTrueForOne()
        => Assert.True(1.0.IsOne());

    [Fact]
    public void IsOne_ReturnsFalseForTwo()
        => Assert.False(2.0.IsOne());

    [Fact]
    public void IsZero_ReturnsTrueForZero()
        => Assert.True(0.0.IsZero());

    [Fact]
    public void IsZero_ReturnsFalseForOne()
        => Assert.False(1.0.IsZero());

    [Fact]
    public void LessThan_WhenStrictlyLess_ReturnsTrue()
        => Assert.True(1.0.IsLessThan(2.0));

    [Fact]
    public void LessThan_WhenClose_ReturnsFalse()
        => Assert.False(1.0.IsLessThan(1.0 + MathExtensions.DblEpsilon));

    [Fact]
    public void GreaterThan_WhenClose_ReturnsFalse()
        => Assert.False(1.0.IsGreaterThan(1.0 - MathExtensions.DblEpsilon));

    [Fact]
    public void GreaterThan_WhenStrictlyGreater_ReturnsTrue()
        => Assert.True(2.0.IsGreaterThan(1.0));

    [Fact]
    public void LessThanOrClose_WhenClose_ReturnsTrue()
        => Assert.True(1.0.IsLessThanOrClose(1.0));

    [Fact]
    public void GreaterThanOrClose_WhenClose_ReturnsTrue()
        => Assert.True(1.0.IsGreaterThanOrClose(1.0));

    [Theory]
    [InlineData(5.0, 1.0, 10.0, 5.0)]
    [InlineData(0.0, 1.0, 10.0, 1.0)]
    [InlineData(15.0, 1.0, 10.0, 10.0)]
    public void SafeClamp_Double_ReturnsExpected(double value, double min, double max, double expected)
        => Assert.Equal(expected, value.SafeClamp(min, max));

    [Fact]
    public void Range_Double_ReturnsExpectedSequence()
    {
        var result = 0.0.Range(1.0, 0.5).ToList();
        Assert.Equal(3, result.Count);
        Assert.Equal(0.0, result[0]);
        Assert.Equal(0.5, result[1], 10);
        Assert.Equal(1.0, result[2], 10);
    }

    [Fact]
    public void Range_Double_ZeroStep_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => 0.0.Range(1.0, 0.0).ToList());

    #endregion

    #region float extensions

    [Fact]
    public void Float_IsCloseTo_SameValue_ReturnsTrue()
        => Assert.True(1.0f.IsCloseTo(1.0f));

    [Fact]
    public void Float_IsCloseTo_NaN_ReturnsFalse()
    {
        Assert.False(float.NaN.IsCloseTo(float.NaN));
        Assert.False(1.0f.IsCloseTo(float.NaN));
    }

    [Fact]
    public void Float_IsCloseTo_SameInfinity_ReturnsTrue()
        => Assert.True(float.PositiveInfinity.IsCloseTo(float.PositiveInfinity));

    [Fact]
    public void Float_IsOne_ReturnsTrueForOne()
        => Assert.True(1.0f.IsOne());

    [Fact]
    public void Float_IsZero_ReturnsTrueForZero()
        => Assert.True(0.0f.IsZero());

    [Fact]
    public void Float_LessThan_WhenStrictlyLess_ReturnsTrue()
        => Assert.True(1.0f.IsLessThan(2.0f));

    [Fact]
    public void Float_GreaterThan_WhenStrictlyGreater_ReturnsTrue()
        => Assert.True(2.0f.IsGreaterThan(1.0f));

    [Fact]
    public void Float_LessThanOrClose_WhenClose_ReturnsTrue()
        => Assert.True(1.0f.IsLessThanOrClose(1.0f));

    [Fact]
    public void Float_GreaterThanOrClose_WhenClose_ReturnsTrue()
        => Assert.True(1.0f.IsGreaterThanOrClose(1.0f));

    [Theory]
    [InlineData(5.0f, 1.0f, 10.0f, 5.0f)]
    [InlineData(0.0f, 1.0f, 10.0f, 1.0f)]
    [InlineData(15.0f, 1.0f, 10.0f, 10.0f)]
    public void SafeClamp_Float_ReturnsExpected(float value, float min, float max, float expected)
        => Assert.Equal(expected, value.SafeClamp(min, max));

    #endregion

    #region nullable extensions

    [Fact]
    public void NullableDouble_IsCloseTo_BothNull_ReturnsTrue()
    {
        double? a = null;
        double? b = null;
        Assert.True(a.IsCloseTo(b));
    }

    [Fact]
    public void NullableDouble_IsCloseTo_OneNull_ReturnsFalse()
    {
        double? a = 1.0;
        double? b = null;
        Assert.False(a.IsCloseTo(b));
    }

    [Fact]
    public void NullableDouble_IsCloseTo_BothSame_ReturnsTrue()
    {
        double? a = 1.0;
        double? b = 1.0;
        Assert.True(a.IsCloseTo(b));
    }

    [Fact]
    public void NullableFloat_IsCloseTo_BothNull_ReturnsTrue()
    {
        float? a = null;
        float? b = null;
        Assert.True(a.IsCloseTo(b));
    }

    #endregion

    #region IEnumerable<double> extensions

    [Fact]
    public void AnyNan_WithNanValue_ReturnsTrue()
        => Assert.True(new[] { 1.0, double.NaN, 3.0 }.AnyNan());

    [Fact]
    public void AnyNan_WithoutNan_ReturnsFalse()
        => Assert.False(new[] { 1.0, 2.0, 3.0 }.AnyNan());

    [Fact]
    public void AnyInfinity_WithInfinity_ReturnsTrue()
        => Assert.True(new[] { 1.0, double.PositiveInfinity }.AnyInfinity());

    [Fact]
    public void AnyInfinity_WithoutInfinity_ReturnsFalse()
        => Assert.False(new[] { 1.0, 2.0 }.AnyInfinity());

    #endregion

    #region decimal extensions

    [Theory]
    [InlineData(5.0, 1.0, 10.0, 5.0)]
    [InlineData(0.0, 1.0, 10.0, 1.0)]
    [InlineData(15.0, 1.0, 10.0, 10.0)]
    public void SafeClamp_Decimal_ReturnsExpected(double valueD, double minD, double maxD, double expectedD)
    {
        var value = (decimal)valueD;
        var min = (decimal)minD;
        var max = (decimal)maxD;
        var expected = (decimal)expectedD;
        Assert.Equal(expected, value.SafeClamp(min, max));
    }

    #endregion

    #region ExtractDouble

    [Fact]
    public void ExtractDouble_Null_ReturnsNaN()
        => Assert.Equal(double.NaN, MathExtensions.ExtractDouble(null));

    [Fact]
    public void ExtractDouble_Double_ReturnsValue()
        => Assert.Equal(3.14, 3.14.ExtractDouble());

    [Fact]
    public void ExtractDouble_Infinity_ReturnsInfinity()
        => Assert.Equal(double.PositiveInfinity, double.PositiveInfinity.ExtractDouble());

    [Fact]
    public void ExtractDouble_Float_ReturnsValue()
        => Assert.Equal(1.5f, 1.5f.ExtractDouble(), 5);

    [Fact]
    public void ExtractDouble_Int_ReturnsValue()
        => Assert.Equal(42.0, 42.ExtractDouble());

    [Fact]
    public void ExtractDouble_InvalidString_ThrowsFormatException()
        => Assert.Throws<FormatException>(() => "hello".ExtractDouble());

    #endregion
}
