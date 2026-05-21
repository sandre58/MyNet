// -----------------------------------------------------------------------
// <copyright file="QuantityRenderingMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Text.Templating;

/// <summary>
/// Specifies how quantity information should be rendered in translation templates.
/// </summary>
public enum QuantityRenderingMode
{
    /// <summary>
    /// Indicates that quantity information should not be rendered at all. Placeholders related to quantity will be ignored and not included in the final output.
    /// </summary>
    None,

    /// <summary>
    /// Indicates that quantity information should be rendered in the final output. Placeholders related to quantity will be replaced with their corresponding values and included in the rendered translation string.
    /// </summary>
    Prefix,

    /// <summary>
    /// Indicates that quantity information should be rendered in the final output, but as a suffix. Placeholders related to quantity will be replaced with their corresponding values and included in the rendered translation string.
    /// </summary>
    Suffix,

    /// <summary>
    /// Indicates that only placeholders related to quantity should be rendered, without including the actual quantity values in the final output.
    /// </summary>
    PlaceholderOnly
}
