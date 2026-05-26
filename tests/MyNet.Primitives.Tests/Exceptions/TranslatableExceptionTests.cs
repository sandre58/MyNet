// -----------------------------------------------------------------------
// <copyright file="TranslatableExceptionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Exceptions;
using Xunit;

namespace MyNet.Primitives.Tests.Exceptions;

public class TranslatableExceptionTests
{
    [Fact]
    public void Constructor_WithResourceKey_SetsMessageAndParameters()
    {
        var exception = new TranslatableException("Error.Key", 12, "abc");

        Assert.Equal("Error.Key", exception.ResourceKey);
        Assert.Equal("Error.Key", exception.Message);
        Assert.Equal(2, exception.Parameters.Count);
        Assert.Equal(12, exception.Parameters[0]);
        Assert.Equal("abc", exception.Parameters[1]);
    }

    [Fact]
    public void Constructor_WithInnerException_SetsInnerExceptionAndParameters()
    {
        var inner = new InvalidOperationException("boom");

        var exception = new TranslatableException("Error.WithInner", inner, 42);

        Assert.Equal("Error.WithInner", exception.ResourceKey);
        Assert.Same(inner, exception.InnerException);
        Assert.Single(exception.Parameters);
        Assert.Equal(42, exception.Parameters[0]);
    }
}
