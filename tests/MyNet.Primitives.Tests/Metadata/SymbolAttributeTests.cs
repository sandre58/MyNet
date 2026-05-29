// -----------------------------------------------------------------------
// <copyright file="SymbolAttributeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Metadata;
using Xunit;

namespace MyNet.Primitives.Tests.Metadata;

public sealed class SymbolAttributeTests
{
    [Fact]
    public void Constructor_SetsSymbol()
    {
        var attribute = new SymbolAttribute("kg");

        Assert.Equal("kg", attribute.Value);
    }

    [Fact]
    public void AttributeUsage_AllowsEnumFields()
    {
        var usage = typeof(SymbolAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false);

        Assert.NotEmpty(usage);
    }
}
