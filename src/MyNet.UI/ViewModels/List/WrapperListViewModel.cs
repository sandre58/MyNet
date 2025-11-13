// -----------------------------------------------------------------------
// <copyright file="WrapperListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using DynamicData;
using MyNet.Observable.Attributes;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Providers;
using MyNet.UI.Collections;
using MyNet.UI.Loading;
using MyNet.UI.Threading;
using MyNet.Utilities;
using MyNet.Utilities.Providers;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Base view model for managing lists with wrappers, providing additional wrapper-specific functionality
/// on top of standard list management (filtering, sorting, paging, etc.).
/// </summary>
/// <typeparam name="T">The type of items in the list. Must be a reference type.</typeparam>
/// <typeparam name="TWrapper">The wrapper type that wraps items of type <typeparamref name="T"/>.</typeparam>
/// <typeparam name="TCollection">The collection type managing the items and wrappers. Must derive from <see cref="ExtendedWrapperCollection{T, TWrapper}"/>.</typeparam>
/// <remarks>
/// This class extends <see cref="ListViewModelBase{T, TCollection}"/> to provide wrapper-specific collections
/// and paging support for wrapped items.
/// </remarks>
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
[CanBeValidatedForDeclaredClassOnly(false)]
public abstract class WrapperListViewModel<T, TWrapper, TCollection> : ListViewModelBase<T, TCollection>, IWrapperListViewModel<T, TWrapper>
    where TCollection : ExtendedWrapperCollection<T, TWrapper>
    where TWrapper : IWrapper<T>
    where T : notnull
{
    /// <summary>
    /// Observable collection containing the currently paged wrappers.
    /// </summary>
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup")]
    private readonly UiObservableCollection<TWrapper> _pagedWrappers = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T, TWrapper, TCollection}"/> class.
    /// </summary>
    /// <param name="collection">The underlying collection managing the data and wrappers.</param>
    /// <param name="parametersProvider">Optional provider for list parameters (filters, sorting, etc.). Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    protected WrapperListViewModel(
            TCollection collection,
            IListParametersProvider? parametersProvider = null,
            IBusyService? busyService = null)
    : base(collection, parametersProvider, busyService)
          => PagedWrappers = new(_pagedWrappers);

    /// <summary>
    /// Gets the filtered and sorted wrappers ready for display.
    /// This collection reflects the current filtering and sorting applied to the items.
    /// </summary>
    public ReadOnlyObservableCollection<TWrapper> Wrappers => Collection.Wrappers;

    /// <summary>
    /// Gets the source wrappers before any filtering or sorting is applied.
    /// This collection contains all wrappers in their original order.
    /// </summary>
    public ReadOnlyObservableCollection<TWrapper> WrappersSource => Collection.WrappersSource;

    /// <summary>
    /// Gets the paged subset of wrappers when paging is enabled.
    /// Contains only the wrappers for the current page.
    /// </summary>
    public ReadOnlyObservableCollection<TWrapper> PagedWrappers { get; }

    /// <summary>
    /// Subscribes to the pager observable and creates the paging pipeline for both items and wrappers.
    /// </summary>
    /// <param name="pager">The observable page request stream.</param>
    /// <returns>A disposable subscription to the paging pipeline.</returns>
    /// <remarks>
    /// This method overrides the base implementation to also page the wrapper collection
    /// in parallel with the item collection, ensuring consistency between items and wrappers.
    /// </remarks>
    protected override IDisposable SubscribePager(IObservable<PageRequest> pager)
    {
        // Clear existing paged wrappers before creating new subscription
        _pagedWrappers.Clear();

        // Create composite disposable for both item and wrapper paging subscriptions
        return new CompositeDisposable(
            base.SubscribePager(pager), // Page items
            Collection.ConnectWrappers() // Page wrappers in parallel
                      .Page(pager)
                      .Bind(_pagedWrappers)
                      .Subscribe());
    }

    /// <summary>
    /// Releases resources and performs cleanup operations.
    /// </summary>
    protected override void Cleanup()
    {
        _pagedWrappers.Dispose();
        base.Cleanup();
    }
}

/// <summary>
/// Concrete implementation of <see cref="WrapperListViewModel{T, TWrapper, TCollection}"/> class using the default <see cref="ExtendedWrapperCollection{T, TWrapper}"/> collection type.
/// Provides multiple constructors for different data source scenarios.
/// </summary>
/// <typeparam name="T">The type of items in the list. Must be a reference type.</typeparam>
/// <typeparam name="TWrapper">The wrapper type that wraps items of type <typeparamref name="T"/>.</typeparam>
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
[CanBeValidatedForDeclaredClassOnly(false)]
public class WrapperListViewModel<T, TWrapper> : WrapperListViewModel<T, TWrapper, ExtendedWrapperCollection<T, TWrapper>>
    where TWrapper : IWrapper<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T, TWrapper}"/> class
    /// from an existing collection.
    /// </summary>
    /// <param name="source">The source collection of items.</param>
    /// <param name="createWrapper">Function to create a wrapper for each item.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public WrapperListViewModel(
        ICollection<T> source,
        Func<T, TWrapper> createWrapper,
        IListParametersProvider? parametersProvider = null,
        IBusyService? busyService = null)
        : base(
            new ExtendedWrapperCollection<T, TWrapper>(source, Scheduler.UiOrCurrent, createWrapper),
            parametersProvider,
            busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T, TWrapper}"/> class
    /// from an items provider.
    /// </summary>
    /// <param name="source">The items provider.</param>
    /// <param name="createWrapper">Function to create a wrapper for each item.</param>
    /// <param name="loadItems">Whether to load items immediately. Default is true.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public WrapperListViewModel(
        IItemsProvider<T> source,
        Func<T, TWrapper> createWrapper,
        bool loadItems = true,
        IListParametersProvider? parametersProvider = null,
        IBusyService? busyService = null)
        : base(new ExtendedWrapperCollection<T, TWrapper>(source, loadItems, Scheduler.UiOrCurrent, createWrapper),
               parametersProvider,
               busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T, TWrapper}"/> class
    /// from a source provider.
    /// </summary>
    /// <param name="source">The source provider.</param>
    /// <param name="createWrapper">Function to create a wrapper for each item.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public WrapperListViewModel(
        ISourceProvider<T> source,
        Func<T, TWrapper> createWrapper,
        IListParametersProvider? parametersProvider = null,
        IBusyService? busyService = null)
   : base(new ExtendedWrapperCollection<T, TWrapper>(source, Scheduler.UiOrCurrent, createWrapper),
          parametersProvider,
          busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T, TWrapper}"/> class
    /// from an observable change set.
    /// </summary>
    /// <param name="source">The observable change set of items.</param>
    /// <param name="createWrapper">Function to create a wrapper for each item.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public WrapperListViewModel(
        IObservable<IChangeSet<T>> source,
        Func<T, TWrapper> createWrapper,
        IListParametersProvider? parametersProvider = null,
        IBusyService? busyService = null)
        : base(new ExtendedWrapperCollection<T, TWrapper>(source, Scheduler.UiOrCurrent, createWrapper),
               parametersProvider,
               busyService)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperListViewModel{T, TWrapper}"/> class
    /// with an empty collection.
    /// </summary>
    /// <param name="createWrapper">Function to create a wrapper for each item.</param>
    /// <param name="parametersProvider">Optional provider for list parameters. Uses default if null.</param>
    /// <param name="busyService">Optional busy service. If null, a new instance is created.</param>
    public WrapperListViewModel(
        Func<T, TWrapper> createWrapper,
        IListParametersProvider? parametersProvider = null,
        IBusyService? busyService = null)
    : base(new ExtendedWrapperCollection<T, TWrapper>(Scheduler.UiOrCurrent, createWrapper),
           parametersProvider,
           busyService)
    { }
}
