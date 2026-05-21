// -----------------------------------------------------------------------
// <copyright file="Normalization.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

namespace MyNet.Utilities.Text.Normalization;

/// <summary>
/// Provides common text normalization transforms.
/// </summary>
public static class Normalization
{
    /// <summary>
    /// Gets a transform that removes diacritics.
    /// </summary>
    public static ITextNormalizationTransform RemoveDiacritics { get; } = new DiacriticsRemovalTransform();

    /// <summary>
    /// Gets a transform that normalizes to Form C.
    /// </summary>
    public static ITextNormalizationTransform UnicodeFormC { get; } = new UnicodeNormalizationTransform(NormalizationForm.FormC);

    /// <summary>
    /// Gets a transform that trims and collapses whitespace.
    /// </summary>
    public static ITextNormalizationTransform CleanWhitespace { get; } = new WhitespaceNormalizationTransform(new());
}
