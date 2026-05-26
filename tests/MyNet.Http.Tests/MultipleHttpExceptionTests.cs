// -----------------------------------------------------------------------
// <copyright file="MultipleHttpExceptionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using MyNet.Http.Exceptions;
using Xunit;

namespace MyNet.Http.Tests;

public sealed class MultipleHttpExceptionTests
{
    [Fact]
    public void Constructor_WithHttpErrors_PreservesErrors()
    {
        var errors = new[]
        {
            new HttpError("400", "Bad request"),
            new HttpError("500", "Server error")
        };

        var ex = new MultipleHttpException(errors);

        Assert.Equal(2, ex.Errors.Count);
        Assert.Equal("400", ex.Errors.First().Code);
        Assert.Equal("Server error", ex.Errors.Last().Message);
    }

    [Fact]
    public void HttpError_ToString_ReturnsMessage()
    {
        var error = new HttpError("404", "Not found");

        Assert.Equal("Not found", error.ToString());
        Assert.Equal("404", error.Code);
    }
}
