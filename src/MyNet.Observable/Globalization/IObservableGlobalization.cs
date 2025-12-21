// -----------------------------------------------------------------------
// <copyright file="IObservableGlobalization.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace MyNet.Observable.Globalization;

public interface IObservableGlobalization : INotifyPropertyChanged
{
    CultureInfo Culture { get; }

    CultureInfo UICulture { get; }

    TimeZoneInfo TimeZone { get; }

    DateTime Now { get; }

    DateTime UtcNow { get; }

    ObservableCollection<CultureInfo> SupportedCultures { get; }

    ObservableCollection<TimeZoneInfo> SupportedTimeZones { get; }

    void SetCulture(string culture);

    void SetTimeZone(TimeZoneInfo timeZone);
}
