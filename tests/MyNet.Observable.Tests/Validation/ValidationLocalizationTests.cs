// -----------------------------------------------------------------------
// <copyright file="ValidationLocalizationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyNet.Observable.Validation;
using Xunit;

namespace MyNet.Observable.Tests.Validation;

public sealed class ValidationLocalizationTests
{
    [Fact]
    public void ResolveDisplayName_WithMember_ReturnsMemberName()
    {
        Expression<Func<SampleModel, string>> expression = m => m.Name;

        var member = ((MemberExpression)expression.Body).Member;
        var result = ValidationLocalization.ResolveDisplayName(typeof(SampleModel), member, expression);

        Assert.Equal("Name", result);
    }

    [Fact]
    public void ResolveDisplayName_WithoutMember_ReturnsNull()
    {
        Expression<Func<SampleModel, string>> expression = m => m.Name;

        var result = ValidationLocalization.ResolveDisplayName(typeof(SampleModel), null, expression);

        Assert.Null(result);
    }

    private sealed class SampleModel
    {
        public string Name { get; } = string.Empty;
    }
}
