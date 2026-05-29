// -----------------------------------------------------------------------
// <copyright file="ExpressionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;

namespace MyNet.Primitives.Tests.Extensions;

public sealed class ExpressionExtensionsTests
{
    [Fact]
    public void IsSimilar_ComparesExpressionStrings()
    {
        Expression<Func<Sample, int>> left = x => x.Id;
        Expression<Func<Sample, int>> right = x => x.Id;

        Assert.True(left.IsSimilar(right));

        Expression<Func<Sample, string>> different = x => x.Name;
        Assert.False(left.IsSimilar(different));
    }

    [Fact]
    public void PropertyName_ReturnsNestedPath()
    {
        Expression<Func<Sample, string>> expression = x => x.Address.City;

        Assert.Equal("Address.City", expression.PropertyName());
    }

    [Fact]
    public void PropertyName_InvalidExpression_Throws()
    {
        Expression<Func<Sample, int>> expression = x => x.Id + 1;

        Assert.Throws<InvalidOperationException>(expression.PropertyName);
    }

    [Fact]
    public void GetMembers_ReturnsMemberStack()
    {
        Expression<Func<Sample, string>> expression = x => x.Address.City;

        var members = expression.GetMembers();

        Assert.Equal(2, members.Count);
        Assert.Equal("Address", members.Pop().Name);
        Assert.Equal("City", members.Pop().Name);
    }

    [Fact]
    public void PropertyInfo_ReturnsPropertyMetadata()
    {
        Expression<Func<Sample, string>> expression = x => x.Name;

        Assert.Equal(nameof(Sample.Name), expression.PropertyInfo.Name);
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used for testing expression parsing.")]
    private sealed class Sample
    {
        public int Id { get; } = 1;

        public string Name { get; } = string.Empty;

        public Address Address { get; } = new();
    }

    private sealed class Address
    {
        public string City { get; } = string.Empty;
    }
}
