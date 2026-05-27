// -----------------------------------------------------------------------
// <copyright file="IndeterminateBusy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Loading.Models;

/// <summary>
/// Busy state for operations without a known duration (spinner). Optional status message.
/// </summary>
public class IndeterminateBusy : Busy
{
    /// <summary>
    /// Gets or sets the message displayed during the busy operation.
    /// </summary>
    public string? Message { get; set => SetProperty(ref field, value); }
}
