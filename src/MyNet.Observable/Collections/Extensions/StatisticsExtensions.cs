// -----------------------------------------------------------------------
// <copyright file="StatisticsExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Statistics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Creates reactive statistics over an <see cref="ExtendedCollection{T}"/> filtered view.
/// </summary>
public static class StatisticsExtensions
{
    extension<T>(ExtendedCollection<T> collection)
        where T : notnull
    {
        /// <summary>
        /// Tracks view metrics and numeric aggregates (sum, average, min, max) over the filtered view.
        /// </summary>
        /// <param name="valueSelector">Selects the numeric value for each item.</param>
        public CollectionStatistics<T, double> Statistics(Func<T, double> valueSelector) =>
            CollectionStatistics<T, double>.ForDouble(collection, valueSelector);

        /// <summary>
        /// Tracks view metrics and <see cref="TimeSpan"/> aggregates over the filtered view.
        /// </summary>
        /// <param name="valueSelector">Selects the duration for each item.</param>
        public CollectionStatistics<T, TimeSpan> Statistics(Func<T, TimeSpan> valueSelector) =>
            CollectionStatistics<T, TimeSpan>.ForTimeSpan(collection, valueSelector);
    }
}
