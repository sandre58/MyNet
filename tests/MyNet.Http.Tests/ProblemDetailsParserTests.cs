// -----------------------------------------------------------------------
// <copyright file="ProblemDetailsParserTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Http.Exceptions;
using Xunit;

namespace MyNet.Http.Tests;

public class ProblemDetailsParserTests
{
    [Fact]
    public void TryParseExceptionReturnsMultipleHttpException()
    {
        const string json = """
            {
              "title": "Validation failed",
              "errors": {
                "email": ["Email is invalid"]
              }
            }
            """;

        var exception = ProblemDetailsParser.TryParseException(json);

        var multiple = Assert.IsType<MultipleHttpException>(exception);
        Assert.Equal("Validation failed", multiple.Message);
        Assert.Contains(multiple.Errors, error => error is { Code: "email", Message: "Email is invalid" });
    }

    [Fact]
    public void TryParseExceptionReturnsNullForNonProblemDetails()
    {
        var exception = ProblemDetailsParser.TryParseException("plain text");

        Assert.Null(exception);
    }
}
