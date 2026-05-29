// -----------------------------------------------------------------------
// <copyright file="ImportBySourcesDialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentValidation;
using MyNet.UI.Commands;
using MyNet.UI.Notifications;

namespace MyNet.UI.ViewModels.Import;

/// <summary>
/// Provides a source-driven import dialog workflow.
/// </summary>
/// <typeparam name="T">The import item type.</typeparam>
public class ImportBySourcesDialogViewModel<T> : ImportDialogViewModel<T>
    where T : ImportItemViewModel
{
    private readonly ImportItemsProvider<T> _itemsProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportBySourcesDialogViewModel{T}"/> class.
    /// </summary>
    /// <param name="sources">The available import sources.</param>
    /// <param name="list">The import list view model.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    /// <param name="notificationPublisher">Optional notification publisher used to display validation errors.</param>
    /// <param name="validator">Optional validator used to validate this view model.</param>
    public ImportBySourcesDialogViewModel(
        IReadOnlyCollection<IImportSourceViewModel<T>> sources,
        ImportListViewModel<T> list,
        ICommandFactory? commandFactory = null,
        INotificationPublisher? notificationPublisher = null,
        IValidator? validator = null)
        : base(list, notificationPublisher, commandFactory, validator)
    {
        _itemsProvider = new(sources);

        var commands = commandFactory ?? RelayCommandFactory.Default;
        LoadSourceCommand = commands.Create<IImportSourceViewModel<T>>(async x => await LoadSourceAsync(x).ConfigureAwait(false), x => x is not null);
        ReloadSourceCommand = commands.Create(ReloadSource, CanReloadSource);
        ClearLoadedItemsCommand = commands.Create(ClearLoadedItems, CanClearLoadedItems);
    }

    /// <summary>
    /// Gets available import sources.
    /// </summary>
    public IReadOnlyCollection<IImportSourceViewModel<T>> Sources => _itemsProvider.Sources;

    /// <summary>
    /// Gets the currently loaded source.
    /// </summary>
    public IImportSourceViewModel<T>? CurrentSource { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets a value indicating whether source items are currently shown.
    /// </summary>
    public bool ShowList { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the command that loads items for the selected source.
    /// </summary>
    public ICommand LoadSourceCommand { get; }

    /// <summary>
    /// Gets the command that reloads current source data.
    /// </summary>
    public ICommand ReloadSourceCommand { get; }

    /// <summary>
    /// Gets the command that resets the loaded list.
    /// </summary>
    public ICommand ClearLoadedItemsCommand { get; }

    /// <summary>
    /// Initializes all sources.
    /// </summary>
    public override async Task OnOpenedAsync()
    {
        await base.OnOpenedAsync().ConfigureAwait(false);
        await Task.WhenAll(Sources.Select(x => x.InitializeAsync(CancellationToken.None))).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override bool CanValidate() => ShowList && base.CanValidate();

    private async Task LoadSourceAsync(IImportSourceViewModel<T>? source)
    {
        if (source is null)
            return;

        await ExecuteSafeAsync(_ =>
        {
            _itemsProvider.LoadSource(source);
            List.Load(_itemsProvider.Items);
            CurrentSource = source;
            ShowList = true;
            return Task.CompletedTask;
        }).ConfigureAwait(false);
    }

    private bool CanReloadSource() => ShowList && CurrentSource is not null;

    private void ReloadSource()
    {
        if (!CanReloadSource())
            return;

        _itemsProvider.Reload();

        if (CurrentSource is not null)
        {
            _itemsProvider.LoadSource(CurrentSource);
            List.Load(_itemsProvider.Items);
        }
    }

    private bool CanClearLoadedItems() => ShowList;

    private void ClearLoadedItems()
    {
        _itemsProvider.Clear();
        List.Load(Array.Empty<T>());
        CurrentSource = null;
        ShowList = false;
    }
}
