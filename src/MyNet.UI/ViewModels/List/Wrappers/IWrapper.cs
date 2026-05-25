// -----------------------------------------------------------------------
// <copyright file="IWrapper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.List.Wrappers;

/// <summary>
/// Minimal contract for list item wrappers in the UI layer.
/// </summary>
/// <typeparam name="T">The wrapped item type.</typeparam>
public interface IWrapper<out T>
{
    /// <summary>
    /// Gets the wrapped source item.
    /// </summary>
    T Item { get; }
}
