// -----------------------------------------------------------------------
// <copyright file="NavigationMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Navigation.Models;

/// <summary>
/// Defines the mode of a navigation operation, indicating whether the navigation is a normal navigation, a backward navigation, or a forward navigation.
/// </summary>
public enum NavigationMode
{
    /// <summary>
    /// Indicates a normal navigation, where the user is navigating to a new page or view.
    /// </summary>
    Normal,

    /// <summary>
    /// Indicates a backward navigation, where the user is navigating back to a previous page or view in the navigation history.
    /// </summary>
    Back,

    /// <summary>
    /// Indicates a forward navigation, where the user is navigating forward to a page or view that was previously navigated back from in the navigation history.
    /// </summary>
    Forward
}
