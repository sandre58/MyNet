// -----------------------------------------------------------------------
// <copyright file="SelectionMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Collections.Selection;

/// <summary>
/// Specifies the selection mode for a list or collection.
/// </summary>
public enum SelectionMode
{
    /// <summary>
    /// Multiple items can be selected.
    /// </summary>
    Multiple,

    /// <summary>
    /// Only a single item can be selected.
    /// </summary>
    Single
}
