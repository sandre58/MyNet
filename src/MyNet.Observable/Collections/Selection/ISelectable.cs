// -----------------------------------------------------------------------
// <copyright file="ISelectable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Collections.Selection;

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
