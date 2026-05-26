// -----------------------------------------------------------------------
// <copyright file="SmartEnumTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyNet.Utilities.Tests.SmartEnum;

public class SmartEnumTests
{
    [Fact]
    public void All_ReturnsDiscoveredStaticInstances()
    {
        Assert.Contains(TestSmartEnum.One, TestSmartEnum.All);
        Assert.Contains(TestSmartEnum.Two, TestSmartEnum.All);
    }

    [Fact]
    public void TryFromValue_ReturnsMatch_WhenExists()
    {
        var ok = TestSmartEnum.TryFromValue(1, out var result);

        Assert.True(ok);
        Assert.Same(TestSmartEnum.One, result);
    }

    [Fact]
    public void TryFromValue_ReturnsFalse_WhenMissing()
    {
        var ok = TestSmartEnum.TryFromValue(999, out var result);

        Assert.False(ok);
        Assert.Null(result);
    }

    [Fact]
    public void FromValue_WhenMissing_ThrowsKeyNotFoundException()
        => Assert.Throws<KeyNotFoundException>(() => TestSmartEnum.FromValue(999));

    [Fact]
    [SuppressMessage("ReSharper", "EqualExpressionComparison", Justification = "Testing operator overloads")]
    public void Equality_And_Comparison_Operators_Work()
    {
        var left = TestSmartEnum.One;
        var right = TestSmartEnum.Two;
        var sameAsLeft = TestSmartEnum.FromValue(1);
        var sameAsRight = TestSmartEnum.FromValue(2);

        Assert.True(left == sameAsLeft);
        Assert.True(left != right);
        Assert.True(left < right);
        Assert.True(left <= sameAsLeft);
        Assert.True(right > left);
        Assert.True(right >= sameAsRight);
    }

    [Fact]
    public void CompareTo_Object_WrongType_ThrowsArgumentException()
    {
        IComparable comparable = TestSmartEnum.One;

        Assert.Throws<ArgumentException>(() => comparable.CompareTo("not an enum"));
    }

    [Fact]
    public void ToString_ReturnsName()
        => Assert.Equal("One", TestSmartEnum.One.ToString());

    [Fact]
    public void CreatingDuplicateValue_ThrowsInvalidOperationException()
    {
        var unique = Environment.TickCount;
        _ = new DuplicateSmartEnum(unique);

        Assert.Throws<InvalidOperationException>(() => _ = new DuplicateSmartEnum(unique));
    }

    private sealed class TestSmartEnum(int value, string name) : Primitives.SmartEnum<TestSmartEnum, int>(value)
    {
        public static readonly TestSmartEnum One = new(1, "One");

        public static readonly TestSmartEnum Two = new(2, "Two");

        public override string Name { get; } = name;
    }

    private sealed class DuplicateSmartEnum(int value) : Primitives.SmartEnum<DuplicateSmartEnum, int>(value);
}
