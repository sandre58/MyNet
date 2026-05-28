// -----------------------------------------------------------------------
// <copyright file="ViewFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Locators.Conventions;

namespace MyNet.UI.Locators.Factories;

/// <summary>
/// Resolves a view type from a view model type, then instantiates it via <see cref="IViewLocator"/>.
/// </summary>
/// <param name="resolver">The type resolver used to determine the view type for a given view model type.</param>
/// <param name="viewLocator">The view locator used to retrieve an instance of the view.</param>
public sealed class ViewFactory(ITypeResolver resolver, IViewLocator viewLocator) : IViewFactory
{
    /// <inheritdoc />
    public object CreateView(Type viewModelType)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);

        var viewType = resolver.Resolve(viewModelType)
                       ?? throw ViewResolutionException.NoMapping(viewModelType);

        return viewLocator.Get(viewType);
    }

    /// <inheritdoc />
    public TView CreateView<TViewModel, TView>()
        where TViewModel : class
        where TView : class
    {
        var view = CreateView(typeof(TViewModel));
        return view as TView ?? throw ViewResolutionException.ResolvedViewTypeMismatch(typeof(TViewModel), view.GetType(), typeof(TView));
    }
}
