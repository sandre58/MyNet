// -----------------------------------------------------------------------
// <copyright file="IViewFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Factories;

/// <summary>
/// Defines the contract for a view factory that can create view instances based on view model types. This interface allows for the creation of views in a decoupled manner, enabling flexibility in how views are instantiated and associated with their corresponding view models. The factory can use various strategies to determine which view to create for a given view model type, such as naming conventions, attributes, or configuration mappings.
/// </summary>
public interface IViewFactory
{
    /// <summary>
    /// Creates a view instance based on the specified view model type. The factory should determine the appropriate view type to create based on the provided view model type, and return an instance of that view. This allows for decoupling the view creation logic from the rest of the application, enabling flexibility in how views are instantiated and associated with their corresponding view models.
    /// </summary>
    /// <param name="viewModelType">The type of the view model for which a view should be created.</param>
    /// <returns>An instance of the view associated with the specified view model type, or null if no suitable view is found.</returns>
    object? CreateView(Type viewModelType);

    /// <summary>
    /// Creates a view instance of the specified generic types. This allows for type-safe creation of views without needing to specify the types explicitly at runtime. The factory should determine the appropriate view type to create based on the provided generic types, and return an instance of that view. If no suitable view is found, null is returned.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model for which a view should be created.</typeparam>
    /// <typeparam name="TView">The type of the view to create.</typeparam>
    /// <returns>An instance of the view associated with the specified view model type, or null if no suitable view is found.</returns>
    TView? CreateView<TViewModel, TView>()
        where TViewModel : class
        where TView : class;
}
