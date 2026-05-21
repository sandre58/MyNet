// -----------------------------------------------------------------------
// <copyright file="HumanFriendlyTimeSpanQuantizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities;
using MyNet.Utilities.Temporal.Decomposition;

namespace MyNet.Humanizer.Temporal;

/// <summary>
/// Rounds decomposed values to friendlier thresholds (for instance 59 seconds to 1 minute).
/// </summary>
/// <example>
/// [59 seconds] becomes [1 minute].
/// [1 hour, 59 minutes] can become [2 hours].
/// </example>
public sealed class HumanFriendlyTimeSpanQuantizer : ITimeSpanQuantizer
{
    /// <summary>
    /// Gets the default quantizer instance.
    /// </summary>
    public static HumanFriendlyTimeSpanQuantizer Default { get; } = new();

    /// <inheritdoc/>
    public IReadOnlyList<TimeUnitValue> Quantize(IReadOnlyList<TimeUnitValue> values)
    {
        if (values.Count <= 1)
            return values;

        var ordered = values.OrderByDescending(x => x.Unit).ToList();
        var dict = ordered.ToDictionary(x => x.Unit, x => x.Value);

        for (var i = ordered.Count - 1; i > 0; i--)
        {
            var currentUnit = ordered[i].Unit;
            var nextUnit = ordered[i - 1].Unit;
            var currentValue = dict[currentUnit];
            var threshold = GetThreshold(currentUnit);

            if (threshold > 0 && currentValue >= threshold)
            {
                dict[nextUnit]++;
                dict[currentUnit] = 0;

                // Drop smaller details after promoting to a bigger unit.
                for (var j = i + 1; j < ordered.Count; j++)
                    dict[ordered[j].Unit] = 0;
            }
        }

        return ordered.ConvertAll(x => x with { Value = dict[x.Unit] });
    }

    private static int GetThreshold(TimeUnit unit)
        => unit switch
        {
            TimeUnit.Millisecond => 500,
            TimeUnit.Second => 59,
            TimeUnit.Minute => 59,
            TimeUnit.Hour => 23,
            TimeUnit.Day => 6,
            TimeUnit.Week => 3,
            TimeUnit.Month => 11,
            _ => 0
        };
}
