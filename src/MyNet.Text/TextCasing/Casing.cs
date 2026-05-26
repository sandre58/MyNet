// -----------------------------------------------------------------------
// <copyright file="Casing.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Text.TextCasing;

/// <summary>
/// Provides common text casing transformations.
/// </summary>
public static class Casing
{
    /// <summary>
    /// Gets transforms a string to upper case.
    /// </summary>
    public static ITextCasingTransform UpperCase { get; } = new UpperCasingTransform();

    /// <summary>
    /// Gets transforms a string to lower case.
    /// </summary>
    public static ITextCasingTransform LowerCase { get; } = new LowerCasingTransform();

    /// <summary>
    /// Gets transforms a string to title case.
    /// </summary>
    public static ITextCasingTransform TitleCase { get; } = new TitleCasingTransform();

    /// <summary>
    /// Gets transforms a string to sentence case.
    /// </summary>
    public static ITextCasingTransform SentenceCase { get; } = new SentenceCasingTransform();

    /// <summary>
    /// Gets transforms a string to PascalCase.
    /// </summary>
    public static ITextCasingTransform PascalCase { get; } = new PascalCasingTransform();

    /// <summary>
    /// Gets transforms a string to camelCase.
    /// </summary>
    public static ITextCasingTransform CamelCase { get; } = new CamelCasingTransform();

    /// <summary>
    /// Gets transforms a string to snake_case.
    /// </summary>
    public static ITextCasingTransform SnakeCase { get; } = new SnakeCasingTransform();

    /// <summary>
    /// Gets transforms a string to kebab-case.
    /// </summary>
    public static ITextCasingTransform KebabCase { get; } = new KebabCasingTransform();
}
