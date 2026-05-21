// -----------------------------------------------------------------------
// <copyright file="ViewFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Locators.Conventions;

namespace MyNet.UI.Locators.Factories;

/// <summary>
/// Implements a view factory that creates views based on the associated view model type. It uses a type resolver to determine the view type corresponding to a given view model type, and then uses a view locator to retrieve an instance of the view. This allows for a flexible and decoupled way to create views based on their view models, following conventions or custom mappings defined in the type resolver and view locator.
/// </summary>
/// <param name="resolver">The type resolver used to determine the view type for a given view model type.</param>
/// <param name="viewLocator">The view locator used to retrieve an instance of the view.</param>
public sealed class ViewFactory(ITypeResolver resolver, IViewLocator viewLocator) : IViewFactory
{
    /// <summary>
    /// Creates a view instance based on the specified view model type. The factory uses the provided type resolver to determine the corresponding view type for the given view model type, and then uses the view locator to retrieve an instance of that view. If the resolver cannot determine a view type for the provided view model type, or if the view locator cannot retrieve an instance of the resolved view type, this method returns null.
    /// </summary>
    /// <param name="viewModelType">The type of the view model for which to create a view.</param>
    /// <returns>An instance of the view corresponding to the specified view model type, or null if no view could be created.</returns>
    public object? CreateView(Type viewModelType)
    {
        var viewType = resolver.Resolve(viewModelType);

        return viewType is null ? null : viewLocator.Get(viewType);
    }

    /// <summary>
    /// Creates a view instance based on the specified generic view model type. This method is a type-safe wrapper around the non-generic CreateView method, allowing for easier usage when the view model type is known at compile time. It uses the same logic as the non-generic version to resolve the corresponding view type and retrieve an instance of that view. If the resolver cannot determine a view type for the provided view model type, or if the view locator cannot retrieve an instance of the resolved view type, this method returns null.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model for which to create a view.</typeparam>
    /// <typeparam name="TView">The type of the view to create.</typeparam>
    /// <returns>An instance of the view corresponding to the specified view model type, or null if no view could be created.</returns>
    public TView? CreateView<TViewModel, TView>()
        where TViewModel : class
        where TView : class =>
        (TView?)CreateView(typeof(TViewModel));
}
