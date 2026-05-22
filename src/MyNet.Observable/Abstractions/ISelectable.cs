// -----------------------------------------------------------------------
// <copyright file="ISelectable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines the contract for an object that can be selected and notifies when its selection state changes.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Gets or sets a value indicating whether the object can be selected.
    /// </summary>
    bool IsSelectable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the object is currently selected.
    /// </summary>
    bool IsSelected { get; set; }
}
