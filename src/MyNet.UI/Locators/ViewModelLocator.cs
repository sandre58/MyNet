// -----------------------------------------------------------------------
// <copyright file="ViewModelLocator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators;

/// <summary>
/// Implements a view model locator that uses an IServiceProvider to resolve view model instances. This allows for dependency injection and decoupling of view models from their creation logic.
/// </summary>
/// <param name="provider">The service provider used to resolve view model instances.</param>
public sealed class ViewModelLocator(IServiceProvider provider) : IViewModelLocator
{
    /// <summary>
    /// Resolves a view model instance based on the specified view model type. The locator uses the provided IServiceProvider to retrieve an instance of the view model associated with the given type. If the service provider cannot create an instance of the specified type, an InvalidOperationException is thrown.
    /// </summary>
    /// <param name="viewModelType">The type of the view model to retrieve.</param>
    /// <returns>The view model instance.</returns>
    public object Get(Type viewModelType)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);

        return provider.GetService(viewModelType) ?? throw new InvalidOperationException($"Cannot create instance of {viewModelType}");
    }

    /// <summary>
    /// Resolves a view model instance of the specified generic type. This allows for type-safe retrieval of view models without needing to specify the type explicitly at runtime. The locator uses the provided IServiceProvider to retrieve an instance of the view model associated with the given generic type. If the service provider cannot create an instance of the specified type, an InvalidOperationException is thrown.
    /// </summary>
    /// <typeparam name="T">The type of the view model to retrieve.</typeparam>
    /// <returns>The view model instance.</returns>
    public T Get<T>()
        where T : class => (T)Get(typeof(T));
}
