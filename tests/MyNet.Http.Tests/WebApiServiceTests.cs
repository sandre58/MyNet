// -----------------------------------------------------------------------
// <copyright file="WebApiServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Http.Exceptions;
using Xunit;

namespace MyNet.Http.Tests;

public class WebApiServiceTests
{
    [Fact]
    public async Task GetDataAsyncDeserializesJsonResponseAsync()
    {
        using var handler = new StubHttpMessageHandler((_, _) => new(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"value\":\"hello\"}", Encoding.UTF8, "application/json")
        });
        using var client = new HttpClient(handler);
        client.BaseAddress = new("https://example.com/");
        using var service = new WebApiService(client);

        var result = await service.GetDataAsync<ResponseDto>("/data", CancellationToken.None);

        Assert.Equal("hello", result.Value);
    }

    [Fact]
    public async Task ErrorResponseThrowsWebApiExceptionWithStatusAndBodyAsync()
    {
        using var handler = new StubHttpMessageHandler((_, _) => new(HttpStatusCode.BadRequest)
        {
            ReasonPhrase = "Bad Request",
            Content = new StringContent("invalid payload", Encoding.UTF8, "text/plain")
        });
        using var client = new HttpClient(handler);
        client.BaseAddress = new("https://example.com/");
        using var service = new WebApiService(client);

        var ex = await Assert.ThrowsAsync<WebApiException>(() => service.GetDataAsync<ResponseDto>("/data", CancellationToken.None));

        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("invalid payload", ex.ResponseBody);
    }

    [Fact]
    public async Task ProblemDetailsResponseThrowsMultipleHttpExceptionAsync()
    {
        const string json = """
            {
              "title": "Validation failed",
              "errors": {
                "name": ["Name is required"]
              }
            }
            """;

        using var handler = new StubHttpMessageHandler((_, _) => new(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/problem+json")
        });
        using var client = new HttpClient(handler);
        client.BaseAddress = new("https://example.com/");
        using var service = new WebApiService(client);

        var ex = await Assert.ThrowsAsync<MultipleHttpException>(() => service.GetDataAsync<ResponseDto>("/data", CancellationToken.None));

        Assert.Equal("Validation failed", ex.Message);
        Assert.Contains(ex.Errors, error => error.Message == "Name is required");
    }

    [Fact]
    public async Task PostDataAsyncWithoutTypedResponseSucceedsAsync()
    {
        HttpMethod? method = null;
        using var handler = new StubHttpMessageHandler((request, _) =>
        {
            method = request.Method;
            return new(HttpStatusCode.NoContent);
        });
        using var client = new HttpClient(handler);
        client.BaseAddress = new("https://example.com/");
        using var service = new WebApiService(client);

        await service.PostDataAsync("/items", new { Id = 1 }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, method);
    }

    [Fact]
    public async Task PerRequestTimeoutThrowsTimeoutExceptionAsync()
    {
        using var handler = new StubHttpMessageHandler(async (_, cancellationToken) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);
            return new(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"value\":\"late\"}", Encoding.UTF8, "application/json")
            };
        });
        using var client = new HttpClient(handler);
        client.BaseAddress = new("https://example.com/");
        using var service = new WebApiService(client, timeout: TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAsync<TimeoutException>(() => service.GetDataAsync<ResponseDto>("/slow", CancellationToken.None));
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "DTO for testing deserialization")]
    private sealed record ResponseDto(string Value);

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        : HttpMessageHandler
    {
        public StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
            : this((request, token) => Task.FromResult(handler(request, token)))
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            handler(request, cancellationToken);
    }
}
