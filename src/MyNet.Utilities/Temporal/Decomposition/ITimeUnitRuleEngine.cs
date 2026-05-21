// -----------------------------------------------------------------------
// <copyright file="ITimeUnitRuleEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Temporal.Decomposition;

/// <summary>
/// Selects which <see cref="TimeUnit"/> values should represent a given <see cref="TimeSpan"/>.
/// This engine only selects units and never computes unit values.
/// </summary>
public interface ITimeUnitRuleEngine
{
    /// <summary>
    /// Selects units from largest to smallest within the provided bounds.
    /// </summary>
    /// <param name="span">The source duration.</param>
    /// <param name="minUnit">The smallest allowed unit.</param>
    /// <param name="maxUnit">The largest allowed unit.</param>
    /// <returns>The ordered list of units to decompose against.</returns>
    /// <example>
    /// 63 minutes with min=Minute and max=Hour can return [Hour, Minute].
    /// </example>
    IReadOnlyList<TimeUnit> Select(TimeSpan span, TimeUnit minUnit, TimeUnit maxUnit);
}
