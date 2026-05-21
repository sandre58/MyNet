// -----------------------------------------------------------------------
// <copyright file="EnumExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Utilities.Tests.Enums;

public class EnumExtensionsTests
{
    [Flags]
    private enum TestFlags
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4,
        D = 8
    }

    [Fact]
    public void CountFlags_NoFlags_ReturnsOne()
    {
        // None (0) has HasFlag return true for None itself
        var result = TestFlags.None.CountFlags();
        Assert.Equal(1, result); // None.HasFlag(None) == true
    }

    [Fact]
    public void CountFlags_SingleFlag_ReturnsCorrectCount()
    {
        var result = TestFlags.A.CountFlags();

        // A.HasFlag(None) = true, A.HasFlag(A) = true → 2
        Assert.Equal(2, result);
    }

    [Fact]
    public void CountFlags_TwoFlags_ReturnsCorrectCount()
    {
        const TestFlags flags = TestFlags.A | TestFlags.B;
        var result = flags.CountFlags();

        // (A|B).HasFlag(None)=true, HasFlag(A)=true, HasFlag(B)=true → 3
        Assert.Equal(3, result);
    }

    [Fact]
    public void CountFlags_AllFlags_ReturnsAllCount()
    {
        const TestFlags flags = TestFlags.A | TestFlags.B | TestFlags.C | TestFlags.D;
        var result = flags.CountFlags();

        // None + A + B + C + D = 5
        Assert.Equal(5, result);
    }
}
