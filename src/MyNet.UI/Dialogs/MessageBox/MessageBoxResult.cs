// -----------------------------------------------------------------------
// <copyright file="MessageBoxResult.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Specifies which message box button the user selected.
/// </summary>
public enum MessageBoxResult
{
    /// <summary>No button was selected (e.g. programmatic close).</summary>
    None,

    /// <summary>The user selected OK.</summary>
    Ok,

    /// <summary>The user selected Cancel.</summary>
    Cancel,

    /// <summary>The user selected Yes.</summary>
    Yes,

    /// <summary>The user selected No.</summary>
    No
}
