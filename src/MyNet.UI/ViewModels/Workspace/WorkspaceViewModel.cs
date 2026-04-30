// -----------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.ViewModels.Common;

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Provides a reusable base implementation for workspace view models.
/// </summary>
public class WorkspaceViewModel : ViewModelBase, IWorkspaceViewModel, INavigationRequestSource
{
    /// <summary>
    /// Occurs when the workspace requests navigation.
    /// </summary>
    public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

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
    /// Raises a navigation request.
    /// </summary>
    /// <param name="target">The navigation target.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    protected void RequestNavigation(object? target, object? parameter = null)
        => NavigationRequested?.Invoke(this, new() { Target = target, Parameter = parameter });

    /// <summary>
    /// Handles culture changes by updating the workspace title using the <see cref="CreateTitle"/> method. This ensures that the title is localized when the culture changes.
    /// </summary>
    protected override void OnCultureChanged()
    {
        Title = CreateTitle();
        base.OnCultureChanged();
    }
}
