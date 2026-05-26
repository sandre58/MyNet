// -----------------------------------------------------------------------
// <copyright file="TimeSpanDecompositionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Primitives;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Temporal.Decomposition;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TimeSpanDecompositionExtensions
{
    extension(TimeSpan timeSpan)
    {
        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with humanized options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <returns>A list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition.</returns>
        public IReadOnlyList<TimeUnitValue> Decompose() => timeSpan.Decompose(TimeSpanDecompositionPresets.Humanized);

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with humanized options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition can be customized using the optional configuration action for <see cref="TimeSpanDecompositionBuilder"/>, which allows for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <param name="options">The options to use for the decomposition.</param>
        /// <returns>A list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition.</returns>
        public IReadOnlyList<TimeUnitValue> Decompose(TimeSpanDecompositionOptions options) => TimeSpanDecomposer.Default.Decompose(timeSpan, options);

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with humanized options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition can be customized using the optional configuration action for <see cref="TimeSpanDecompositionBuilder"/>, which allows for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <param name="configure">The configuration action to customize the decomposition.</param>
        /// <returns>A list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition.</returns>
        public IReadOnlyList<TimeUnitValue> Decompose(Action<TimeSpanDecompositionBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new TimeSpanDecompositionBuilder(TimeSpanDecomposer.Default, timeSpan);
            configure(builder);

            return builder.Decompose();
        }

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a custom decomposer. The method takes an <see cref="ITimeSpanDecomposer"/> to perform the decomposition based on specific rules and returns a list of <see cref="TimeUnitValue"/> representing the value of each time unit in the decomposition. The optional configuration action allows for customizing the decomposition options before performing the decomposition.
        /// </summary>
        /// <param name="decomposer">The custom decomposer to use for the decomposition.</param>
        /// <param name="options">The options to use for the decomposition.</param>
        /// <returns>A list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition.</returns>
        public IReadOnlyList<TimeUnitValue> Decompose(ITimeSpanDecomposer decomposer, TimeSpanDecompositionOptions options)
        {
            ArgumentNullException.ThrowIfNull(decomposer);
            return decomposer.Decompose(timeSpan, options);
        }

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a custom decomposer. The method takes an <see cref="ITimeSpanDecomposer"/> to perform the decomposition based on specific rules and returns a list of <see cref="TimeUnitValue"/> representing the value of each time unit in the decomposition. The optional configuration action allows for customizing the decomposition options before performing the decomposition.
        /// </summary>
        /// <param name="decomposer">The custom decomposer to use for the decomposition.</param>
        /// <param name="configure">The configuration action to customize the decomposition.</param>
        /// <returns>A list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition.</returns>
        public IReadOnlyList<TimeUnitValue> Decompose(ITimeSpanDecomposer decomposer, Action<TimeSpanDecompositionBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(decomposer);
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new TimeSpanDecompositionBuilder(decomposer, timeSpan);

            configure(builder);

            return builder.Decompose();
        }

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with humanized options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <returns>A list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition.</returns>
        public IReadOnlyList<TimeUnitValue> Humanized() => timeSpan.Decompose(TimeSpanDecompositionPresets.Humanized);

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with compact options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <returns>A list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition.</returns>
        public IReadOnlyList<TimeUnitValue> Compact() => timeSpan.Decompose(TimeSpanDecompositionPresets.Compact);

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with full options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <returns>A list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition.</returns>
        public IReadOnlyList<TimeUnitValue> Full() => timeSpan.Decompose(TimeSpanDecompositionPresets.Full);

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with largest unit options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <returns>A <see cref="TimeUnitValue"/> object representing the value of the largest time unit in the decomposition.</returns>
        public TimeUnitValue Largest() => timeSpan.Largest(TimeSpanDecompositionPresets.LargestUnit);

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with largest unit options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <param name="options">The options to customize the decomposition.</param>
        /// <returns>A <see cref="TimeUnitValue"/> object representing the value of the largest time unit in the decomposition.</returns>
        public TimeUnitValue Largest(TimeSpanDecompositionOptions options)
        {
            var result = timeSpan.Decompose(options);

            return result.Count > 0 ? result[0] : new(0, options.MinUnit);
        }

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with largest unit options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <param name="configure">A delegate to configure the decomposition options.</param>
        /// <returns>A <see cref="TimeUnitValue"/> object representing the value of the largest time unit in the decomposition.</returns>
        public TimeUnitValue Largest(Action<TimeSpanDecompositionBuilder> configure)
        {
            var result = timeSpan.Decompose(configure);

            return result.Count > 0 ? result[0] : new(0, TimeUnit.Millisecond);
        }

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with smallest unit options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <returns>A <see cref="TimeUnitValue"/> object representing the value of the smallest time unit in the decomposition.</returns>
        public TimeUnitValue Smallest() => timeSpan.Largest(TimeSpanDecompositionPresets.SmallestUnit);

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with smallest unit options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <param name="options">The options to customize the decomposition.</param>
        /// <returns>A <see cref="TimeUnitValue"/> object representing the value of the smallest time unit in the decomposition.</returns>
        public TimeUnitValue Smallest(TimeSpanDecompositionOptions options)
        {
            var result = timeSpan.Decompose(options);

            return result.Count > 0 ? result[^1] : new(0, options.MinUnit);
        }

        /// <summary>
        /// Decomposes the <see cref="TimeSpan"/> into its constituent time units (e.g., years, months, days, hours, minutes, seconds) using a default decomposer with smallest unit options. The method returns a list of <see cref="TimeUnitValue"/> objects representing the value of each time unit in the decomposition. The decomposition is performed according to the rules defined in the default decomposer and can be customized using the provided options for <see cref="TimeSpanDecompositionOptions"/>, which allow for specifying options such as which time units to include in the decomposition and how to handle rounding or truncation of values.
        /// </summary>
        /// <param name="configure">A delegate to configure the decomposition options.</param>
        /// <returns>A <see cref="TimeUnitValue"/> object representing the value of the smallest time unit in the decomposition.</returns>
        public TimeUnitValue Smallest(Action<TimeSpanDecompositionBuilder> configure)
        {
            var result = timeSpan.Decompose(configure);

            return result.Count > 0 ? result[^1] : new(0, TimeUnit.Millisecond);
        }
    }
}
