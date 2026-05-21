// -----------------------------------------------------------------------
// <copyright file="RecentFilesProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Binding;
using MyNet.UI.Legacy.ViewModels.FileHistory;
using MyNet.UI.Messages;
using MyNet.Utilities;
using MyNet.Utilities.Collections;
using MyNet.Utilities.IO.FileHistory;
using MyNet.Utilities.Messaging;

namespace MyNet.UI.Services.Providers;

/// <summary>
/// Provides a collection of recent files for the UI, synchronizing with the recent files service and manager.
/// Handles updates, image loading, and notifications for recent files.
/// </summary>
public sealed class RecentFilesProvider : IDisposable
{
    private readonly RecentFilesService _recentFilesService;
    private readonly RecentFilesManager _recentFilesManager;
    private readonly IRecentFileCommandsService _recentFileCommandsService;

    private readonly OptimizedObservableCollection<RecentFileViewModel> _source = [];
    private readonly CompositeDisposable _cleanup = [];
    private readonly IObservable<IChangeSet<RecentFileViewModel>> _observableItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecentFilesProvider"/> class.
    /// </summary>
    /// <param name="recentFilesService">The service providing recent files data.</param>
    /// <param name="recentFilesManager">The manager for recent files operations.</param>
    /// <param name="recentFileCommandsService">The service for recent file commands.</param>
    public RecentFilesProvider(RecentFilesService recentFilesService, RecentFilesManager recentFilesManager, IRecentFileCommandsService recentFileCommandsService)
    {
        _recentFileCommandsService = recentFileCommandsService;
        _recentFilesService = recentFilesService;
        _recentFilesManager = recentFilesManager;
        Items = new(_source);
        _observableItems = Items.ToObservableChangeSet();

        // Subscribe to item changes and load images asynchronously
        _cleanup.Add(Items.ToObservableChangeSet().DisposeMany().OnItemAdded(async x => await x.LoadImageAsync().ConfigureAwait(false)).Subscribe());

        // Register for recent files changed notifications
        Messenger.Default?.Register<RecentFilesChangedMessage>(this, _ => Reload());
    }

    /// <summary>
    /// Gets the read-only collection of recent file view models.
    /// </summary>
    public ReadOnlyObservableCollection<RecentFileViewModel> Items { get; }

    /// <summary>
    /// Reloads the recent files collection from the service and updates the view models.
    /// </summary>
    public void Reload()
    {
        var recentFiles = _recentFilesService.GetAll();

        _source.UpdateFrom(recentFiles,
            x => _source.Add(new(x, _recentFilesManager, _recentFileCommandsService)),
            x => _source.Remove(x),
            (destination, source) => destination.Update(source),
            (x, y) => x.Path == y.Path);
    }

    /// <summary>
    /// Connects to the observable change set for recent file view models.
    /// </summary>
    /// <returns>An observable sequence of changes to the recent files collection.</returns>
    public IObservable<IChangeSet<RecentFileViewModel>> Connect() => _observableItems;

    /// <summary>
    /// Disposes the provider, unregistering notifications and cleaning up resources.
    /// </summary>
    public void Dispose()
    {
        Messenger.Default?.Unregister(this);
        _cleanup.Dispose();
    }
}
