// -----------------------------------------------------------------------
// <copyright file="Formatter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Text.Formatting;

/// <summary>
/// Provides common text formatting transforms.
/// </summary>
public static class Formatter
{
    /// <summary>
    /// Gets a transform that converts text to initials.
    /// </summary>
    public static ITextFormatterTransform Initials { get; } = new InitialsFormatterTransform();
}
