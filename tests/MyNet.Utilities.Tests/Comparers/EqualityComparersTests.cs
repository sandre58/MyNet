// -----------------------------------------------------------------------
// <copyright file="EqualityComparersTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MyNet.Primitives.Comparers;
using Xunit;

namespace MyNet.Utilities.Tests.Comparers;

public class EqualityComparersTests
{
    #region PredicateEqualityComparer

    [Fact]
    public void Predicate_GenericEquals_ReturnsTrueWhenPredicatePasses()
    {
        IEqualityComparer<string> comparer = new PredicateEqualityComparer<string>((a, b) => a.Length == b.Length);
        Assert.True(comparer.Equals("abc", "xyz")); // same length
    }

    [Fact]
    public void Predicate_GenericEquals_ReturnsFalseWhenPredicateFails()
    {
        IEqualityComparer<string> comparer = new PredicateEqualityComparer<string>((a, b) => a == b);
        Assert.False(comparer.Equals("abc", "xyz"));
    }

    [Fact]
    public void Predicate_GenericEquals_ReturnsFalseForSameReference()
    {
        const string obj = "hello";
        IEqualityComparer<string> comparer = new PredicateEqualityComparer<string>((_, _) => true);

        // Same reference → always false (ReferenceEquals guard)
        Assert.False(comparer.Equals(obj, obj));
    }

    [Fact]
    public void Predicate_GenericEquals_ReturnsFalseForNulls()
    {
        IEqualityComparer<string> comparer = new PredicateEqualityComparer<string>((_, _) => true);
        Assert.False(comparer.Equals(null, "abc"));
        Assert.False(comparer.Equals("abc", null));
    }

    [Fact]
    public void Predicate_NonGenericEquals_ReturnsTrueWhenPredicatePasses()
    {
        System.Collections.IEqualityComparer comparer = new PredicateEqualityComparer<string>((a, b) => a.Length == b.Length);
        Assert.True(comparer.Equals("abc", "xyz"));
    }

    [Fact]
    public void Predicate_GetHashCode_ReturnsIdentityBasedHash()
    {
        var comparer = new PredicateEqualityComparer<string>((a, b) => a == b);
        const string obj = "hello";
        Assert.Equal(RuntimeHelpers.GetHashCode(obj), comparer.GetHashCode(obj));
    }

    #endregion

    #region ReferenceEqualityComparer

    [Fact]
    public void Reference_Instance_IsSingleton() => Assert.Same(MyNet.Primitives.Comparers.ReferenceEqualityComparer.Instance, MyNet.Primitives.Comparers.ReferenceEqualityComparer.Instance);

    [Fact]
    public void Reference_GenericEquals_ReturnsTrueForSameReference()
    {
        var obj = new object();
        IEqualityComparer<object> comparer = MyNet.Primitives.Comparers.ReferenceEqualityComparer.Instance;
        Assert.True(comparer.Equals(obj, obj));
    }

    [Fact]
    public void Reference_GenericEquals_ReturnsFalseForDifferentInstances()
    {
        IEqualityComparer<object> comparer = MyNet.Primitives.Comparers.ReferenceEqualityComparer.Instance;
        Assert.False(comparer.Equals(new(), new()));
    }

    [Fact]
    public void Reference_NonGenericEquals_ReturnsTrueForSameReference()
    {
        var obj = new object();
        System.Collections.IEqualityComparer comparer = MyNet.Primitives.Comparers.ReferenceEqualityComparer.Instance;
        Assert.True(comparer.Equals(obj, obj));
    }

    [Fact]
    public void Reference_GetHashCode_ReturnsRuntimeHashCode()
    {
        var obj = new object();
        IEqualityComparer<object> comparer = MyNet.Primitives.Comparers.ReferenceEqualityComparer.Instance;
        Assert.Equal(RuntimeHelpers.GetHashCode(obj), comparer.GetHashCode(obj));
    }

    #endregion
}
