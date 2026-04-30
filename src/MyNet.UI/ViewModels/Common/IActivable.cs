// -----------------------------------------------------------------------
// <copyright file="IActivable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Common;

/// <summary>
/// Defines the contract for an activable view model, which can be enabled or disabled. The IsEnabled property indicates whether the view model is currently active and can be interacted with. This interface can be used to control the availability of certain features or sections of the user interface based on the state of the view model or other conditions.
/// </summary>
public interface IActivable
{
    /// <summary>
    /// Gets or sets a value indicating whether the view model is currently enabled and can be interacted with.
    /// </summary>
    bool IsEnabled { get; set; }
}
