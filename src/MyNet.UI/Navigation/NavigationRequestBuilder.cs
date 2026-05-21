// -----------------------------------------------------------------------
// <copyright file="NavigationRequestBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Default fluent builder for navigation requests.
/// </summary>
public sealed class NavigationRequestBuilder(Func<INavigationPage> pageFactory, INavigationService navigationService) : INavigationRequestBuilder
{
    private object? _parameters;

    /// <inheritdoc />
    public INavigationRequestBuilder With(INavigationParameters? parameters)
    {
        _parameters = parameters;
        return this;
    }

    /// <inheritdoc />
    public INavigationRequestBuilder With(object? parameters)
    {
        _parameters = parameters;
        return this;
    }

    /// <inheritdoc />
    public INavigationRequestBuilder WithParameter(string key, object? value)
    {
        var parameters = _parameters as NavigationParameters;

        if (_parameters is null)
        {
            parameters = new();
            _parameters = parameters;
        }
        else if (parameters is null)
        {
            parameters = NavigationParameters.From(_parameters);

            _parameters = parameters;
        }

        parameters.Set(key, value);
        return this;
    }

    /// <inheritdoc />
    public Task<NavigationResult> GoAsync(CancellationToken cancellationToken = default)
        => navigationService.NavigateToAsync(pageFactory(), NormalizeParameters(), cancellationToken);

    /// <summary>
    /// Normalizes the current parameter payload into an INavigationParameters instance, if necessary.
    /// </summary>
    /// <returns>The normalized parameter payload.</returns>
    private INavigationParameters? NormalizeParameters()
        => _parameters switch
        {
            null => null,
            INavigationParameters navigationParameters => navigationParameters,
            _ => NavigationParameters.From(_parameters),
        };
}
