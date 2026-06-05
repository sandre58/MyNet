// -----------------------------------------------------------------------
// <copyright file="DisplayTextService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Culture;

namespace MyNet.Humanizer.Display;

/// <summary>
/// Current implementation of <see cref="IDisplayTextService"/> that retrieves display texts using registered display text strategies and culture-specific information provided by an <see cref="ICultureContext"/>.
/// </summary>
/// <param name="resolver">The resolver used to obtain the appropriate display text strategy for a given type.</param>
/// <param name="cultureContext">The culture context used to determine the current culture.</param>
public class DisplayTextService(IDisplayTextStrategyResolver resolver, ICultureContext cultureContext) : IDisplayTextService
{
    /// <inheritdoc/>
    public string GetDisplayText<T>(T value, DisplayTextOptions options, CultureInfo? culture = null)
        where T : notnull
    {
        var cultureToUse = culture ?? cultureContext.CurrentCulture;

        if (typeof(T) == value.GetType())
            return resolver.GetRequired<T>().GetDisplayText(value, options, cultureToUse);

        return resolver.GetRequiredForType(value.GetType()).GetDisplayText(value, options, cultureToUse);
    }
}
