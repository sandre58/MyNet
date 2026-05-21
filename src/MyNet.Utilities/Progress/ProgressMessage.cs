// -----------------------------------------------------------------------
// <copyright file="ProgressMessage.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Utilities.Progress;

/// <summary>
/// Represents a localizable progress message with optional format parameters.
/// </summary>
/// <param name="Message">The message string. May contain format placeholders (e.g. <c>{0}</c>) resolved by <paramref name="Parameters"/>.</param>
/// <param name="Parameters">Optional format parameters passed to <see cref="string.Format(string, object[])"/> when <see cref="ToString"/> is called.</param>
public sealed record ProgressMessage(string Message, params object[] Parameters)
{
    /// <summary>
    /// Returns the formatted message string.
    /// When <see cref="Parameters"/> is non-empty the message is treated as a composite-format string;
    /// otherwise the raw <see cref="Message"/> value is returned.
    /// </summary>
    public override string ToString()
        => Parameters.Length > 0
            ? string.Format(CultureInfo.CurrentCulture, Message, Parameters)
            : Message;
}
