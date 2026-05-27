// -----------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.Globalization.Culture;
using MyNet.Observable;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.UI.Commands;

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Provides a reusable base implementation for workspace view models.
/// </summary>
public class WorkspaceViewModel : ViewModelBase, IWorkspaceViewModel, IEventAware<CultureChangedEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkspaceViewModel"/> class.
    /// </summary>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    /// <param name="cultureService">Optional culture service used to manage culture changes.</param>
    protected WorkspaceViewModel(
        ICommandFactory? commandFactory = null,
        ICultureService? cultureService = null)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;

        RefreshCommand = commands.Create(async () => await RefreshAsync().ConfigureAwait(false), CanRefresh);
        ResetCommand = commands.Create(async () => await ResetAsync().ConfigureAwait(false), CanReset);

        if (cultureService is not null)
        {
            this.ReactOnCultureChanged(cultureService);
        }

        ScheduleInitialTitle(cultureService?.CurrentCulture ?? CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Gets the command to refresh the workspace content.
    /// </summary>
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// Gets the command to reset the workspace state and content.
    /// </summary>
    public ICommand ResetCommand { get; }

    /// <summary>
    /// Gets or sets the workspace title.
    /// </summary>
    public string? Title
    {
        get;

        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets or sets the workspace display mode.
    /// </summary>
    public ScreenMode Mode { get; set => SetProperty(ref field, value); } = ScreenMode.Unknown;

    /// <summary>
    /// Gets or sets a value indicating whether the workspace is enabled.
    /// </summary>
    public bool IsEnabled { get; set => SetProperty(ref field, value); } = true;

    /// <summary>
    /// Determines whether the workspace can be refreshed.
    /// </summary>
    /// <returns><see langword="true"/> when the workspace can be refreshed; otherwise <see langword="false"/>.</returns>
    protected virtual bool CanRefresh() => true;

    /// <summary>
    /// Determines whether the workspace can be reset.
    /// </summary>
    /// <returns><see langword="true"/> when the workspace can be reset; otherwise <see langword="false"/>.</returns>
    protected virtual bool CanReset() => true;

    /// <inheritdoc />
    Task ILoadable.LoadAsync(CancellationToken cancellationToken) => LoadAsync(cancellationToken);

    /// <summary>
    /// Refreshes the workspace content.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RefreshAsync(CancellationToken cancellationToken = default) => ExecuteAsync(async ct => await OnRefreshAsync(ct).ConfigureAwait(false), cancellationToken);

    /// <summary>
    /// Resets the workspace state and content.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ResetAsync(CancellationToken cancellationToken = default) => ExecuteAsync(async ct => await OnResetAsync(ct).ConfigureAwait(false), cancellationToken);

    /// <summary>
    /// Performs the asynchronous refresh logic.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnRefreshAsync(CancellationToken cancellationToken = default) => OnLoadAsync(cancellationToken);

    /// <summary>
    /// Performs the asynchronous reset logic.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnResetAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>
    /// Creates the workspace title for the specified culture.
    /// Called from <see cref="ApplyTitle"/> after construction and on culture changes.
    /// </summary>
    /// <param name="culture">The culture information to use for creating the title.</param>
    /// <returns>The title to assign, or <see langword="null"/> to leave <see cref="Title"/> unchanged.</returns>
    protected virtual string? CreateTitle(CultureInfo culture) => null;

    /// <inheritdoc/>
    public virtual void OnEvent(CultureChangedEvent e) => ApplyTitle(e.Culture);

    /// <summary>
    /// Assigns <see cref="Title"/> from <see cref="CreateTitle"/> when the result is not <see langword="null"/>.
    /// </summary>
    /// <param name="culture">The culture used to build the title.</param>
    protected void ApplyTitle(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var title = CreateTitle(culture);

        if (title is not null)
            Title = title;
    }

    /// <summary>
    /// Schedules an initial title assignment using the current culture.
    /// </summary>
    /// <param name="culture">The culture to use for the initial title assignment.</param>
    private void ScheduleInitialTitle(CultureInfo culture)
    {
        var sync = SynchronizationContext.Current;

        if (sync is not null)
        {
            sync.Post(_ => ApplyTitle(culture), null);
            return;
        }

        ThreadPool.QueueUserWorkItem(_ => ApplyTitle(culture));
    }
}
