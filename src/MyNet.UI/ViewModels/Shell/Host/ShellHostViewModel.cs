// -----------------------------------------------------------------------
// <copyright file="ShellHostViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Loading;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Shell.Chrome;
using MyNet.UI.ViewModels.Shell.Drawers;
using MyNet.UI.ViewModels.Shell.FileMenu;
using MyNet.UI.ViewModels.Shell.Notifications;
using MyNet.UI.ViewModels.Shell.Taskbar;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell.Host;

/// <summary>
/// Shell host view model: drawers, composition of panel view models, and optional file menu navigation.
/// </summary>
[SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed through the shared disposables collection.")]
public class ShellHostViewModel : ViewModelBase, IShellCapabilities, IShellHostWithFileMenu, IShellNotificationsDrawerHost, IShellFileMenuDrawer
{
    private readonly ShellOptions _options;
    private readonly IDialogService _dialogService;
    private readonly ShellNotificationsHost _notificationsHost;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellHostViewModel"/> class.
    /// </summary>
    public ShellHostViewModel(
        IApplicationInfo applicationInfo,
        ShellNotificationsViewModel notificationsViewModel,
        ShellCultureViewModel cultureChrome,
        ShellThemeViewModel themeChrome,
        IAppCommandsService appCommandsService,
        IBusyService applicationBusy,
        ITaskbarProgressSource taskbarProgress,
        IDialogService dialogService,
        ShellNotificationsDrawerCoordinator notificationsDrawerCoordinator,
        FileMenuViewModel? fileMenuViewModel = null,
        IShellHostProvider? shellHostProvider = null,
        ShellOptions? options = null,
        ICommandFactory? commandFactory = null)
    {
        ArgumentNullException.ThrowIfNull(applicationInfo);
        ArgumentNullException.ThrowIfNull(notificationsViewModel);
        ArgumentNullException.ThrowIfNull(cultureChrome);
        ArgumentNullException.ThrowIfNull(themeChrome);
        ArgumentNullException.ThrowIfNull(appCommandsService);
        ArgumentNullException.ThrowIfNull(applicationBusy);
        ArgumentNullException.ThrowIfNull(taskbarProgress);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(notificationsDrawerCoordinator);

        _options = options ?? ShellOptions.Default;
        _dialogService = dialogService;

        ProductName = applicationInfo.ProductName;
        NotificationsViewModel = notificationsViewModel;
        CultureChrome = cultureChrome;
        ThemeChrome = themeChrome;
        ApplicationBusy = applicationBusy;
        TaskbarProgress = taskbarProgress;

        if (fileMenuViewModel is not null)
            FileMenuHost = new(fileMenuViewModel, this, OnFileMenuDrawerOpening);

        var commands = commandFactory.GetOrDefault();

        _notificationsHost = new(
            this,
            notificationsViewModel,
            notificationsDrawerCoordinator,
            CanUseShellChrome,
            OnNotificationsDrawerOpening);

        ToggleNotificationsCommand = _notificationsHost.CreateToggleCommand(commands);
        ToggleFileMenuCommand = commands.Create(ToggleFileMenu, CanToggleFileMenu);
        CloseDrawersCommand = commands.Create(CloseDrawers, CanUseShellChrome);
        ExitCommand = commands.Create(appCommandsService.Exit, CanUseShellChrome);

        if (shellHostProvider is not null)
        {
            shellHostProvider.Attach(this);
            Disposables.Add(Disposable.Create(() => shellHostProvider.Detach(this)));
        }

        Disposables.Add(_notificationsHost);
    }

#if DEBUG
    /// <summary>
    /// Gets a value indicating whether the application runs in debug configuration.
    /// </summary>
    public bool IsDebug { get; } = true;
#else
    /// <summary>
    /// Gets a value indicating whether the application runs in debug configuration.
    /// </summary>
    public bool IsDebug { get; }
#endif

    /// <inheritdoc />
    public bool HasFileMenu => FileMenuHost is not null;

    /// <summary>
    /// Gets the optional file menu host.
    /// </summary>
    public ShellFileMenuHost? FileMenuHost { get; }

    /// <summary>
    /// Gets the notifications panel view model.
    /// </summary>
    public ShellNotificationsViewModel NotificationsViewModel { get; }

    /// <summary>
    /// Gets the shell culture selector.
    /// </summary>
    public ShellCultureViewModel CultureChrome { get; }

    /// <summary>
    /// Gets the shell quick theme toggle.
    /// </summary>
    public ShellThemeViewModel ThemeChrome { get; }

    /// <summary>
    /// Gets the application-wide busy service (overlay).
    /// </summary>
    public IBusyService ApplicationBusy { get; }

    /// <summary>
    /// Gets the taskbar progress source synchronized with <see cref="ApplicationBusy"/> by <see cref="BusyTaskbarCoordinator"/>.
    /// </summary>
    public ITaskbarProgressSource TaskbarProgress { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the notifications drawer is open.
    /// </summary>
    public bool IsNotificationsOpen { get; set => SetProperty(ref field, value); }

    /// <inheritdoc />
    public bool IsFileMenuOpen { get; set => SetProperty(ref field, value); }

    /// <inheritdoc />
    public ICommand ToggleNotificationsCommand { get; }

    /// <summary>
    /// Gets the command that toggles the file menu drawer.
    /// </summary>
    public ICommand ToggleFileMenuCommand { get; }

    /// <summary>
    /// Gets the command that closes all shell drawers.
    /// </summary>
    public ICommand CloseDrawersCommand { get; }

    /// <summary>
    /// Gets the command that exits the application.
    /// </summary>
    public ICommand ExitCommand { get; }

    /// <inheritdoc />
    public FileMenuViewModel FileMenuViewModel => RequireFileMenuHost().FileMenuViewModel;

    /// <summary>
    /// Gets the product name displayed in the window title.
    /// </summary>
    public string ProductName { get; }

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public virtual string Title => IsDebug ? $"{ProductName} [Debug]" : ProductName;

    /// <summary>
    /// Closes all shell drawers.
    /// </summary>
    public virtual void CloseDrawers()
    {
        _notificationsHost.Close();
        IsFileMenuOpen = false;
        FileMenuHost?.OnCloseAllDrawers();
    }

    /// <summary>
    /// Applies an action to the notifications drawer.
    /// </summary>
    public void SetNotificationsDrawer(ShellDrawerAction action) => _notificationsHost.SetDrawer(action);

    /// <inheritdoc />
    public void SetFileMenuDrawer(ShellDrawerAction action)
    {
        if (FileMenuHost is not null)
            FileMenuHost.SetDrawer(action);
        else
            IsFileMenuOpen = action.ApplyToOpenState(IsFileMenuOpen);
    }

    /// <inheritdoc />
    public void OpenFileMenuContent<T>()
        where T : class, IWorkspaceViewModel
        => RequireFileMenuHost().OpenFileMenuContent<T>();

    /// <inheritdoc />
    public void OpenFileMenuContent(Type contentType) => RequireFileMenuHost().OpenFileMenuContent(contentType);

    /// <inheritdoc />
    public void CloseFileMenuContent() => RequireFileMenuHost().CloseFileMenuContent();

    /// <inheritdoc />
    public void SetFileMenuContentVisibility<T>(ShellDrawerAction action)
        where T : class, IWorkspaceViewModel
        => RequireFileMenuHost().SetFileMenuContentVisibility<T>(action);

    /// <inheritdoc />
    public void SetFileMenuContentVisibility(Type contentType, ShellDrawerAction action)
        => RequireFileMenuHost().SetFileMenuContentVisibility(contentType, action);

    private ShellFileMenuHost RequireFileMenuHost()
        => FileMenuHost
           ?? throw new InvalidOperationException("File menu is not configured. Pass a FileMenuViewModel to ShellHostViewModel.");

    private void ToggleFileMenu() => SetFileMenuDrawer(ShellDrawerAction.Toggle);

    private bool CanToggleFileMenu() => HasFileMenu && CanUseShellChrome();

    private bool CanUseShellChrome() => !_dialogService.HasOpenedDialogs;

    private void OnNotificationsDrawerOpening()
    {
        if (!_options.MutuallyExclusiveDrawers)
            return;

        IsFileMenuOpen = false;
        FileMenuHost?.OnCloseAllDrawers();
    }

    private void OnFileMenuDrawerOpening()
    {
        if (!_options.MutuallyExclusiveDrawers)
            return;

        _notificationsHost.Close();
    }
}
