// -----------------------------------------------------------------------
// <copyright file="NavigationRequestEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Provides data for a navigation request event, including the target of the navigation and any parameters that may be needed for the navigation. The Target property indicates the destination of the navigation, which could be a view, a view model, or another component within the application. The Parameter property can hold any additional information that may be required to perform the navigation, such as query parameters, state information, or other context. This class is used in conjunction with the INavigationRequestSource interface to facilitate navigation within the application.
/// </summary>
public class NavigationRequestEventArgs : EventArgs
{
    /// <summary>
    /// Gets the target of the navigation request, which could be a view, a view model, or another component within the application. This property is used to specify where the navigation should go when the NavigationRequested event is raised.
    /// </summary>
    public object? Target { get; init; }

    /// <summary>
    /// Gets any additional information that may be required to perform the navigation, such as query parameters, state information, or other context. This property can be used to pass necessary data along with the navigation request to ensure that the target of the navigation has the information it needs to function properly after the navigation occurs.
    /// </summary>
    public object? Parameter { get; init; }
}
