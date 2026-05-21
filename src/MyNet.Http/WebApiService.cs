// -----------------------------------------------------------------------
// <copyright file="WebApiService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Utilities;

namespace MyNet.Http;

/// <summary>
/// Service for sending HTTP requests to a web API.
/// </summary>
public sealed class WebApiService : IDisposable
{
    private readonly HttpClient _client;
    private readonly TimeSpan _timeout;
    private readonly Func<object?, Exception>? _toException;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiService"/> class.
    /// </summary>
    /// <param name="serverUrl">Target server URL.</param>
    /// <param name="timeout">Maximum request timeout duration.</param>
    /// <param name="headers">Custom HTTP headers.</param>
    /// <param name="toException">Function to convert a response to an exception.</param>
    public WebApiService(Uri? serverUrl = null, TimeSpan timeout = default, Dictionary<string, string>? headers = null, Func<object?, Exception>? toException = null)
    {
        _toException = toException;
        _timeout = timeout != default ? timeout : TimeSpan.FromMilliseconds(Timeout.Infinite);
        var handler = new TimeoutHandler
        {
            InnerHandler = new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip }
        };

        _client = new(handler)
        {
            BaseAddress = serverUrl,
            Timeout = _timeout
        };
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new("application/json"));
        _client.DefaultRequestHeaders.Accept.Add(new("application/problem+json"));
        _client.DefaultRequestHeaders.Accept.Add(new("text/plain"));
        _client.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));

        headers?.ForEach(x => _client.DefaultRequestHeaders.Add(x.Key, x.Value));
    }

    /// <summary>
    /// Gets a data stream from the specified URL asynchronously.
    /// </summary>
    public async Task<Stream> GetStreamAsync(string str) => await _client.GetStreamAsync(new Uri(str, UriKind.RelativeOrAbsolute)).ConfigureAwait(false);

    /// <summary>
    /// Gets a data stream from the specified URL synchronously.
    /// </summary>
    public Stream GetStream(string str) => _client.GetStreamAsync(new Uri(str, UriKind.RelativeOrAbsolute)).GetAwaiter().GetResult();

    /// <summary>
    /// Gets typed data from the specified URL with parameters asynchronously.
    /// </summary>
    public async Task<T> GetDataAsync<T>(string str, CancellationToken cancellationToken, params ApiParameter[] parameters) => await GetDataAsync<T>(ToWebUri(str, parameters), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Gets typed data from the specified URI asynchronously.
    /// </summary>
    public async Task<T> GetDataAsync<T>(Uri uri, CancellationToken cancellationToken = default) => await SendRequestAsync<T>(HttpMethod.Get, uri, cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends data using POST and gets a typed response asynchronously.
    /// </summary>
    public async Task<TReturn> PostDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters) => await PostDataAsync<TParam, TReturn>(ToWebUri(str, parameters), value, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends data using POST to the specified URI and gets a typed response asynchronously.
    /// </summary>
    public async Task<T> PostDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default) => await SendRequestAsync<T>(HttpMethod.Post, uri, CreateContent(value), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a POST request without content asynchronously.
    /// </summary>
    public async Task PostDataAsync(string str, CancellationToken cancellationToken, params ApiParameter[] parameters) => await PostDataAsync(ToWebUri(str, parameters), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a POST request to the specified URI without content asynchronously.
    /// </summary>
    public async Task PostDataAsync(Uri uri, CancellationToken cancellationToken = default) => await SendRequestAsync(HttpMethod.Post, uri, cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deletes data using DELETE with parameters asynchronously.
    /// </summary>
    public async Task DeleteDataAsync(string str, CancellationToken cancellationToken, params ApiParameter[] parameters) => await DeleteDataAsync(ToWebUri(str, parameters), cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deletes data using DELETE at the specified URI asynchronously.
    /// </summary>
    public async Task DeleteDataAsync(Uri uri, CancellationToken cancellationToken = default) => await SendRequestAsync(HttpMethod.Delete, uri, cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deletes data using DELETE with content and gets a typed response asynchronously.
    /// </summary>
    public async Task<TReturn> DeleteDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters) => await DeleteDataAsync<TParam, TReturn>(ToWebUri(str, parameters), value, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deletes data using DELETE at the specified URI with content and gets a typed response asynchronously.
    /// </summary>
    public async Task<T> DeleteDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default) => await SendRequestAsync<T>(HttpMethod.Delete, uri, CreateContent(value), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Gets typed data from the specified URL with parameters synchronously.
    /// </summary>
    public T GetData<T>(string str, params ApiParameter[] parameters) => GetData<T>(ToWebUri(str, parameters));

    /// <summary>
    /// Gets typed data from the specified URI synchronously.
    /// </summary>
    public T GetData<T>(Uri uri) => GetDataAsync<T>(uri).GetAwaiter().GetResult();

    /// <summary>
    /// Sends data using POST and gets a typed response synchronously.
    /// </summary>
    public TReturn PostData<TParam, TReturn>(string str, TParam value, params ApiParameter[] parameters) => PostData<TParam, TReturn>(ToWebUri(str, parameters), value);

    /// <summary>
    /// Sends data using POST to the specified URI and gets a typed response synchronously.
    /// </summary>
    public TReturn PostData<TParam, TReturn>(Uri uri, TParam value) => PostDataAsync<TParam, TReturn>(uri, value).GetAwaiter().GetResult();

    /// <summary>
    /// Sends data using POST without getting a typed response synchronously.
    /// </summary>
    public void PostData<TParam>(string str, TParam value) => PostDataAsync<TParam, bool>(str.ToWebUri(), value).GetAwaiter().GetResult();

    /// <summary>
    /// Sends data using POST to the specified URI without getting a typed response synchronously.
    /// </summary>
    public void PostData<TParam>(Uri uri, TParam value) => PostDataAsync<TParam, bool>(uri, value).GetAwaiter().GetResult();

    /// <summary>
    /// Sends a POST request without content synchronously.
    /// </summary>
    public void PostData(string str, params ApiParameter[] parameters) => PostData(ToWebUri(str, parameters));

    /// <summary>
    /// Sends a POST request to the specified URI without content synchronously.
    /// </summary>
    public void PostData(Uri uri) => PostDataAsync(uri).GetAwaiter().GetResult();

    /// <summary>
    /// Deletes data using DELETE with parameters synchronously.
    /// </summary>
    public void DeleteData(string str, params ApiParameter[] parameters) => DeleteData(ToWebUri(str, parameters));

    /// <summary>
    /// Deletes data using DELETE at the specified URI synchronously.
    /// </summary>
    public void DeleteData(Uri uri) => DeleteDataAsync(uri).GetAwaiter().GetResult();

    /// <summary>
    /// Deletes data using DELETE with content synchronously.
    /// </summary>
    public void DeleteDataWithParam<TParam>(string str, TParam value) => DeleteDataAsync<TParam, bool>(str.ToWebUri(), value).GetAwaiter().GetResult();

    /// <summary>
    /// Deletes data using DELETE at the specified URI with content synchronously.
    /// </summary>
    public void DeleteDataWithParam<TParam>(Uri uri, TParam value) => DeleteDataAsync<TParam, bool>(uri, value).GetAwaiter().GetResult();

    /// <summary>
    /// Releases resources used by the service.
    /// </summary>
    public void Dispose() => _client.Dispose();

    /// <summary>
    /// Sends a generic HTTP request (no typed return).
    /// </summary>
    /// <param name="method">HTTP method to use.</param>
    /// <param name="uri">Target URI.</param>
    /// <param name="content">Request content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SendRequestAsync(HttpMethod method, Uri uri, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        using var tokenSource = new CancellationTokenSource();
        using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, cancellationToken);
        tokenSource.CancelAfter(_timeout);

        using var request = new HttpRequestMessage(method, uri);
        request.Content = content;
        request.Headers.AcceptLanguage.Add(new(CultureInfo.CurrentCulture.Name));
        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCancellationToken.Token).ConfigureAwait(false);

        if (response.IsSuccessStatusCode) return;

        throw await GetExceptionAsync(response).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a generic HTTP request and gets a typed response.
    /// </summary>
    /// <typeparam name="T">Expected response type.</typeparam>
    /// <param name="method">HTTP method to use.</param>
    /// <param name="uri">Target URI.</param>
    /// <param name="content">Request content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T> SendRequestAsync<T>(HttpMethod method, Uri uri, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        using var tokenSource = new CancellationTokenSource();
        using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, cancellationToken);
        tokenSource.CancelAfter(_timeout);

        using var request = new HttpRequestMessage(method, uri);
        request.Content = content;

        request.Headers.AcceptLanguage.Add(new(CultureInfo.CurrentCulture.Name));
        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCancellationToken.Token).ConfigureAwait(false);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsAsync<T>(linkedCancellationToken.Token).ConfigureAwait(false)
            : throw await GetExceptionAsync(response).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates HTTP content from an object.
    /// </summary>
    private static ObjectContent<TParam> CreateContent<TParam>(TParam value) => new(value, new JsonMediaTypeFormatter());

    /// <summary>
    /// Converts a string URL and an array of <see cref="ApiParameter"/> to a <see cref="Uri"/> with query parameters.
    /// </summary>
    /// <param name="str">The base URL as a string.</param>
    /// <param name="parameters">An array of <see cref="ApiParameter"/> representing query parameters to append.</param>
    /// <returns>A <see cref="Uri"/> with the specified query parameters.</returns>
    private static Uri ToWebUri(string str, ApiParameter[] parameters) => str.ToWebUri([.. parameters.Select(x => (x.Key, x.Value))]);

    /// <summary>
    /// Converts the HTTP response to a custom exception.
    /// </summary>
    private async Task<Exception> GetExceptionAsync(HttpResponseMessage response)
    {
        var result = await response.Content.ReadAsAsync<object>([new JsonProblemMediaTypeFormatter()]).ConfigureAwait(false);
        return _toException?.Invoke(result) ?? new WebApiException(result);
    }
}

/// <summary>
/// Formatter for the "application/problem+json" media type.
/// </summary>
internal sealed class JsonProblemMediaTypeFormatter : JsonMediaTypeFormatter
{
    public JsonProblemMediaTypeFormatter()
    {
        SupportedMediaTypes.Add(new("application/problem+json"));
        SupportedMediaTypes.Add(new("text/html"));
    }
}

/// <summary>
/// Handler for managing HTTP request timeouts.
/// </summary>
internal sealed class TimeoutHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException();
        }
    }
}

public record struct ApiParameter(string Key, string Value)
{
    public static implicit operator (string Key, string Value)(ApiParameter value) => ToValueTuple(value);

    public static implicit operator ApiParameter((string Key, string Value) value) => ToApiParameter(value);

    public static ApiParameter ToApiParameter((string Key, string Value) value) => new(value.Key, value.Value);

    public static (string Key, string Value) ToValueTuple(ApiParameter value) => (value.Key, value.Value);
}
