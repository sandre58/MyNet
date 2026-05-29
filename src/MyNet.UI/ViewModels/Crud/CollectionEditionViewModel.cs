// -----------------------------------------------------------------------
// <copyright file="CollectionEditionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MyNet.UI.Commands;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Provides a reusable base implementation for editable collections.
/// </summary>
/// <typeparam name="TRow">The edited row view model type.</typeparam>
public abstract class CollectionEditionViewModel<TRow> : ViewModelBase, IEditionStateViewModel
    where TRow : class, INotifyPropertyChanged
{
    private bool _isLoading;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionEditionViewModel{TRow}"/> class.
    /// </summary>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    protected CollectionEditionViewModel(ICommandFactory? commandFactory = null)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;

        AddCommand = commands.Create<TRow>(Add, CanAdd);
        RemoveCommand = commands.Create<TRow>(Remove, CanRemove);

        Items.CollectionChanged += HandleCollectionChanged;
    }

    /// <summary>
    /// Gets the command that adds a row.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Gets the command that removes a row.
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets the edited rows.
    /// </summary>
    public ObservableCollection<TRow> Items { get; } = [];

    /// <inheritdoc />
    public bool IsDirty { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Replaces current rows with <paramref name="items"/> and marks the editor as clean.
    /// </summary>
    /// <param name="items">The rows to load.</param>
    protected void LoadItems(IEnumerable<TRow>? items)
    {
        _isLoading = true;

        try
        {
            UnsubscribeFromItems(Items);
            Items.Clear();

            if (items is not null)
            {
                foreach (var item in items)
                    Items.Add(item);
            }

            EnsureEditableRow();
            SubscribeToItems(Items);
            IsDirty = false;
            OnItemsStateChanged();
        }
        finally
        {
            _isLoading = false;
        }
    }

    /// <summary>
    /// Creates a new row to add to <see cref="Items"/>.
    /// </summary>
    /// <returns>The new row instance.</returns>
    protected abstract TRow CreateNewItem();

    /// <summary>
    /// Determines whether a row can trigger add.
    /// </summary>
    /// <param name="item">The source row.</param>
    /// <returns><see langword="true"/> when add is allowed.</returns>
    protected virtual bool CanAdd(TRow? item) => item is not null;

    /// <summary>
    /// Determines whether a row can be removed.
    /// </summary>
    /// <param name="item">The row to remove.</param>
    /// <returns><see langword="true"/> when remove is allowed.</returns>
    protected virtual bool CanRemove(TRow? item) => item is not null;

    /// <summary>
    /// Ensures editor constraints after a load/remove operation.
    /// </summary>
    protected virtual void EnsureEditableRow()
    {
    }

    /// <summary>
    /// Called after row collection/state changed.
    /// </summary>
    protected virtual void OnItemsStateChanged()
    {
    }

    /// <summary>
    /// Adds the specified item to <see cref="Items"/>.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns><see langword="true"/> when the item was added; otherwise <see langword="false"/>.</returns>
    protected bool AddItem(TRow? item)
    {
        if (item is null)
            return false;

        Items.Add(item);
        return true;
    }

    /// <summary>
    /// Removes the specified item from <see cref="Items"/> when allowed.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><see langword="true"/> when the item was removed; otherwise <see langword="false"/>.</returns>
    protected bool RemoveItem(TRow? item)
    {
        if (!CanRemove(item) || item is null)
            return false;

        var removed = Items.Remove(item);

        if (removed)
            EnsureEditableRow();

        return removed;
    }

    private void Add(TRow? item)
    {
        if (!CanAdd(item))
            return;

        Items.Add(CreateNewItem());
    }

    private void Remove(TRow? item) => _ = RemoveItem(item);

    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
            UnsubscribeFromItems(e.OldItems.Cast<TRow>());

        if (e.NewItems is not null)
            SubscribeToItems(e.NewItems.Cast<TRow>());

        MarkDirtyIfNeeded();
        OnItemsStateChanged();
    }

    private void HandleItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        MarkDirtyIfNeeded();
        OnItemsStateChanged();
    }

    private void MarkDirtyIfNeeded()
    {
        if (!_isLoading)
            IsDirty = true;
    }

    private void SubscribeToItems(IEnumerable<TRow> items)
    {
        foreach (var item in items)
            item.PropertyChanged += HandleItemPropertyChanged;
    }

    private void UnsubscribeFromItems(IEnumerable<TRow> items)
    {
        foreach (var item in items)
            item.PropertyChanged -= HandleItemPropertyChanged;
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        Items.CollectionChanged -= HandleCollectionChanged;
        UnsubscribeFromItems(Items);
        base.DisposeManagedResources();
    }
}
