// -----------------------------------------------------------------------
// <copyright file="ValidationMessages.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Static;
using MyNet.Observable.Localization;
using MyNet.Utilities;

namespace MyNet.Observable.Validation;

/// <summary>
/// Formats localized validation messages from <see cref="ValidationResources"/>.
/// </summary>
public static class ValidationMessages
{
    /// <summary>
    /// Formats a validation template using an already resolved property display name.
    /// </summary>
    /// <param name="template">The localized message template ({0} = field name).</param>
    /// <param name="propertyDisplayName">The resolved display name inserted at {0}.</param>
    /// <param name="args">Additional format arguments starting at {1}.</param>
    public static string Format(string template, string propertyDisplayName, params object?[] args)
    {
        ArgumentException.ThrowIfNullOrEmpty(template);
        ArgumentException.ThrowIfNullOrEmpty(propertyDisplayName);

        var formatArgs = new object?[args.Length + 1];
        formatArgs[0] = propertyDisplayName;
        args.CopyTo(formatArgs, 1);

        return template.FormatWith(CultureInfo.CurrentCulture, formatArgs);
    }

    /// <summary>
    /// Formats a validation template by translating the property key with the current culture.
    /// </summary>
    /// <param name="template">The localized message template ({0} = field name).</param>
    /// <param name="propertyDisplayKey">The translation key for the property display name.</param>
    /// <param name="args">Additional format arguments starting at {1}.</param>
    public static string FormatForPropertyKey(string template, string propertyDisplayKey, params object?[] args)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyDisplayKey);

        var displayName = propertyDisplayKey.Translate();
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = propertyDisplayKey;

        return Format(template, displayName, args);
    }
}
