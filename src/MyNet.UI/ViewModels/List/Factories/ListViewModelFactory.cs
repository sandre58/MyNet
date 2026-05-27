// -----------------------------------------------------------------------
// <copyright file="ListViewModelFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Selection;
using MyNet.Observable.Collections.Sources;
using MyNet.UI.ViewModels.List.Selection;
using MyNet.UI.ViewModels.List.Services;

namespace MyNet.UI.ViewModels.List.Factories;

/// <summary>
/// Provides simple factory methods to create list view models with optional shared configuration.
/// </summary>
public static class ListViewModelFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="ListViewModel{T}"/> with the specified data provider and optional configuration.
    /// </summary>
    /// <param name="provider">The data provider for the list view model.</param>
    /// <param name="options">Optional configuration for the list view model.</param>
    /// <typeparam name="T">The type of items in the list view model.</typeparam>
    /// <returns>A new instance of <see cref="ListViewModel{T}"/>.</returns>
    public static ListViewModel<T> Create<T>(IListDataProvider<T> provider, ListViewModelOptions<T>? options = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(provider);

        return new(provider, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CrudListViewModel{T}"/> with the specified data provider, CRUD service and optional configuration.
    /// </summary>
    /// <param name="provider">The data provider for the list view model.</param>
    /// <param name="crudService">The CRUD service used by the view model commands.</param>
    /// <param name="options">Optional configuration for the list view model.</param>
    /// <typeparam name="T">The type of items in the list view model.</typeparam>
    /// <returns>A new instance of <see cref="CrudListViewModel{T}"/>.</returns>
    public static CrudListViewModel<T> CreateCrud<T>(
        IListDataProvider<T> provider,
        ICrudService<T> crudService,
        ListViewModelOptions<T>? options = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(crudService);

        return new(provider, crudService, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ListViewModel{T}"/> with the specified items and optional configuration.
    /// </summary>
    /// <param name="items">The items to be displayed in the list view model.</param>
    /// <param name="options">Optional configuration for the list view model.</param>
    /// <typeparam name="T">The type of items in the list view model.</typeparam>
    /// <returns>A new instance of <see cref="ListViewModel{T}"/>.</returns>
    public static ListViewModel<T> Create<T>(
        IEnumerable<T> items,
        ListViewModelOptions<T>? options = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(items);

        var source = SourceEngine<T>.From(items, readOnly: false);
        return new(source, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="CrudListViewModel{T}"/> with the specified items, CRUD service and optional configuration.
    /// </summary>
    /// <param name="items">The items to be displayed in the list view model.</param>
    /// <param name="crudService">The CRUD service used by the view model commands.</param>
    /// <param name="options">Optional configuration for the list view model.</param>
    /// <typeparam name="T">The type of items in the list view model.</typeparam>
    /// <returns>A new instance of <see cref="CrudListViewModel{T}"/>.</returns>
    public static CrudListViewModel<T> CreateCrud<T>(
        IEnumerable<T> items,
        ICrudService<T> crudService,
        ListViewModelOptions<T>? options = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(crudService);

        var source = SourceEngine<T>.From(items, readOnly: false);
        var provider = new ExtendedCollectionDataProvider<T>(new(source, options?.Scheduler));

        return new(provider, crudService, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="VirtualizedListViewModel{T}"/> with the specified data provider and optional configuration.
    /// </summary>
    /// <param name="provider">The data provider for the list view model.</param>
    /// <param name="options">Optional configuration for the list view model.</param>
    /// <typeparam name="T">The type of items in the list view model.</typeparam>
    /// <returns>A new instance of <see cref="VirtualizedListViewModel{T}"/>.</returns>
    public static VirtualizedListViewModel<T> CreateVirtualized<T>(
        IListDataProvider<T> provider,
        ListViewModelOptions<T>? options = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(provider);

        return new(provider, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="VirtualizedListViewModel{T}"/> with the specified items and optional configuration.
    /// </summary>
    /// <param name="items">The items to be displayed in the list view model.</param>
    /// <param name="options">Optional configuration for the list view model.</param>
    /// <typeparam name="T">The type of items in the list view model.</typeparam>
    /// <returns>A new instance of <see cref="VirtualizedListViewModel{T}"/>.</returns>
    public static VirtualizedListViewModel<T> CreateVirtualized<T>(
        IEnumerable<T> items,
        ListViewModelOptions<T>? options = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(items);

        var source = SourceEngine<T>.From(items, readOnly: false);
        var provider = new ExtendedCollectionDataProvider<T>(new(source, options?.Scheduler));

        return new(provider, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="SelectableListViewModel{T}"/> with the specified items and optional configuration for selection mode and list view model options.
    /// </summary>
    /// <param name="items">The items to be displayed in the list view model.</param>
    /// <param name="options">Optional configuration for the list view model.</param>
    /// <param name="selectionMode">The selection mode for the list view model.</param>
    /// <typeparam name="T">The type of items in the list view model.</typeparam>
    /// <returns>A new instance of <see cref="SelectableListViewModel{T}"/>.</returns>
    public static SelectableListViewModel<T> CreateSelection<T>(
        IEnumerable<T> items,
        ListViewModelOptions<T>? options = null,
        SelectionMode selectionMode = SelectionMode.Multiple)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(items);

        options ??= new();

        var source = SourceEngine<T>.From(items, readOnly: false);
        var collection = new ExtendedCollection<T>(source, options.Scheduler);

        return new(collection, selectionMode, options);
    }

    /// <summary>
    /// Creates a selectable list view model from an existing extended collection.
    /// </summary>
    public static SelectableListViewModel<T> CreateSelection<T>(
        ExtendedCollection<T> collection,
        ListViewModelOptions<T>? options = null,
        SelectionMode selectionMode = SelectionMode.Multiple)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(collection);

        return new(collection, selectionMode, options);
    }
}
