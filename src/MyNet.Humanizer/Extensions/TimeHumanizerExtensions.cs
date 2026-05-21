// -----------------------------------------------------------------------
// <copyright file="TimeHumanizerExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Humanizer.Temporal;
using MyNet.Utilities.Temporal.Decomposition;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TimeHumanizerExtensions
{
    extension(TimeSpan timeSpan)
    {
        /// <summary>
        /// Infers the tense from the specified time span.
        /// </summary>
        /// <returns>
        /// <see cref="Tense.Future"/> when the span is positive or zero;
        /// otherwise <see cref="Tense.Past"/>.
        /// </returns>
        public Tense Tense() => timeSpan >= TimeSpan.Zero ? Temporal.Tense.Future : Temporal.Tense.Past;
    }

    extension(TimeHumanizationOptions options)
    {
        /// <summary>
        /// Converts humanization options to decomposition options.
        /// </summary>
        /// <returns>The corresponding decomposition options.</returns>
        public TimeSpanDecompositionOptions ToDecompositionOptions()
        {
            ArgumentNullException.ThrowIfNull(options);

            return new()
            {
                MinUnit = options.MinUnit,
                MaxUnit = options.MaxUnit,
                Mode = options.DecompositionMode,
                MaxComponents = options.MaxComponents,
                IncludeZeroUnits = options.IncludeZeroUnits,
                Quantizer = options.Quantizer
            };
        }
    }
}
