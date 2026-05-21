// -----------------------------------------------------------------------
// <copyright file="IBusyScope.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;

namespace MyNet.UI.Loading.Models;

/// <summary>
/// Defines a scope for a busy operation, which provides access to the associated IBusy instance and a cancellation token. The scope should be disposed when the busy operation is completed to automatically clear the busy state and cancel any ongoing operations if needed.
/// </summary>
public interface IBusyScope : IDisposable
{
    /// <summary>
    /// Gets the IBusy instance associated with this busy scope, which can be used to set the busy state and manage the busy operation. The IBusy instance should be used to indicate when the operation is in progress and when it is completed, allowing the UI to show appropriate loading indicators and prevent user interactions during the operation.
    /// </summary>
    IBusy Busy { get; }

    /// <summary>
    /// Gets the cancellation token associated with this busy scope, which can be used to cancel any ongoing operations related to the busy state. The cancellation token should be monitored by any asynchronous operations started within the busy scope, allowing them to respond to cancellation requests and clean up resources appropriately when the busy operation is canceled or completed.
    /// </summary>
    CancellationToken CancellationToken { get; }
}
