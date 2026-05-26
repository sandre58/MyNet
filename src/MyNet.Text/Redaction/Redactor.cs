// -----------------------------------------------------------------------
// <copyright file="Redactor.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace MyNet.Text.Redaction;

/// <summary>
/// Provides common text redaction transforms.
/// </summary>
public static partial class Redactor
{
    /// <summary>
    /// Gets a generic redactor that preserves the first and last two characters.
    /// </summary>
    public static ITextRedactorTransform Generic { get; } = new FixedMaskTextRedactor(new() { ShowStart = 2, ShowEnd = 2 });

    /// <summary>
    /// Gets an email redactor that preserves first character and domain suffix.
    /// </summary>
    public static ITextRedactorTransform Email { get; } = new RegexTextRedactor(EmailRegex());

    /// <summary>
    /// Gets a phone redactor that masks all digits except the last two.
    /// </summary>
    public static ITextRedactorTransform Phone { get; } = new RegexTextRedactor(PhoneRegex(), "*");

    /// <summary>
    /// Gets a card number redactor that masks all digits except the last four.
    /// </summary>
    public static ITextRedactorTransform CardNumber { get; } = new RegexTextRedactor(CardNumberRegex(), "*");

    [GeneratedRegex("(?<=.).(?=[^@]*?@)|(?<=@.).(?=[^.]*\\.)", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"\d(?=(?:\D*\d){2})", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"\d(?=(?:\D*\d){4})", RegexOptions.Compiled)]
    private static partial Regex CardNumberRegex();
}
