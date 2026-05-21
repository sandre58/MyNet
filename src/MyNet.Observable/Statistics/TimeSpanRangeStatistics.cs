// -----------------------------------------------------------------------
// <copyright file="TimeSpanRangeStatistics.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using DynamicData.Binding;

namespace MyNet.Observable.Statistics;

public class TimeSpanRangeStatistics<T> : ObservableObject
    where T : notnull
{
    public TimeSpanRangeStatistics() { }

    public TimeSpanRangeStatistics(ObservableCollection<T> list, Func<T, bool> filterPredicate, Func<T, TimeSpan> valuePredicate, Func<T, IObservable<object>>? reevaluator = null)
    {
        var obs = list.ToObservableChangeSet();

        if (reevaluator is not null)
            obs = obs.AutoRefreshOnObservable(reevaluator);

        Disposables.Add(obs.Subscribe(_ => Update(list, filterPredicate, valuePredicate)));
    }

    public void Update(IEnumerable<T> list, Func<T, bool> filterPredicate, Func<T, TimeSpan> valuePredicate)
    {
        var items = list.Where(filterPredicate).ToList();
        Average = items.Count != 0 ? new((long)items.Average(x => valuePredicate.Invoke(x).Ticks)) : TimeSpan.Zero;
        Sum = items.Count != 0 ? new(items.Sum(x => valuePredicate.Invoke(x).Ticks)) : TimeSpan.Zero;
        Min = items.Count != 0 ? new(items.Min(x => valuePredicate.Invoke(x).Ticks)) : TimeSpan.Zero;
        Max = items.Count != 0 ? new(items.Max(x => valuePredicate.Invoke(x).Ticks)) : TimeSpan.Zero;
    }

    public TimeSpan Average { get; private set; }

    public TimeSpan Sum { get; private set; }

    public TimeSpan Min { get; private set; }

    public TimeSpan Max { get; private set; }
}
