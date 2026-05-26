// -----------------------------------------------------------------------
// <copyright file="WebApiService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Http.Exceptions;
using MyNet.Text;

namespace MyNet.Http;

/// <summary>
/// Provides methods to interact with web APIs using HTTP requests. The <see cref="WebApiService"/> class allows sending GET, POST, PUT, PATCH, and DELETE requests to specified URIs, with support for custom headers, request timeouts, and response deserialization. It also includes error handling by parsing API error responses and converting them into exceptions. The service can be configured with a base server URL and can utilize a custom <see cref="HttpClient"/> if needed. Additionally, it implements the <see cref="IDisposable"/> interface to ensure proper disposal of resources when the service is no longer needed.
/// </summary>
public sealed class WebApiService : IWebApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _client;
    private readonly bool _disposeClient;
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
        : this(CreateHttpClient(serverUrl, headers), timeout, toException, disposeClient: true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiService"/> class with a custom <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="client">Configured HTTP client.</param>
    /// <param name="timeout">Maximum request timeout duration.</param>
    /// <param name="toException">Function to convert a response to an exception.</param>
    /// <param name="disposeClient">Indicates whether this service should dispose the HTTP client.</param>
    public WebApiService(HttpClient client, TimeSpan timeout = default, Func<object?, Exception>? toException = null, bool disposeClient = false)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = client;
        _disposeClient = disposeClient;
        _toException = toException;
        _timeout = timeout != TimeSpan.Zero ? timeout : TimeSpan.FromMilliseconds(Timeout.Infinite);

        // We enforce timeouts per request via CancellationTokenSource.CancelAfter.
        _client.Timeout = Timeout.InfiniteTimeSpan;
    }

    /// <inheritdoc/>
    public Task<Stream> GetStreamAsync(string str) => GetStreamAsync(new(str, UriKind.RelativeOrAbsolute), CancellationToken.None);

    /// <inheritdoc/>
    public Task<T> GetDataAsync<T>(string str, CancellationToken cancellationToken, params ApiParameter[] parameters) => GetDataAsync<T>(ToWebUri(str, parameters), cancellationToken);

    /// <inheritdoc/>
    public Task<T> GetDataAsync<T>(Uri uri, CancellationToken cancellationToken = default) => SendRequestAsync<T>(HttpMethod.Get, uri, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public Task<TReturn> PostDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters) => PostDataAsync<TParam, TReturn>(ToWebUri(str, parameters), value, cancellationToken);

    /// <inheritdoc/>
    public Task<T> PostDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default) => SendRequestAsync<T>(HttpMethod.Post, uri, CreateContent(value), cancellationToken);

    /// <inheritdoc/>
    public Task PostDataAsync<TParam>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters) => PostDataAsync(ToWebUri(str, parameters), value, cancellationToken);

    /// <inheritdoc/>
    public Task PostDataAsync<TParam>(Uri uri, TParam value, CancellationToken cancellationToken = default) => SendRequestAsync(HttpMethod.Post, uri, CreateContent(value), cancellationToken);

    /// <inheritdoc/>
    public Task PostDataAsync(string str, CancellationToken cancellationToken, params ApiParameter[] parameters) => PostDataAsync(ToWebUri(str, parameters), cancellationToken);

    /// <inheritdoc/>
    public Task PostDataAsync(Uri uri, CancellationToken cancellationToken = default) => SendRequestAsync(HttpMethod.Post, uri, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public Task<TReturn> PutDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters) => PutDataAsync<TParam, TReturn>(ToWebUri(str, parameters), value, cancellationToken);

    /// <inheritdoc/>
    public Task<T> PutDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default) => SendRequestAsync<T>(HttpMethod.Put, uri, CreateContent(value), cancellationToken);

    /// <inheritdoc/>
    public Task<TReturn> PatchDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters) => PatchDataAsync<TParam, TReturn>(ToWebUri(str, parameters), value, cancellationToken);

    /// <inheritdoc/>
    public Task<T> PatchDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default) => SendRequestAsync<T>(new("PATCH"), uri, CreateContent(value), cancellationToken);

    /// <inheritdoc/>
    public Task DeleteDataAsync(string str, CancellationToken cancellationToken, params ApiParameter[] parameters) => DeleteDataAsync(ToWebUri(str, parameters), cancellationToken);

    /// <inheritdoc/>
    public Task DeleteDataAsync(Uri uri, CancellationToken cancellationToken = default) => SendRequestAsync(HttpMethod.Delete, uri, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public Task<TReturn> DeleteDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters) => DeleteDataAsync<TParam, TReturn>(ToWebUri(str, parameters), value, cancellationToken);

    /// <inheritdoc/>
    public Task<T> DeleteDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default) => SendRequestAsync<T>(HttpMethod.Delete, uri, CreateContent(value), cancellationToken);

    /// <inheritdoc/>
    public Task DeleteDataAsync<TParam>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters) => DeleteDataAsync(ToWebUri(str, parameters), value, cancellationToken);

    /// <inheritdoc/>
    public Task DeleteDataAsync<TParam>(Uri uri, TParam value, CancellationToken cancellationToken = default) => SendRequestAsync(HttpMethod.Delete, uri, CreateContent(value), cancellationToken);

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposeClient)
        {
            _client.Dispose();
        }
    }

    /// <inheritdoc/>
    public async Task SendRequestAsync(HttpMethod method, Uri uri, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(method, uri, content);
        using var linkedCancellationToken = CreateLinkedTimeoutTokenSource(request, cancellationToken, out var tokenSource);

        HttpResponseMessage? response = null;
        try
        {
            response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCancellationToken.Token).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            throw await GetExceptionAsync(response, linkedCancellationToken.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException();
        }
        finally
        {
            response?.Dispose();
            tokenSource.Dispose();
        }
    }

    /// <inheritdoc/>
    public async Task<T> SendRequestAsync<T>(HttpMethod method, Uri uri, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(method, uri, content);
        using var linkedCancellationToken = CreateLinkedTimeoutTokenSource(request, cancellationToken, out var tokenSource);

        HttpResponseMessage? response = null;
        try
        {
            response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCancellationToken.Token).ConfigureAwait(false);
            return !response.IsSuccessStatusCode
                ? throw await GetExceptionAsync(response, linkedCancellationToken.Token).ConfigureAwait(false)
                : await ReadResponseAsync<T>(response, linkedCancellationToken.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException();
        }
        finally
        {
            response?.Dispose();
            tokenSource.Dispose();
        }
    }

    private static HttpClient CreateHttpClient(Uri? serverUrl, Dictionary<string, string>? headers)
    {
        var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip, CheckCertificateRevocationList = true };

        var client = new HttpClient(handler) { BaseAddress = serverUrl };

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new("application/json"));
        client.DefaultRequestHeaders.Accept.Add(new("application/problem+json"));
        client.DefaultRequestHeaders.Accept.Add(new("text/plain"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
        ApplyHeaders(client.DefaultRequestHeaders, headers);

        return client;
    }

    private async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, uri);
        using var linkedCancellationToken = CreateLinkedTimeoutTokenSource(request, cancellationToken, out var tokenSource);

        HttpResponseMessage? response = null;
        try
        {
            response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, linkedCancellationToken.Token).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync(linkedCancellationToken.Token).ConfigureAwait(false);
                var responseToReturn = response;
                response = null;

                return new ResponseStream(stream, responseToReturn);
            }

            throw await GetExceptionAsync(response, linkedCancellationToken.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException();
        }
        finally
        {
            response?.Dispose();
            tokenSource.Dispose();
        }
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static members should appear before non-static members", Justification = "Grouped helper methods intentionally placed after public instance members.")]
    private static void ApplyHeaders(HttpRequestHeaders headers, Dictionary<string, string>? customHeaders)
    {
        if (customHeaders is null)
        {
            return;
        }

        foreach (var (key, value) in customHeaders)
        {
            if (string.Equals(key, "Authorization", StringComparison.OrdinalIgnoreCase))
            {
                headers.Authorization = AuthenticationHeaderValue.Parse(value);
                continue;
            }

            if (!headers.TryAddWithoutValidation(key, value))
            {
                throw new InvalidOperationException($"Unable to add HTTP header '{key}'.");
            }
        }
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, Uri uri, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, uri) { Content = content };

        request.Headers.AcceptLanguage.Add(new(CultureInfo.CurrentCulture.Name));
        return request;
    }

    private CancellationTokenSource CreateLinkedTimeoutTokenSource(HttpRequestMessage request, CancellationToken cancellationToken, out CancellationTokenSource tokenSource)
    {
        tokenSource = new();
        var linked = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, cancellationToken);
        var effectiveTimeout = request.GetTimeout() ?? _timeout;

        if (effectiveTimeout != Timeout.InfiniteTimeSpan)
        {
            tokenSource.CancelAfter(effectiveTimeout);
        }

        return linked;
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static members should appear before non-static members", Justification = "Grouped helper methods intentionally placed after non-static members.")]
    private static StringContent CreateContent<TParam>(TParam value) => new(JsonSerializer.Serialize(value, JsonOptions), Encoding.UTF8, "application/json");

    private static Uri ToWebUri(string str, ApiParameter[] parameters)
        => parameters.Length == 0 && Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var uri)
            ? uri
            : str.ToWebUri([.. parameters.Select(x => (x.Key, x.Value))]);

    [SuppressMessage("Reliability", "CA2007:Appeler ConfigureAwait sur la tâche attendue", Justification = "Library code should not force ConfigureAwait(false) on consumers.")]
    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)await response.Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        await using var stream = await response.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);

        if (stream.Length == 0)
        {
            throw new JsonException($"Response body is empty and cannot be deserialized to '{typeof(T)}'.");
        }

        var result = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken).ConfigureAwait(false);
        return result ?? throw new JsonException($"Unable to deserialize response body to '{typeof(T)}'.");
    }

    private Task<Exception> GetExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken) => GetExceptionAsync(response.StatusCode, response.ReasonPhrase, response.Content, cancellationToken);

    private async Task<Exception> GetExceptionAsync(HttpStatusCode statusCode, string? reasonPhrase, HttpContent content, CancellationToken cancellationToken)
    {
        string? text = null;
        try
        {
            text = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // ignore read failures: we still want to throw a meaningful exception.
        }

        var problemException = ProblemDetailsParser.TryParseException(text);
        if (problemException is not null)
        {
            return problemException;
        }

        object? payload = null;
        if (!string.IsNullOrWhiteSpace(text))
        {
            try
            {
                payload = JsonNode.Parse(text);
            }
            catch (JsonException)
            {
                payload = text;
            }
        }

        return _toException?.Invoke(payload) ?? new WebApiException(statusCode, reasonPhrase, text, payload);
    }

    /// <summary>
    /// A stream wrapper that ensures the underlying HTTP response is disposed when the stream is disposed. This is necessary because the response stream is tied to the lifecycle of the HTTP response, and disposing the stream should also dispose the response to free up resources. The <see cref="ResponseStream"/> class inherits from <see cref="Stream"/> and overrides all necessary members to delegate to the inner stream while ensuring proper disposal of both the stream and the response.
    /// </summary>
    /// <param name="inner">The inner stream to wrap.</param>
    /// <param name="response">The HTTP response message associated with the stream.</param>
    private sealed class ResponseStream(Stream inner, HttpResponseMessage response) : Stream
    {
        /// <inheritdoc/>
        public override bool CanRead => inner.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => inner.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => inner.CanWrite;

        /// <inheritdoc/>
        public override long Length => inner.Length;

        /// <inheritdoc/>
        public override long Position
        {
            get => inner.Position;
            set => inner.Position = value;
        }

        /// <inheritdoc/>
        public override void Flush() => inner.Flush();

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => inner.ReadAsync(buffer, offset, count, cancellationToken);

        /// <inheritdoc/>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => inner.ReadAsync(buffer, cancellationToken);

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);

        /// <inheritdoc/>
        public override void SetLength(long value) => inner.SetLength(value);

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => inner.Write(buffer, offset, count);

        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => inner.WriteAsync(buffer, offset, count, cancellationToken);

        /// <inheritdoc/>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => inner.WriteAsync(buffer, cancellationToken);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
                response.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        public override async ValueTask DisposeAsync()
        {
            try
            {
                await inner.DisposeAsync().ConfigureAwait(false);
            }
            finally
            {
                response.Dispose();
            }

            await base.DisposeAsync().ConfigureAwait(false);
        }
    }
}

/// <summary>
/// Represents a key-value pair for API parameters, such as query parameters or form data. This struct provides implicit conversions to and from a tuple of (string Key, string Value) for convenient usage when constructing API requests.
/// </summary>
/// <param name="Key">The key of the API parameter.</param>
/// <param name="Value">The value of the API parameter.</param>
public record struct ApiParameter(string Key, string Value)
{
    /// <summary>
    /// Defines an implicit conversion from an <see cref="ApiParameter"/> to a tuple of (string Key, string Value). This allows an instance of <see cref="ApiParameter"/> to be easily converted to a tuple when needed, such as when building query parameters for a URI. The conversion is performed by the static method <see cref="ToValueTuple(ApiParameter)"/>, which extracts the Key and Value properties from the <see cref="ApiParameter"/> instance and returns them as a tuple.
    /// </summary>
    /// <param name="value">The <see cref="ApiParameter"/> instance to convert.</param>
    public static implicit operator (string Key, string Value)(ApiParameter value) => ToValueTuple(value);

    /// <summary>
    /// Defines an implicit conversion from a tuple of (string Key, string Value) to an <see cref="ApiParameter"/>. This allows a tuple representing an API parameter to be easily converted to an instance of <see cref="ApiParameter"/> when needed, such as when constructing API requests. The conversion is performed by the static method <see cref="ToApiParameter"/>, which takes the Key and Value from the input tuple and creates a new <see cref="ApiParameter"/> instance with those values.
    /// </summary>
    /// <param name="value">The tuple representing the API parameter.</param>
    /// <returns>A new <see cref="ApiParameter"/> instance with the specified Key and Value.</returns>
    public static implicit operator ApiParameter((string Key, string Value) value) => ToApiParameter(value);

    /// <summary>
    /// Converts a tuple of (string Key, string Value) to an <see cref="ApiParameter"/>. This method is used by the implicit conversion operator to create an instance of <see cref="ApiParameter"/> from a tuple. It takes the Key and Value from the input tuple and returns a new <see cref="ApiParameter"/> instance initialized with those values.
    /// </summary>
    /// <param name="value">The tuple representing the API parameter.</param>
    /// <returns>A new <see cref="ApiParameter"/> instance with the specified Key and Value.</returns>
    public static ApiParameter ToApiParameter((string Key, string Value) value) => new(value.Key, value.Value);

    /// <summary>
    /// Converts an <see cref="ApiParameter"/> to a tuple of (string Key, string Value). This method is used by the implicit conversion operator to create a tuple from an <see cref="ApiParameter"/> instance. It takes the Key and Value from the <see cref="ApiParameter"/> instance and returns them as a tuple.
    /// </summary>
    /// <param name="value">The <see cref="ApiParameter"/> instance to convert.</param>
    /// <returns>A tuple containing the Key and Value of the <see cref="ApiParameter"/> instance.</returns>
    public static (string Key, string Value) ToValueTuple(ApiParameter value) => (value.Key, value.Value);
}
