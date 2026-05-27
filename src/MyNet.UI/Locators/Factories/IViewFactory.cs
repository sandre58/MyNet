// -----------------------------------------------------------------------
// <copyright file="IViewFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Factories;

/// <summary>
/// Creates view instances from view model types using <see cref="Conventions.ITypeResolver"/> and <see cref="IViewLocator"/>.
/// </summary>
public interface IViewFactory
{
    /// <summary>
    /// Resolves and instantiates the view for the given view model type.
    /// </summary>
    /// <param name="viewModelType">The view model type.</param>
    /// <returns>The view instance.</returns>
    /// <exception cref="ViewResolutionException">No mapping exists or the view cannot be instantiated.</exception>
    object CreateView(Type viewModelType);

    /// <summary>
    /// Resolves and instantiates the view for <typeparamref name="TViewModel"/>, verifying it is assignable to <typeparamref name="TView"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The expected view type.</typeparam>
    /// <returns>The view instance.</returns>
    /// <exception cref="ViewResolutionException">No mapping exists, the view cannot be instantiated, or the resolved type does not match.</exception>
    TView CreateView<TViewModel, TView>()
        where TViewModel : class
        where TView : class;
}
