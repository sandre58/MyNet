// -----------------------------------------------------------------------
// <copyright file="IncrementTransformOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Text.Formatting;

/// <summary>
/// Represents options used to configure <see cref="IncrementTransform"/>.
/// </summary>
public sealed class IncrementTransformOptions
{
    /// <summary>
    /// Gets the collection of existing strings to check against.
    /// </summary>
    public required IEnumerable<string> ExistingStrings { get; init; }

    /// <summary>
    /// Gets the increment strategy kind.
    /// </summary>
    public IncrementKind Kind { get; init; } = IncrementKind.Numeric;

    /// <summary>
    /// Gets the minimum increment value. Current is 1.
    /// </summary>
    public int MinIncrement { get; init; } = 1;

    /// <summary>
    /// Gets the increment step. Current is 1.
    /// </summary>
    public int Step { get; init; } = 1;

    /// <summary>
    /// Gets the optional numeric format, used only when <see cref="Kind"/> is <see cref="IncrementKind.Numeric"/>.
    /// </summary>
    public string? NumericFormat { get; init; }
}
