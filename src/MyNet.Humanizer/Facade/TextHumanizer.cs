// -----------------------------------------------------------------------
// <copyright file="TextHumanizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using MyNet.Globalization.Culture;
using MyNet.Humanizer.Display;
using MyNet.Humanizer.Display.Registration;

namespace MyNet.Humanizer.Facade;

public static class TextHumanizer
{
    private static IDisplayTextService _service = CreateDisplayTextService();
    private static int _configured;

    public static void Configure(IDisplayTextService service)
    {
        ArgumentNullException.ThrowIfNull(service);

        if (Interlocked.Exchange(ref _configured, 1) == 1)
        {
            throw new InvalidOperationException("TextHumanizer has already been configured.");
        }

        _service = service;
    }

    /// <summary>
    /// Gets the display text for a given value of type T, using the appropriate display text strategy registered for that type and taking into account culture-specific information to determine the appropriate display text.
    /// </summary>
    /// <param name="value">The value for which to get the display text.</param>
    /// <param name="options">The options to use when generating the display text.</param>
    /// <param name="culture">The culture to use for determining the display text.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The display text for the given value.</returns>
    public static string Humanize<T>(T value, DisplayTextOptions options, CultureInfo? culture = null)
        where T : notnull => _service.GetDisplayText(value, options, culture);

    private static DisplayTextService CreateDisplayTextService()
    {
        var defaultRegistry = new DisplayTextStrategyRegistry(new Dictionary<Type, IDisplayTextStrategy>());
        return new(new DisplayTextStrategyResolver(defaultRegistry), new ThreadCultureContext());
    }
}
