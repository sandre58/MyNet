// -----------------------------------------------------------------------
// <copyright file="HttpRequestExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net.Http;
using Xunit;

namespace MyNet.Http.Tests;

public sealed class HttpRequestExtensionsTests
{
    [Fact]
    public void SetTimeout_StoresValueInRequestOptions()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var timeout = TimeSpan.FromSeconds(30);

        request.SetTimeout(timeout);

        Assert.Equal(timeout, request.GetTimeout());
    }

    [Fact]
    public void GetTimeout_WhenNotSet_ReturnsNull()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        Assert.Null(request.GetTimeout());
    }

    [Fact]
    public void SetTimeout_NullRequest_ThrowsArgumentNullException()
    {
        HttpRequestMessage? request = null;

        Assert.Throws<ArgumentNullException>(() => request!.SetTimeout(TimeSpan.FromSeconds(1)));
    }
}
