// -----------------------------------------------------------------------
// <copyright file="ProgressionBusy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace MyNet.UI.Loading.Models;

/// <summary>
/// Busy state for multi-step operations with progress value, current title, and message history.
/// </summary>
public class ProgressionBusy : Busy
{
    /// <summary>
    /// Gets or sets the title displayed for the busy operation.
    /// </summary>
    public string Title { get; set => SetProperty(ref field, value); } = string.Empty;

    /// <summary>
    /// Gets or sets the progress value of the busy operation.
    /// </summary>
    public double Value { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the collection of messages history.
    /// </summary>
    public ObservableCollection<string> Messages { get; } = [];

    /// <summary>
    /// Reports progress with optional message.
    /// </summary>
    public void Report(double value, string? message = null)
    {
        Value = value;

        if (!string.IsNullOrWhiteSpace(message))
        {
            Title = message;
            Messages.Add(message);
        }
    }
}
