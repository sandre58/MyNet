// -----------------------------------------------------------------------
// <copyright file="TextTruncationOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Text.Truncation;

/// <summary>
/// Options for truncating text. This is used to specify the length to truncate to, the string to truncate with and the direction to truncate from.
/// </summary>
public sealed record TextTruncationOptions
{
    /// <summary>
    /// Gets the length to truncate to. This is the total length of the truncated string, including the truncation string. For example, if the input string is "Hello World", the truncation string is "…" and the length is 5, the output will be "He…". If the length is less than or equal to the length of the truncation string, the output will be the truncation string. For example, if the input string is "Hello World", the truncation string is "…" and the length is 1, the output will be "…".
    /// </summary>
    public required int Length
    {
        get;
        init => field = value < 0 ? throw new ArgumentOutOfRangeException(nameof(value), "Length must be greater than or equal to 0.") : value;
    }

    /// <summary>
    /// Gets the string to use for truncation. This will be appended to the truncated string if the input string is longer than the specified length.
    /// </summary>
    public string TruncationString
    {
        get;
        init => field = value ?? throw new ArgumentNullException(nameof(value));
    }

        = "…";

    /// <summary>
    /// Gets the direction from which to truncate the string. This can be from the left or the right.
    /// </summary>
    public TruncateFrom Direction { get; init; } = TruncateFrom.Right;
}
