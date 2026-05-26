// -----------------------------------------------------------------------
// <copyright file="IWebApiService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.Http;

/// <summary>
/// Abstraction for sending HTTP requests to a web API.
/// </summary>
public interface IWebApiService : IDisposable
{
    /// <summary>
    /// Sends a GET request to the specified URI and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="str">The URI as a string.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response stream.</returns>
    Task<Stream> GetStreamAsync(string str);

    /// <summary>
    /// Sends a GET request to the specified URI and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="str">The URI as a string.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="parameters">Optional parameters for the API request.</param>
    /// <typeparam name="T">The type of the data to return.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the typed data.</returns>
    Task<T> GetDataAsync<T>(string str, CancellationToken cancellationToken, params ApiParameter[] parameters);

    /// <summary>
    /// Sends a GET request to the specified URI and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="T">The type of the data to return.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the typed data.</returns>
    Task<T> GetDataAsync<T>(Uri uri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request to the specified URI with the provided data and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="str">The URI as a string.</param>
    /// <param name="value">The data to send in the request body.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="parameters">Optional parameters for the API request.</param>
    /// <typeparam name="TParam">The type of the data to send in the request body.</typeparam>
    /// <typeparam name="TReturn">The type of the data to return.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the typed data.</returns>
    Task<TReturn> PostDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters);

    /// <summary>
    /// Sends a POST request to the specified URI with the provided data and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="value">The data to send in the request body.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TParam">The type of the data to send in the request body.</typeparam>
    /// <typeparam name="T">The type of the data to return.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the typed data.</returns>
    Task<T> PostDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request to the specified URI with the provided data and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="str">The URI as a string.</param>
    /// <param name="value">The data to send in the request body.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="parameters">Optional parameters for the API request.</param>
    /// <typeparam name="TParam">The type of the data to send in the request body.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PostDataAsync<TParam>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters);

    /// <summary>
    /// Sends a POST request to the specified URI with the provided data and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="value">The data to send in the request body.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TParam">The type of the data to send in the request body.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PostDataAsync<TParam>(Uri uri, TParam value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request to the specified URI and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="str">The URI as a string.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="parameters">Optional parameters for the API request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PostDataAsync(string str, CancellationToken cancellationToken, params ApiParameter[] parameters);

    /// <summary>
    /// Sends a POST request to the specified URI and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PostDataAsync(Uri uri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a PUT request to the specified URI with the provided data and returns the response content as a stream. The caller is responsible for disposing the returned stream, which will also dispose the underlying HTTP response. This method is useful for efficiently handling large responses or streaming data from a web API without loading the entire content into memory at once.
    /// </summary>
    /// <param name="str">The URI as a string.</param>
    /// <param name="value">The data to send in the request body.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="parameters">Optional parameters for the API request.</param>
    /// <typeparam name="TParam">The type of the data to send in the request body.</typeparam>
    /// <typeparam name="TReturn">The type of the data returned in the response.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<TReturn> PutDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters);

    Task<T> PutDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default);

    Task<TReturn> PatchDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters);

    Task<T> PatchDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default);

    Task DeleteDataAsync(string str, CancellationToken cancellationToken, params ApiParameter[] parameters);

    Task DeleteDataAsync(Uri uri, CancellationToken cancellationToken = default);

    Task<TReturn> DeleteDataAsync<TParam, TReturn>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters);

    Task<T> DeleteDataAsync<TParam, T>(Uri uri, TParam value, CancellationToken cancellationToken = default);

    Task DeleteDataAsync<TParam>(string str, TParam value, CancellationToken cancellationToken, params ApiParameter[] parameters);

    Task DeleteDataAsync<TParam>(Uri uri, TParam value, CancellationToken cancellationToken = default);

    Task SendRequestAsync(HttpMethod method, Uri uri, HttpContent? content = null, CancellationToken cancellationToken = default);

    Task<T> SendRequestAsync<T>(HttpMethod method, Uri uri, HttpContent? content = null, CancellationToken cancellationToken = default);
}
