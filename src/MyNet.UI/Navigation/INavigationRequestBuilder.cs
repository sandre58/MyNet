// -----------------------------------------------------------------------
// <copyright file="INavigationRequestBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Represents a fluent builder used to configure and execute a navigation request.
/// </summary>
public interface INavigationRequestBuilder
{
    /// <summary>
    /// Replaces the current parameter payload.
    /// </summary>
    /// <param name="parameters">The new parameter payload.</param>
    /// <returns>The same builder instance.</returns>
    INavigationRequestBuilder With(INavigationParameters? parameters);

    /// <summary>
    /// Replaces the current parameter payload using an anonymous object, dictionary, or parameter bag.
    /// </summary>
    /// <param name="parameters">The new parameter payload.</param>
    /// <returns>The same builder instance.</returns>
    INavigationRequestBuilder With(object? parameters);

    /// <summary>
    /// Adds or replaces a single named parameter.
    /// </summary>
    /// <param name="key">Parameter key.</param>
    /// <param name="value">Parameter value.</param>
    /// <returns>The same builder instance.</returns>
    INavigationRequestBuilder WithParameter(string key, object? value);

    /// <summary>
    /// Executes the navigation request.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The navigation result.</returns>
    Task<NavigationResult> GoAsync(CancellationToken cancellationToken = default);
}
