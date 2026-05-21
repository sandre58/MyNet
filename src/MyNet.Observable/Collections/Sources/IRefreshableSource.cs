// -----------------------------------------------------------------------
// <copyright file="IRefreshableSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Collections.Sources;

/// <summary>
/// Defines an interface for a refreshable source, which allows implementing classes to provide a mechanism for refreshing their data or state. This is particularly useful in scenarios where the underlying data may change and needs to be reloaded or updated to reflect the latest information.
/// </summary>
public interface IRefreshableSource
{
    /// <summary>
    /// Refreshes the data or state of the source. Implementing classes should provide the logic to reload or update their data when this method is called.
    /// </summary>
    void Refresh();
}
