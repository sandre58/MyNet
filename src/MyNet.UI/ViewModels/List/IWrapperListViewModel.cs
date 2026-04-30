// -----------------------------------------------------------------------
// <copyright file="IWrapperListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Defines a list view model that provides wrapped items of type TWrapper corresponding to the source items of type T.
/// </summary>
/// <typeparam name="T">The type of the source items.</typeparam>
/// <typeparam name="TWrapper">The type of the wrapped items.</typeparam>
public interface IWrapperListViewModel<T, TWrapper> : IListViewModel<T>
    where T : notnull
    where TWrapper : IWrapper<T>
{
    /// <summary>
    /// Gets the wrapped items corresponding to the current Items.
    /// </summary>
    ReadOnlyObservableCollection<TWrapper> Wrappers { get; }

    /// <summary>
    /// Gets the wrapped groups if grouping is active.
    /// </summary>
    ReadOnlyObservableCollection<IGroup<TWrapper>>? WrapperGroups { get; }

    /// <summary>
    /// Gets the wrapper associated with a given item.
    /// </summary>
    TWrapper? GetWrapper(T item);

    /// <summary>
    /// Ensures a wrapper exists for the given item.
    /// Creates it if necessary.
    /// </summary>
    TWrapper GetOrCreateWrapper(T item);
}
