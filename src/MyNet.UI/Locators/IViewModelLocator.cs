// -----------------------------------------------------------------------
// <copyright file="IViewModelLocator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators;

/// <summary>
/// Defines the contract for a view model locator that can resolve view model instances based on their type. This is typically used in MVVM architectures to locate and instantiate view models for views.
/// </summary>
public interface IViewModelLocator
{
    /// <summary>
    /// Resolves a view model instance based on the specified view model type. The locator should determine how to create or retrieve an instance of the view model associated with the given type, and return that instance. This allows for decoupling the view model creation logic from the rest of the application, enabling flexibility in how view models are instantiated and associated with their corresponding views.
    /// </summary>
    /// <param name="viewModelType">The type of the view model to resolve.</param>
    /// <returns>The resolved view model instance.</returns>
    object Get(Type viewModelType);

    /// <summary>
    /// Resolves a view model instance of the specified generic type. This allows for type-safe retrieval of view models without needing to specify the type explicitly at runtime.
    /// </summary>
    /// <typeparam name="T">The type of the view model to resolve.</typeparam>
    /// <returns>The resolved view model instance.</returns>
    T Get<T>()
        where T : class;
}
