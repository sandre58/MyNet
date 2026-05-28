// -----------------------------------------------------------------------
// <copyright file="ImportMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Import;

/// <summary>
/// Defines import behavior for an item.
/// </summary>
public enum ImportMode
{
    /// <summary>
    /// Add the item when importing.
    /// </summary>
    Add,

    /// <summary>
    /// Update an existing item when importing.
    /// </summary>
    Update
}
