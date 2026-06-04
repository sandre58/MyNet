// -----------------------------------------------------------------------
// <copyright file="CrudListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.List.Services;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Provides a list view model enriched with CRUD commands.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class CrudListViewModel<T> : ListViewModel<T>
    where T : notnull
{
    private readonly ICrudService<T> _crudService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrudListViewModel{T}"/> class.
    /// </summary>
    public CrudListViewModel(
        IListDataProvider<T> dataProvider,
        ICrudService<T> crudService,
        Factories.ListViewModelOptions<T>? options = null)
        : base(dataProvider, options)
    {
        _crudService = crudService ?? throw new ArgumentNullException(nameof(crudService));
        var commands = (options?.CommandFactory).GetOrDefault();

        AddCommand = commands.Create(() => AddAsync(), CanAdd);
        EditCommand = commands.CreateRequired<T>(item => EditAsync(item), CanEdit);
        DeleteCommand = commands.CreateRequired<T>(item => DeleteAsync(item), CanDelete);
    }

    /// <summary>
    /// Gets the command used to add an item.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets the command used to edit an item.
    /// </summary>
    public ICommand EditCommand { get; }

    /// <summary>
    /// Gets the command used to delete an item.
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// Determines whether an add operation is currently allowed.
    /// </summary>
    protected virtual bool CanAdd() => true;

    /// <summary>
    /// Determines whether an edit operation is currently allowed for a specific item.
    /// </summary>
    protected virtual bool CanEdit(T item) => true;

    /// <summary>
    /// Determines whether a delete operation is currently allowed for a specific item.
    /// </summary>
    protected virtual bool CanDelete(T item) => true;

    /// <summary>
    /// Executes item creation and hooks into extension points.
    /// </summary>
    protected virtual async Task AddAsync(CancellationToken cancellationToken = default)
    {
        var item = await _crudService.CreateAsync(cancellationToken).ConfigureAwait(false);
        if (item is not null)
            await OnItemAddedAsync(item, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes item edition and hooks into extension points.
    /// </summary>
    protected virtual async Task EditAsync(T item, CancellationToken cancellationToken = default)
    {
        if (await _crudService.UpdateAsync(item, cancellationToken).ConfigureAwait(false))
            await OnItemEditedAsync(item, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes item deletion and hooks into extension points.
    /// </summary>
    protected virtual async Task DeleteAsync(T item, CancellationToken cancellationToken = default)
    {
        if (await _crudService.DeleteAsync(item, cancellationToken).ConfigureAwait(false))
            await OnItemDeletedAsync(item, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Extension point called when an item has been added.
    /// </summary>
    protected virtual Task OnItemAddedAsync(T item, CancellationToken cancellationToken = default)
    {
        RequestPipelineRefresh();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Extension point called when an item has been edited.
    /// </summary>
    protected virtual Task OnItemEditedAsync(T item, CancellationToken cancellationToken = default)
    {
        RequestPipelineRefresh();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Extension point called when an item has been deleted.
    /// </summary>
    protected virtual Task OnItemDeletedAsync(T item, CancellationToken cancellationToken = default)
    {
        RequestPipelineRefresh();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes multiple items and calls item-level hooks for each successful deletion.
    /// </summary>
    protected virtual async Task DeleteRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        var snapshot = items as IReadOnlyList<T> ?? [.. items];

        if (!await _crudService.DeleteRangeAsync(snapshot, cancellationToken).ConfigureAwait(false))
            return;

        foreach (var item in snapshot)
            await OnItemDeletedAsync(item, cancellationToken).ConfigureAwait(false);
    }
}
