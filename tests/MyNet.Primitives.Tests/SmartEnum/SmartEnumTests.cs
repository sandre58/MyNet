// -----------------------------------------------------------------------
// <copyright file="SmartEnumTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyNet.Primitives.Tests.SmartEnum;

public class SmartEnumTests
{
    [Fact]
    [SuppressMessage("Performance", "CA2263:Prefer generic overload when type is known", Justification = "Tests the Type-based overload behavior.")]
    public void GetAll_FromType_ReturnsDiscoveredStaticInstances()
    {
        var result = SmartEnumSource.GetAll(typeof(TestSmartEnum));

        Assert.Contains(TestSmartEnum.One, result);
        Assert.Contains(TestSmartEnum.Two, result);
    }

    [Fact]
    public void GetAll_Generic_ReturnsDiscoveredStaticInstances()
    {
        var result = SmartEnumSource.GetAll<TestSmartEnum>();

        Assert.Equal(2, result.Count);
        Assert.Contains(TestSmartEnum.One, result);
        Assert.Contains(TestSmartEnum.Two, result);
    }

    [Fact]
    [SuppressMessage("Performance", "CA2263:Prefer generic overload when type is known", Justification = "Tests the Type-based overload behavior.")]
    public void GetAll_FromType_WhenNotSmartEnum_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => SmartEnumSource.GetAll(typeof(string)));

    [Fact]
    public void GetAll_FromType_WhenTypeIsNull_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => SmartEnumSource.GetAll(null!));

    [Fact]
    [SuppressMessage("Performance", "CA2263:Prefer generic overload when type is known", Justification = "Tests the Type-based overload behavior.")]
    public void GetAll_FromType_ForCustomSmartEnum_DiscoversStaticFields()
    {
        var result = SmartEnumSource.GetAll(typeof(CustomSmartEnum));

        Assert.Equal(2, result.Count);
        Assert.Contains(CustomSmartEnum.Alpha, result);
        Assert.Contains(CustomSmartEnum.Beta, result);
    }

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

    private sealed class TestSmartEnum(int value, string name) : SmartEnum<TestSmartEnum, int>(value)
    {
        public static readonly TestSmartEnum One = new(1, "One");

        public static readonly TestSmartEnum Two = new(2, "Two");

        public override string Name { get; } = name;
    }

    private sealed class DuplicateSmartEnum(int value) : SmartEnum<DuplicateSmartEnum, int>(value);

    private sealed class CustomSmartEnum(string name) : ISmartEnum
    {
        public static readonly CustomSmartEnum Alpha = new("Alpha");

        public static readonly CustomSmartEnum Beta = new("Beta");

        public string Name { get; } = name;
    }
}
