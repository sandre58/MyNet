// -----------------------------------------------------------------------
// <copyright file="ApiAndExceptionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Http.Exceptions;
using Xunit;

namespace MyNet.Http.Tests;

public class ApiAndExceptionTests
{
    [Fact]
    public void ApiParameterImplicitConversionToTuple()
    {
        ApiParameter value = new("k", "v");

        var (key, v) = value;

        Assert.Equal("k", key);
        Assert.Equal("v", v);
    }

    [Fact]
    public void ApiParameterImplicitConversionFromTuple()
    {
        (string Key, string Value) tuple = ("k", "v");

        ApiParameter value = tuple;

        Assert.Equal("k", value.Key);
        Assert.Equal("v", value.Value);
    }

    [Fact]
    public void MultipleHttpExceptionToStringIncludesMessageAndErrors()
    {
        var ex = new MultipleHttpException("Parent", ["E1", "E2"]);

        var result = ex.ToString();

        Assert.Contains("Parent", result, StringComparison.Ordinal);
        Assert.Contains(" - E1", result, StringComparison.Ordinal);
        Assert.Contains(" - E2", result, StringComparison.Ordinal);
    }
}
