// -----------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Helpers;
using MyNet.UI.Loading;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Shell host view model: drawers, composition of panel view models, and optional file menu navigation.
/// </summary>
public class MainWindowViewModel : ViewModelBase, IShellHostWithFileMenu, IShellNotificationsDrawerHost, IShellFileMenuDrawer
{
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="notificationsViewModel">Notifications panel view model.</param>
    /// <param name="cultureChrome">Shell culture selector.</param>
    /// <param name="themeChrome">Shell quick theme toggle.</param>
    /// <param name="appCommandsService">Application commands.</param>
    /// <param name="applicationBusy">Application-wide busy service.</param>
    /// <param name="taskbarProgress">Taskbar progress source.</param>
    /// <param name="dialogService">Dialog service.</param>
    /// <param name="notificationsDrawerCoordinator">Notifications drawer coordinator.</param>
    /// <param name="fileMenuViewModel">Optional file menu content; when omitted, file menu APIs are unavailable.</param>
    /// <param name="shellHostProvider">Optional provider that registers this instance as the current shell host.</param>
    /// <param name="commandFactory">Optional command factory.</param>
    public MainWindowViewModel(
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
        ICommandFactory? commandFactory = null)
    {
        ArgumentNullException.ThrowIfNull(notificationsViewModel);
        ArgumentNullException.ThrowIfNull(cultureChrome);
        ArgumentNullException.ThrowIfNull(themeChrome);
        ArgumentNullException.ThrowIfNull(appCommandsService);
        ArgumentNullException.ThrowIfNull(applicationBusy);
        ArgumentNullException.ThrowIfNull(taskbarProgress);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(notificationsDrawerCoordinator);

        NotificationsViewModel = notificationsViewModel;
        CultureChrome = cultureChrome;
        ThemeChrome = themeChrome;
        ApplicationBusy = applicationBusy;
        TaskbarProgress = taskbarProgress;
        _dialogService = dialogService;

        if (fileMenuViewModel is not null)
            FileMenuHost = new(fileMenuViewModel, this);

        var commands = commandFactory ?? RelayCommandFactory.Default;

        ToggleNotificationsCommand = commands.Create(ToggleNotifications, CanToggleNotifications);
        ToggleFileMenuCommand = commands.Create(ToggleFileMenu, CanToggleFileMenu);
        CloseDrawersCommand = commands.Create(CloseDrawers, CanUseShellChrome);
        ExitCommand = commands.Create(appCommandsService.Exit, CanUseShellChrome);

        notificationsDrawerCoordinator.Bind(this, notificationsViewModel);

        if (shellHostProvider is not null)
        {
            shellHostProvider.Attach(this);
            Disposables.Add(Disposable.Create(() => shellHostProvider.Detach(this)));
        }

        Disposables.Add(Disposable.Create(notificationsDrawerCoordinator.Unbind));
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

    /// <summary>
    /// Gets a value indicating whether a file menu is configured.
    /// </summary>
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

    /// <summary>
    /// Gets the file menu drawer view model when <see cref="HasFileMenu"/> is true.
    /// </summary>
    public FileMenuViewModel FileMenuViewModel => RequireFileMenuHost().FileMenuViewModel;

    /// <summary>
    /// Gets the product name displayed in the window title.
    /// </summary>
    public string ProductName { get; } = ApplicationHelper.GetProductName();

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public virtual string Title => IsDebug ? $"{ProductName} [Debug]" : ProductName;

    /// <summary>
    /// Closes all shell drawers.
    /// </summary>
    public virtual void CloseDrawers()
    {
        IsNotificationsOpen = false;
        IsFileMenuOpen = false;
        FileMenuHost?.OnCloseAllDrawers();
    }

    /// <summary>
    /// Applies an action to the notifications drawer.
    /// </summary>
    public void SetNotificationsDrawer(ShellDrawerAction action)
        => IsNotificationsOpen = action.ApplyToOpenState(IsNotificationsOpen);

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
           ?? throw new InvalidOperationException("File menu is not configured. Pass a FileMenuViewModel to MainWindowViewModel.");

    private void ToggleNotifications() => SetNotificationsDrawer(ShellDrawerAction.Toggle);

    private void ToggleFileMenu() => SetFileMenuDrawer(ShellDrawerAction.Toggle);

    private bool CanToggleNotifications()
        => CanUseShellChrome() && NotificationsViewModel.HasNotifications;

    private bool CanToggleFileMenu() => HasFileMenu && CanUseShellChrome();

    private bool CanUseShellChrome() => !_dialogService.HasOpenedDialogs;
}
