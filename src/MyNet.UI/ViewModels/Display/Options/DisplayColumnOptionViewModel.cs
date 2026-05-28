// -----------------------------------------------------------------------
// <copyright file="DisplayColumnOptionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable;

namespace MyNet.UI.ViewModels.Display.Options;

/// <summary>
/// Represents one configurable column option for list display mode.
/// </summary>
public sealed class DisplayColumnOptionViewModel : ObservableObject
{
    private readonly bool _defaultIsVisible;
    private readonly string _defaultWidth;
    private readonly int _defaultOrder;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayColumnOptionViewModel"/> class.
    /// </summary>
    /// <param name="identifier">The unique column identifier.</param>
    /// <param name="canBeHidden">Whether the column can be hidden.</param>
    /// <param name="isVisible">Whether the column is initially visible.</param>
    /// <param name="width">The column width string.</param>
    /// <param name="order">The initial display order.</param>
    public DisplayColumnOptionViewModel(
        string identifier,
        bool canBeHidden = true,
        bool isVisible = true,
        string width = "*",
        int order = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        Identifier = identifier;
        CanBeHidden = canBeHidden;
        IsVisible = isVisible;
        Width = width;
        Order = order;

        _defaultIsVisible = isVisible;
        _defaultWidth = width;
        _defaultOrder = order;
    }

    /// <summary>
    /// Gets the unique column identifier.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the column can be hidden.
    /// </summary>
    public bool CanBeHidden { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets a value indicating whether the column is visible.
    /// </summary>
    public bool IsVisible { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the column width.
    /// </summary>
    public string Width { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the column display order.
    /// </summary>
    public int Order { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Resets this column option to its default values.
    /// </summary>
    public void Reset()
    {
        IsVisible = _defaultIsVisible;
        Width = _defaultWidth;
        Order = _defaultOrder;
    }
}
