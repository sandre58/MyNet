// -----------------------------------------------------------------------
// <copyright file="RecentFilesViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.IO.FileHistory;
using MyNet.Observable.Collections;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Notifications;
using MyNet.UI.Services;
using MyNet.UI.Services.FileHistory;
using MyNet.UI.ViewModels.List;

namespace MyNet.UI.ViewModels.FileHistory;

/// <summary>
/// View model for the recent-files list.
/// </summary>
public sealed class RecentFilesViewModel : ListViewModelBase<RecentFileViewModel, ExtendedCollection<RecentFileViewModel>>
{
    private readonly IRecentFilesOperations _recentFilesOperations;
    private readonly IRecentFileCommandsService _recentFileCommandsService;
    private readonly IDialogService _dialogService;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly RecentFilesListOptions _listOptions;
    private readonly Dictionary<string, RecentFileViewModel> _entries = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="RecentFilesViewModel"/> class.
    /// </summary>
    public RecentFilesViewModel(
        IRecentFilesOperations recentFilesOperations,
        IRecentFileCommandsService recentFileCommandsService,
        IDialogService dialogService,
        INotificationPublisher notificationPublisher,
        ICommandFactory? commandFactory = null)
        : base(CreateCollection(), CreateListOptions(out var listOptions).Filters, listOptions.Sorting)
    {
        _recentFilesOperations = recentFilesOperations ?? throw new ArgumentNullException(nameof(recentFilesOperations));
        _recentFileCommandsService = recentFileCommandsService ?? throw new ArgumentNullException(nameof(recentFileCommandsService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
        _listOptions = listOptions;

        var commands = commandFactory.GetOrDefault();
        OpenCommand = commands.Create<RecentFileViewModel?>(async item =>
        {
            if (item is not null)
                await item.OpenAsync().ConfigureAwait(false);
        },
            item => item is not null);

        RemoveItemsCommand = commands.Create<IEnumerable<RecentFileViewModel>>(async items =>
        {
            foreach (var item in items ?? [])
                await _recentFilesOperations.RemoveAsync(item.Path).ConfigureAwait(false);
        });

        ReloadCommand = commands.Create(() => ReloadAsync());

        _recentFilesOperations.Changed += OnRecentFilesChanged;
        Disposables.Add(Disposable.Create(() => _recentFilesOperations.Changed -= OnRecentFilesChanged));

        _ = ReloadAsync();
    }

    /// <summary>
    /// Gets the command that opens a recent file.
    /// </summary>
    public ICommand OpenCommand { get; }

    /// <summary>
    /// Gets the command that removes items from the recent-files list.
    /// </summary>
    public ICommand RemoveItemsCommand { get; }

    /// <summary>
    /// Gets the command that reloads the list from storage.
    /// </summary>
    public ICommand ReloadCommand { get; }

    /// <summary>
    /// Gets or sets the search text applied to name and path.
    /// </summary>
    public string SearchText
    {
        get => _listOptions.NameFilter.Value ?? string.Empty;
        set => _listOptions.SetSearchText(value);
    }

    /// <summary>
    /// Reloads items from the recent-files service.
    /// </summary>
    public async Task ReloadAsync(CancellationToken cancellationToken = default) => await ExecuteSafeAsync(async ct =>
    {
        var files = await _recentFilesOperations.GetAllAsync(ct).ConfigureAwait(false);
        var items = new List<RecentFileViewModel>(files.Count);
        var activePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in files)
        {
            activePaths.Add(file.Path);

            if (!_entries.TryGetValue(file.Path, out var viewModel))
            {
                viewModel = CreateItemViewModel(file);
                _entries[file.Path] = viewModel;
            }
            else
            {
                viewModel.Update(file);
            }

            items.Add(viewModel);
        }

        foreach (var path in _entries.Keys.Where(path => !activePaths.Contains(path)).ToList())
            _entries.Remove(path);

        Collection.Set(items);

        foreach (var viewModel in items.Where(x => x.Image is null))
            await viewModel.LoadImageAsync(ct).ConfigureAwait(false);
    },
        cancellationToken).ConfigureAwait(false);

    private static ExtendedCollection<RecentFileViewModel> CreateCollection() =>
        ExtendedCollection.Create<RecentFileViewModel>();

    private static RecentFilesListOptions CreateListOptions(out RecentFilesListOptions options)
    {
        options = RecentFilesListOptions.Create();
        return options;
    }

    private void OnRecentFilesChanged(object? sender, EventArgs e) => _ = ReloadAsync();

    private RecentFileViewModel CreateItemViewModel(RecentFile file) =>
        new(
            file,
            _recentFilesOperations,
            _recentFileCommandsService,
            _dialogService,
            _notificationPublisher);
}
