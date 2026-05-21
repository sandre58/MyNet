// -----------------------------------------------------------------------
// <copyright file="FileNameTextSanitizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MyNet.Utilities.Text.Sanitization;

/// <summary>
/// Sanitizes text so it can be used as a file name.
/// </summary>
public sealed class FileNameTextSanitizer : ITextSanitizerTransform
{
    private static readonly HashSet<char> InvalidCharacters = [.. Path.GetInvalidFileNameChars()];

    /// <inheritdoc/>
    public string Apply(string input, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(culture);

        if (input.Length == 0)
            return input;

        var sb = new StringBuilder(input.Length);
        foreach (var ch in input.Where(ch => !InvalidCharacters.Contains(ch)))
        {
            sb.Append(ch);
        }

        return sb.ToString().Trim();
    }
}
