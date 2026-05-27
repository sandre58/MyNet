// -----------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Loading;

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Provides a reusable base implementation for workspace view models.
/// </summary>
[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public class WorkspaceViewModel : ViewModelBase, IWorkspaceViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkspaceViewModel"/> class.
    /// </summary>
    /// <param name="busyService">Optional busy service used to manage loading state.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    protected WorkspaceViewModel(IBusyService? busyService = null, ICommandFactory? commandFactory = null)
        : base(busyService)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;

        RefreshCommand = commands.Create(async () => await RefreshAsync().ConfigureAwait(false), CanRefresh);
        ResetCommand = commands.Create(async () => await ResetAsync().ConfigureAwait(false), CanReset);
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
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the workspace display mode.
    /// </summary>
    public ScreenMode Mode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the workspace is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

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
    public Task RefreshAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(async ct => await OnRefreshAsync(ct).ConfigureAwait(false), cancellationToken);

    /// <summary>
    /// Resets the workspace state and content.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ResetAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(async ct => await OnResetAsync(ct).ConfigureAwait(false), cancellationToken);

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
    /// Creates the workspace title. This method is called when the culture changes to update the title accordingly. The default implementation returns null, so derived classes should override it to provide a localized title if needed.
    /// </summary>
    /// <returns>The workspace title.</returns>
    protected virtual string? CreateTitle() => null;

    /// <summary>
    /// Handles culture changes by updating the workspace title using the <see cref="CreateTitle"/> method. This ensures that the title is localized when the culture changes.
    /// </summary>
    protected override void OnCultureChanged()
    {
        Title = CreateTitle();
        base.OnCultureChanged();
    }
}
