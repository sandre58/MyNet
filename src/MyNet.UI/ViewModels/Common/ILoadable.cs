// -----------------------------------------------------------------------
// <copyright file="ILoadable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Common;

/// <summary>
/// Defines the contract for a loadable view model, which can be loaded, refreshed, and reset asynchronously. The load state is represented by the LoadState enum, which indicates whether the view model is not loaded, loading, loaded, or failed to load.
/// </summary>
public interface ILoadable
{
    /// <summary>
    /// Gets the current load state of the view model, which indicates whether it is not loaded, loading, loaded, or failed to load. This property can be used to determine when to display loading indicators, error messages, or the actual content of the view model.
    /// </summary>
    LoadState State { get; }

    /// <summary>
    /// Asynchronously loads the view model, which may involve fetching data, initializing properties, or performing other operations necessary to prepare the view model for use. The method should update the State property accordingly to reflect the current load state of the view model.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously refreshes the view model, which may involve re-fetching data or updating properties to reflect the latest state. The method should update the State property accordingly to reflect the current load state of the view model.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous refresh operation.</returns>
    Task RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously resets the view model, which may involve clearing data, resetting properties, or performing other operations necessary to return the view model to its initial state. The method should update the State property accordingly to reflect the current load state of the view model.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous reset operation.</returns>
    Task ResetAsync(CancellationToken cancellationToken = default);
}
