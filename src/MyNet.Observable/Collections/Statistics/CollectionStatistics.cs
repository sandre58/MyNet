// -----------------------------------------------------------------------
// <copyright file="CollectionStatistics.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;

namespace MyNet.Observable.Collections.Statistics;

/// <summary>
/// Reactive view statistics over the filtered items of an <see cref="ExtendedCollection{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <typeparam name="TValue">The aggregated value type.</typeparam>
public sealed class CollectionStatistics<T, TValue> : ObservableObject
    where T : notnull
{
    private readonly ExtendedCollection<T> _collection;
    private readonly Func<T, TValue> _valueSelector;
    private readonly IStatisticsComputer<TValue> _computer;
    private double _filteredPercentage;
    private TValue _sum = default!;
    private TValue _average = default!;
    private TValue _min = default!;
    private TValue _max = default!;

    private CollectionStatistics(ExtendedCollection<T> collection, Func<T, TValue> valueSelector, IStatisticsComputer<TValue> computer)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        _valueSelector = valueSelector ?? throw new ArgumentNullException(nameof(valueSelector));
        _computer = computer;

        Disposables.Add(_collection.Connect().Subscribe(_ => RecalculateValues()));
        _collection.PropertyChanged += OnCollectionPropertyChanged;
        Disposables.Add(Disposable.Create(() => _collection.PropertyChanged -= OnCollectionPropertyChanged));

        UpdateFilteredPercentage();
        RecalculateValues();
    }

    /// <summary>
    /// Gets the ratio of filtered items to source items, or <c>0</c> when the source is empty.
    /// </summary>
    public double FilteredPercentage => _filteredPercentage;

    /// <summary>
    /// Gets the sum of values across filtered items.
    /// </summary>
    public TValue Sum => _sum;

    /// <summary>
    /// Gets the average of values across filtered items.
    /// </summary>
    public TValue Average => _average;

    /// <summary>
    /// Gets the minimum value across filtered items.
    /// </summary>
    public TValue Min => _min;

    /// <summary>
    /// Gets the maximum value across filtered items.
    /// </summary>
    public TValue Max => _max;

    /// <summary>
    /// Creates a <see cref="CollectionStatistics{T, TType}"/> instance to track view metrics and numeric aggregates (sum, average, min, max) over the filtered view.
    /// </summary>
    /// <param name="collection">The collection whose filtered view is aggregated.</param>
    /// <param name="valueSelector">Selects the numeric value for each item.</param>
    /// <returns>A <see cref="CollectionStatistics{T, TType}"/> instance.</returns>
    internal static CollectionStatistics<T, double> ForDouble(ExtendedCollection<T> collection, Func<T, double> valueSelector) =>
        new(collection, valueSelector, DoubleStatisticsComputer.Instance);

    /// <summary>
    /// Creates a <see cref="CollectionStatistics{T, TType}"/> instance to track view metrics and <see cref="TimeSpan"/> aggregates over the filtered view.
    /// </summary>
    /// <param name="collection">The collection whose filtered view is aggregated.</param>
    /// <param name="valueSelector">Selects the duration for each item.</param>
    /// <returns>A <see cref="CollectionStatistics{T, TType}"/> instance.</returns>
    internal static CollectionStatistics<T, TimeSpan> ForTimeSpan(ExtendedCollection<T> collection, Func<T, TimeSpan> valueSelector) =>
        new(collection, valueSelector, TimeSpanStatisticsComputer.Instance);

    /// <summary>
    /// Handles property changes of the underlying collection to update the filtered percentage and recalculate aggregates when necessary.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void OnCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ExtendedCollection<>.Count) or nameof(ExtendedCollection<>.SourceCount))
            UpdateFilteredPercentage();

        if (e.PropertyName is nameof(ExtendedCollection<>.Count))
            RecalculateValues();
    }

    /// <summary>
    /// Updates the filtered percentage based on the current count of filtered items and the total source count. If the source count is zero, the filtered percentage is set to zero to avoid division by zero. Otherwise, it calculates the ratio of filtered items to source items and updates the <see cref="FilteredPercentage"/> property accordingly.
    /// </summary>
    private void UpdateFilteredPercentage()
    {
        var value = _collection.SourceCount == 0
            ? 0
            : (double)_collection.Count / _collection.SourceCount;

        SetProperty(ref _filteredPercentage, value, nameof(FilteredPercentage));
    }

    /// <summary>
    /// Recalculates the sum, average, minimum, and maximum values across the filtered items in the collection. It retrieves the values for each filtered item using the provided value selector function, computes the aggregates using the specified statistics computer, and updates the corresponding properties to reflect the new aggregate values. If there are no filtered items, it uses the empty aggregates defined by the statistics computer to set the properties accordingly.
    /// </summary>
    private void RecalculateValues()
    {
        var values = _collection.Items.Select(_valueSelector).ToList();
        var (sum, average, min, max) = values.Count == 0
            ? _computer.Empty
            : _computer.Compute(values);

        SetProperty(ref _sum, sum, nameof(Sum));
        SetProperty(ref _average, average, nameof(Average));
        SetProperty(ref _min, min, nameof(Min));
        SetProperty(ref _max, max, nameof(Max));
    }
}

/// <summary>
/// Defines an interface for computing statistics such as sum, average, minimum, and maximum values from a list of values. This interface provides a method to compute these statistics based on a given list of values and also defines an empty state for when there are no values to compute. Implementations of this interface can be used to calculate statistics for different types of values, such as numeric values or time durations, by providing specific logic for computing the aggregates based on the type of value being processed.
/// </summary>
/// <typeparam name="TValue">The type of value for which statistics are computed.</typeparam>
internal interface IStatisticsComputer<TValue>
{
    /// <summary>
    /// Gets the default statistics values for an empty list of values. This property returns a tuple containing the sum, average, minimum, and maximum values that should be used when there are no values to compute. The specific values returned by this property may depend on the type of value being processed and the logic defined in the implementation of the interface. For example, for numeric values, the sum might be zero, while for time durations, the sum might be a zero time span. This property allows for consistent handling of cases where there are no values to compute statistics from, ensuring that the properties of the collection statistics can be set to meaningful default values in such scenarios.
    /// </summary>
    (TValue Sum, TValue Average, TValue Min, TValue Max) Empty { get; }

    /// <summary>
    /// Computes the sum, average, minimum, and maximum values from the provided list of values. This method takes a read-only list of values as input and returns a tuple containing the computed sum, average, minimum, and maximum values based on the logic defined in the implementation of the interface. The specific calculations performed by this method will depend on the type of value being processed and the requirements for computing these statistics. For example, for numeric values, it might use standard mathematical operations to calculate the sum and average, while for time durations, it might use time-specific logic to compute these aggregates. This method is essential for updating the properties of the collection statistics whenever there are changes in the filtered items of the collection, allowing for accurate tracking of these metrics in real-time as the collection is modified.
    /// </summary>
    /// <param name="values">The list of values for which to compute statistics.</param>
    /// <returns>A tuple containing the computed sum, average, minimum, and maximum values.</returns>
    (TValue Sum, TValue Average, TValue Min, TValue Max) Compute(IReadOnlyList<TValue> values);
}

/// <summary>
/// Provides an implementation of the <see cref="IStatisticsComputer{TValue}"/> interface for computing statistics on double values. This class defines the logic for calculating the sum, average, minimum, and maximum values from a list of double values, as well as providing default values for these statistics when there are no values to compute. The implementation uses standard mathematical operations to compute these aggregates, making it suitable for tracking numeric metrics in the collection statistics. By using this class, you can easily compute and update statistics for double values in the context of an observable collection's filtered view.
/// </summary>
internal sealed class DoubleStatisticsComputer : IStatisticsComputer<double>
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="DoubleStatisticsComputer"/> class. This instance can be used to compute statistics for double values without needing to create multiple instances of the class, as the logic for computing these statistics is stateless and can be shared across different collections. By using this singleton instance, you can efficiently compute and update statistics for double values in various contexts within your application while minimizing resource usage and ensuring consistent behavior across different parts of the codebase that require these computations.
    /// </summary>
    public static DoubleStatisticsComputer Instance { get; } = new();

    /// <inheritdoc/>
    public (double Sum, double Average, double Min, double Max) Empty =>
        (0, double.NaN, double.NaN, double.NaN);

    /// <inheritdoc/>
    public (double Sum, double Average, double Min, double Max) Compute(IReadOnlyList<double> values) =>
        (values.Sum(), values.Average(), values.Min(), values.Max());
}

/// <summary>
/// Provides an implementation of the <see cref="IStatisticsComputer{TValue}"/> interface for computing statistics on <see cref="TimeSpan"/> values. This class defines the logic for calculating the sum, average, minimum, and maximum values from a list of <see cref="TimeSpan"/> values, as well as providing default values for these statistics when there are no values to compute. The implementation uses time-specific logic to compute these aggregates, making it suitable for tracking duration metrics in the collection statistics. By using this class, you can easily compute and update statistics for <see cref="TimeSpan"/> values in the context of an observable collection's filtered view.
/// </summary>
internal sealed class TimeSpanStatisticsComputer : IStatisticsComputer<TimeSpan>
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="TimeSpanStatisticsComputer"/> class. This instance can be used to compute statistics for <see cref="TimeSpan"/> values without needing to create multiple instances of the class, as the logic for computing these statistics is stateless and can be shared across different collections. By using this singleton instance, you can efficiently compute and update statistics for <see cref="TimeSpan"/> values in various contexts within your application while minimizing resource usage and ensuring consistent behavior across different parts of the codebase that require these computations.
    /// </summary>
    public static TimeSpanStatisticsComputer Instance { get; } = new();

    /// <inheritdoc/>
    public (TimeSpan Sum, TimeSpan Average, TimeSpan Min, TimeSpan Max) Empty =>
        (TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);

    /// <inheritdoc/>
    public (TimeSpan Sum, TimeSpan Average, TimeSpan Min, TimeSpan Max) Compute(IReadOnlyList<TimeSpan> values)
    {
        var ticks = values.Select(v => v.Ticks).ToList();
        return (
            new(ticks.Sum()),
            new((long)ticks.Average()),
            new(ticks.Min()),
            new(ticks.Max()));
    }
}
