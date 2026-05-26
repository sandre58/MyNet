// -----------------------------------------------------------------------
// <copyright file="TimeHumanizerExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Facade;
using MyNet.Humanizer.Temporal;
using MyNet.Temporal.Decomposition;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer.Facade;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TimeHumanizerExtensions
{
    extension(TimeSpan timeSpan)
    {
        /// <summary>
        /// Humanizes the time span using default options.
        /// </summary>
        public string Humanize(CultureInfo? culture = null) => timeSpan.Humanize(TimeHumanizationPresets.Default, culture);

        /// <summary>
        /// Humanizes the time span using the specified options.
        /// </summary>
        public string Humanize(TimeHumanizationOptions options, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(options);

            var decompositionOptions = options.ToDecompositionOptions();

            var decomposition = timeSpan.Decompose(decompositionOptions);

            return options.Mode switch
                {
                    TimeHumanizationMode.Duration => Localizer.ForCulture(culture).GetRequired<ITimeHumanizer>().HumanizeDuration(decomposition, options.ListFormatting),
                    TimeHumanizationMode.Relative => Localizer.ForCulture(culture).GetRequired<ITimeHumanizer>().HumanizeRelativeTime(decomposition, options.Tense ?? timeSpan.Tense()),
                    _ => string.Empty
                };
        }

        public string Humanize(Action<TimeHumanizationBuilder> configure, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new TimeHumanizationBuilder();

            configure(builder);

            return timeSpan.Humanize(builder.Build(), culture);
        }
    }
}
