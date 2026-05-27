// -----------------------------------------------------------------------
// <copyright file="ViewLocator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators;

/// <summary>
/// Implements the IViewLocator interface to provide a mechanism for locating and instantiating views based on their types. This implementation uses an IServiceProvider to resolve view instances, allowing for dependency injection and flexible view creation. If the service provider cannot resolve the view type, it falls back to using Activator.CreateInstance to create an instance of the view. This allows for both registered services and direct instantiation of views, making it versatile for various application architectures.
/// </summary>
/// <param name="provider">The service provider used to resolve view instances.</param>
public sealed class ViewLocator(IServiceProvider provider) : IViewLocator
{
    /// <summary>
    /// Retrieves a view instance based on the specified view type. The locator first attempts to resolve the view type using the provided IServiceProvider. If the service provider cannot resolve the view type, it falls back to using Activator.CreateInstance to create an instance of the view. This allows for both registered services and direct instantiation of views, making it versatile for various application architectures.
    /// </summary>
    /// <param name="viewType">The type of the view to retrieve.</param>
    /// <returns>The view instance.</returns>
    public object Get(Type viewType)
    {
        ArgumentNullException.ThrowIfNull(viewType);

        if (provider.GetService(viewType) is { } instance)
            return instance;

        try
        {
            return Activator.CreateInstance(viewType)
                   ?? throw ViewResolutionException.CannotInstantiateView(viewType);
        }
        catch (ViewResolutionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw ViewResolutionException.CannotInstantiateView(viewType, ex);
        }
    }

    /// <summary>
    /// Retrieves a view instance of the specified generic type. This method provides a type-safe way to retrieve views without needing to specify the type explicitly at runtime. It internally calls the non-generic Get method, passing the type of the generic parameter T, and casts the result to T. If the view cannot be resolved or created, it will throw an exception as defined in the non-generic Get method.
    /// </summary>
    /// <typeparam name="T">The type of the view to retrieve.</typeparam>
    /// <returns>The view instance of type T.</returns>
    public T Get<T>()
        where T : class => (T)Get(typeof(T));
}
