// -----------------------------------------------------------------------
// <copyright file="ComparerExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Primitives.Comparers;
using Xunit;

namespace MyNet.Primitives.Tests.Comparers;

public sealed class ComparerExtensionsTests
{
    [Fact]
    public void Reverse_InvertsComparerOrder()
    {
        var comparer = Comparer<int>.Default.Reverse();

        Assert.True(comparer.Compare(2, 1) < 0);
        Assert.True(comparer.Compare(1, 2) > 0);
    }

    [Fact]
    public void CultureInfoNameComparer_EqualsByName()
    {
        var comparer = CultureInfoNameComparer.Instance;
        var enUs = new CultureInfo("en-US");
        var enUsClone = new CultureInfo("en-us");

        Assert.True(comparer.Equals(enUs, enUsClone));
        Assert.Equal(comparer.GetHashCode(enUs), comparer.GetHashCode(enUsClone));
    }

    [Fact]
    public void CultureInfoNameComparer_GetHashCode_Null_Throws() => Assert.Throws<ArgumentNullException>(() => CultureInfoNameComparer.Instance.GetHashCode(null!));
}
