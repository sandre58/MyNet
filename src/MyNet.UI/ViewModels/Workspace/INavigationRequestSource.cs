// -----------------------------------------------------------------------
// <copyright file="INavigationRequestSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Defines the contract for a navigation request source, which can raise navigation requests to navigate to a different view or section of the application. The NavigationRequested event is triggered when a navigation request is made, and it provides information about the target of the navigation and any parameters that may be needed for the navigation. This interface can be implemented by view models or other components that need to initiate navigation within the application.
/// </summary>
public interface INavigationRequestSource
{
    /// <summary>
    /// Occurs when a navigation request is made.
    /// </summary>
    event EventHandler<NavigationRequestEventArgs> NavigationRequested;
}
