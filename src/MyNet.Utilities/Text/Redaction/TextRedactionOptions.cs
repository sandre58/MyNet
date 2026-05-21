// -----------------------------------------------------------------------
// <copyright file="TextRedactionOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Text.Redaction;

/// <summary>
/// Options for fixed-mask redaction.
/// </summary>
public sealed record TextRedactionOptions
{
    /// <summary>
    /// Gets the number of leading characters kept visible.
    /// </summary>
    public int ShowStart
    {
        get;
        init => field = value < 0 ? throw new ArgumentOutOfRangeException(nameof(value)) : value;
    }

    /// <summary>
    /// Gets the number of trailing characters kept visible.
    /// </summary>
    public int ShowEnd
    {
        get;
        init => field = value < 0 ? throw new ArgumentOutOfRangeException(nameof(value)) : value;
    }

    /// <summary>
    /// Gets the mask character used for hidden characters.
    /// </summary>
    public char MaskCharacter { get; init; } = '*';
}
