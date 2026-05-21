// -----------------------------------------------------------------------
// <copyright file="IViewLocator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators;

/// <summary>
/// Defines the contract for a view locator that can retrieve view instances based on their type. This is typically used in MVVM architectures to locate and instantiate views corresponding to view models or other types. The view locator abstracts the logic of creating and managing view instances, allowing for flexibility in how views are resolved and instantiated.
/// </summary>
public interface IViewLocator
{
    /// <summary>
    /// Retrieves a view instance based on the specified view type. The locator should determine how to create or retrieve an instance of the view associated with the given type, and return that instance. This allows for decoupling the view creation logic from the rest of the application, enabling flexibility in how views are instantiated and associated with their corresponding view models or other types.
    /// </summary>
    /// <param name="viewType">The type of the view to retrieve.</param>
    /// <returns>The view instance.</returns>
    object Get(Type viewType);

    /// <summary>
    /// Retrieves a view instance of the specified generic type. This allows for type-safe retrieval of views without needing to specify the type explicitly at runtime.
    /// </summary>
    /// <typeparam name="T">The type of the view to retrieve.</typeparam>
    /// <returns>The view instance.</returns>
    T Get<T>()
        where T : class;
}
