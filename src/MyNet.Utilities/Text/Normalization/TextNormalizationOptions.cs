// -----------------------------------------------------------------------
// <copyright file="TextNormalizationOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

namespace MyNet.Utilities.Text.Normalization;

/// <summary>
/// Options used by text normalization transforms.
/// </summary>
public sealed record TextNormalizationOptions
{
    /// <summary>
    /// Gets the target Unicode normalization form.
    /// </summary>
    public NormalizationForm UnicodeForm { get; init; } = NormalizationForm.FormC;

    /// <summary>
    /// Gets a value indicating whether leading and trailing white spaces are trimmed.
    /// </summary>
    public bool Trim { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether whitespace runs are collapsed to a single space.
    /// </summary>
    public bool CollapseWhitespace { get; init; } = true;
}
