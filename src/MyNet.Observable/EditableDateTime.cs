// -----------------------------------------------------------------------
// <copyright file="EditableDateTime.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Observable.Attributes;
using MyNet.Observable.Resources;
using MyNet.Utilities;
using MyNet.Utilities.Deferring;
using MyNet.Utilities.Localization;
using PropertyChanged;

namespace MyNet.Observable;

public class EditableDateTime : EditableObject
{
    private readonly DeferredAction _dateTimeChangedDeferrer;
    private TimeZoneInfo _currentTimeZone = GlobalizationService.Current.TimeZone;

    public EditableDateTime(bool isRequired = true)
    {
        _dateTimeChangedDeferrer = new(() => OnPropertyChanged(nameof(DateTime)));

        if (!isRequired)
            return;

        ValidationRules.Add<EditableDateTime, DateOnly?>(x => x.Date, () => ValidationResources.FieldXIsRequiredError.FormatWith(CultureInfo.CurrentCulture, nameof(Date).Translate()), x => x is not null);
        ValidationRules.Add<EditableDateTime, TimeOnly?>(x => x.Time, () => ValidationResources.FieldXIsRequiredError.FormatWith(CultureInfo.CurrentCulture, nameof(Time).Translate()), x => x is not null);
    }

    public DateOnly? Date { get; set; }

    public TimeOnly? Time { get; set; }

    [IgnoreModificationTracking(false)]
    [CanBeValidated(false)]
    [AlsoNotifyFor(nameof(Date), nameof(Time))]
    public bool HasValue => Date.HasValue && Time.HasValue;

    [IgnoreModificationTracking(false)]
    [CanBeValidated(false)]
    public DateTime? DateTime => Date.HasValue && Time.HasValue ? Date.Value.At(Time.Value) : null;

    public DateTime? ToUtc() => DateTime.HasValue ? GlobalizationService.Current.ConvertToUtc(DateTime.Value) : null;

    public DateTime? ToLocal() => DateTime.HasValue ? GlobalizationService.Current.ConvertToTimeZone(DateTime.Value, TimeZoneInfo.Local) : null;

    public DateTime? ToTimeZone(TimeZoneInfo timezone) => DateTime.HasValue ? GlobalizationService.Current.ConvertToTimeZone(DateTime.Value, timezone) : null;

    public DateTime ToUtcOrDefault(DateTime defaultValue = default) => ToUtc() ?? defaultValue;

    public DateTime ToLocalOrDefault(DateTime defaultValue = default) => ToLocal() ?? defaultValue;

    public void Load(DateTime dateTime)
    {
        var date = GlobalizationService.Current.Convert(dateTime);

        using (_dateTimeChangedDeferrer.Defer())
        {
            Date = date.Date.ToDate();
            Time = date.ToTime();
        }
    }

    public void Clear()
    {
        using (_dateTimeChangedDeferrer.Defer())
        {
            Date = null;
            Time = null;
        }
    }

    [SuppressPropertyChangedWarnings]
    protected override void OnTimeZoneChanged()
    {
        base.OnTimeZoneChanged();

        if (DateTime.HasValue)
        {
            var date = TimeZoneInfo.ConvertTime(DateTime.Value, _currentTimeZone, GlobalizationService.Current.TimeZone);
            using (_dateTimeChangedDeferrer.Defer())
            {
                Date = date.ToDate();
                Time = date.ToTime();
            }
        }

        _currentTimeZone = GlobalizationService.Current.TimeZone;
    }

    protected virtual void OnDateChanged() => _dateTimeChangedDeferrer.IsDeferred.IfFalse(() => OnPropertyChanged(nameof(DateTime)));

    protected virtual void OnTimeChanged() => _dateTimeChangedDeferrer.IsDeferred.IfFalse(() => OnPropertyChanged(nameof(DateTime)));

    public override string? ToString() => Date?.At(Time ?? TimeOnly.MinValue).ToString(CultureInfo.CurrentCulture);
}
