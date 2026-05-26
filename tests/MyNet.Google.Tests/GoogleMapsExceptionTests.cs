// -----------------------------------------------------------------------
// <copyright file="GoogleMapsExceptionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Google.Maps;
using Xunit;

namespace MyNet.Google.Tests;

public sealed class GoogleMapsExceptionTests
{
    [Fact]
    public void RequestDeniedException_PreservesMessage()
    {
        const string message = "API key invalid";
        var exception = new RequestDeniedException(message);

        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void QueryLimitExceededException_PreservesMessage()
    {
        const string message = "Quota exceeded";
        var exception = new QueryLimitExceededException(message);

        Assert.Equal(message, exception.Message);
    }
}
