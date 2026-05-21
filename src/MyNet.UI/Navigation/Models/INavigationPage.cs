// -----------------------------------------------------------------------
// <copyright file="INavigationPage.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Navigation.Models;

/// <summary>
/// Defines the contract for a page that can participate in navigation operations, including handling navigation events such as navigating to, navigating from, and navigating back to the page.
/// </summary>
public interface INavigationPage : INavigationLifecycle;
