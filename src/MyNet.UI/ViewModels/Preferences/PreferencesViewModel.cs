// -----------------------------------------------------------------------
// <copyright file="PreferencesViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentValidation;
using MyNet.Globalization.Culture;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Notifications;
using MyNet.UI.Resources;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Crud;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Preferences;

/// <summary>
/// Preferences dialog with tabbed pages and persistent settings save/reset/reload.
/// </summary>
[SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in DisposeManagedResources.")]
public sealed class PreferencesViewModel : EditionViewModel, ITabWorkspaceViewModel
{
    private readonly IPersistentPreferencesService _preferencesService;
    private readonly PreferencesTabWorkspace _tabs;

    /// <summary>
    /// Initializes a new instance of the <see cref="PreferencesViewModel"/> class.
    /// </summary>
    public PreferencesViewModel(
        IPersistentPreferencesService preferencesService,
        IEnumerable<IWorkspaceViewModel> pages,
        IDialogService dialogService,
        INotificationPublisher notificationPublisher,
        ICommandFactory? commandFactory = null,
        IValidator? validator = null,
        ICultureService? cultureService = null)
        : base(dialogService, notificationPublisher, commandFactory, validator, cultureService)
    {
        _preferencesService = preferencesService ?? throw new ArgumentNullException(nameof(preferencesService));
        ArgumentNullException.ThrowIfNull(pages);

        _tabs = new(commandFactory, cultureService);
        _tabs.AddPages(pages);
    }

    /// <inheritdoc />
    public ReadOnlyObservableCollection<IWorkspaceViewModel> Workspaces => _tabs.Workspaces;

    /// <inheritdoc />
    public IWorkspaceViewModel? SelectedWorkspace => _tabs.SelectedWorkspace;

    /// <inheritdoc />
    public int SelectedIndex => _tabs.SelectedIndex;

    /// <summary>
    /// Gets the command to navigate to a specific tab.
    /// </summary>
    public ICommand GoToTabCommand => _tabs.GoToTabCommand;

    /// <summary>
    /// Gets the command to navigate to the next tab.
    /// </summary>
    public ICommand GoToNextTabCommand => _tabs.GoToNextTabCommand;

    /// <summary>
    /// Gets the command to navigate to the previous tab.
    /// </summary>
    public ICommand GoToPreviousTabCommand => _tabs.GoToPreviousTabCommand;

    /// <inheritdoc />
    public void Select(IWorkspaceViewModel workspace) => _tabs.Select(workspace);

    /// <inheritdoc />
    public void Select(int index) => _tabs.Select(index);

    /// <inheritdoc />
    protected override string? CreateTitle(CultureInfo culture) => UiResources.Preferences;

    /// <inheritdoc />
    protected override void SaveCore() => _preferencesService.Save();

    /// <inheritdoc />
    protected override async Task OnResetAsync(CancellationToken cancellationToken = default)
    {
        _preferencesService.Reset();

        foreach (var workspace in Workspaces)
            await workspace.ResetAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task OnRefreshAsync(CancellationToken cancellationToken = default)
    {
        _preferencesService.Reload();

        foreach (var workspace in Workspaces)
            await workspace.RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        _tabs.Dispose();
        base.DisposeManagedResources();
    }
}
