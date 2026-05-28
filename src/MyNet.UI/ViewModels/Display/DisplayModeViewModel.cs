// -----------------------------------------------------------------------
// <copyright file="DisplayModeViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using MyNet.Collections;
using MyNet.Observable;
using MyNet.Primitives;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Display.Options;

namespace MyNet.UI.ViewModels.Display;

/// <summary>
/// Provides a reusable base implementation for display mode view models.
/// </summary>
public abstract class DisplayModeViewModel : ObservableObject, IDisplayModeViewModel
{
    /// <summary>Legacy-compatible key for grid mode.</summary>
    public const string GridKey = "DisplayModeGrid";

    /// <summary>Legacy-compatible key for detailed mode.</summary>
    public const string DetailedKey = "DisplayModeDetailled";

    /// <summary>Legacy-compatible key for chart mode.</summary>
    public const string ChartKey = "DisplayModeChart";

    /// <summary>Legacy-compatible key for list mode.</summary>
    public const string ListKey = "DisplayModeList";

    /// <summary>Legacy-compatible key for hour mode.</summary>
    public const string HourKey = "DisplayModeHour";

    /// <summary>Legacy-compatible key for day mode.</summary>
    public const string DayKey = "DisplayModeDay";

    /// <summary>Legacy-compatible key for week mode.</summary>
    public const string WeekKey = "DisplayModeWeek";

    /// <summary>Legacy-compatible key for month mode.</summary>
    public const string MonthKey = "DisplayModeMonth";

    /// <summary>Legacy-compatible key for year mode.</summary>
    public const string YearKey = "DisplayModeYear";

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayModeViewModel"/> class.
    /// </summary>
    /// <param name="key">The display mode key.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    protected DisplayModeViewModel(string key, ICommandFactory? commandFactory = null)
    {
        Key = key;
        var commands = commandFactory ?? RelayCommandFactory.Default;
        ResetCommand = commands.Create(Reset);
    }

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public virtual bool OverrideEmptySourceTemplate => false;

    /// <inheritdoc />
    public virtual bool OverrideEmptyItemsTemplate => false;

    /// <summary>
    /// Gets the command that resets this mode options.
    /// </summary>
    public ICommand ResetCommand { get; }

    /// <inheritdoc />
    public virtual void Reset()
    {
    }
}

/// <summary>
/// Represents a grid display mode.
/// </summary>
public sealed class GridDisplayModeViewModel : DisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridDisplayModeViewModel"/> class.
    /// </summary>
    public GridDisplayModeViewModel()
        : base(GridKey)
    {
    }
}

/// <summary>
/// Represents a detailed display mode.
/// </summary>
public sealed class DetailedDisplayModeViewModel : DisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DetailedDisplayModeViewModel"/> class.
    /// </summary>
    public DetailedDisplayModeViewModel()
        : base(DetailedKey)
    {
    }
}

/// <summary>
/// Represents a chart display mode.
/// </summary>
public sealed class ChartDisplayModeViewModel : DisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartDisplayModeViewModel"/> class.
    /// </summary>
    public ChartDisplayModeViewModel()
        : base(ChartKey)
    {
    }
}

/// <summary>
/// Represents a list display mode with column options.
/// </summary>
public sealed class ListDisplayModeViewModel : DisplayModeViewModel, IColumnOptionsDisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListDisplayModeViewModel"/> class.
    /// </summary>
    /// <param name="defaultColumns">The default displayed columns.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    public ListDisplayModeViewModel(string[]? defaultColumns = null, ICommandFactory? commandFactory = null)
        : base(ListKey, commandFactory)
    {
        ColumnOptions = new(defaultColumns);
        var commands = commandFactory ?? RelayCommandFactory.Default;
        SetDisplayedColumnsCommand = commands.Create<IEnumerable<string>>(x => SetDisplayedColumns(x ?? []));
    }

    /// <inheritdoc />
    public override bool OverrideEmptyItemsTemplate => true;

    /// <inheritdoc />
    public ColumnOptionsViewModel ColumnOptions { get; }

    /// <inheritdoc />
    public ObservableRangeCollection<LabeledValue<string[]>> PresetColumns { get; } = [];

    /// <inheritdoc />
    public ICommand SetDisplayedColumnsCommand { get; }

    /// <inheritdoc />
    public void SetDisplayedColumns(IEnumerable<string> columns) => ColumnOptions.SetDisplayedColumns(columns);

    /// <inheritdoc />
    public override void Reset() => ColumnOptions.Reset();
}

/// <summary>
/// Provides a reusable base implementation for calendar-like display modes.
/// </summary>
public class CalendarDisplayModeViewModel : DisplayModeViewModel, IDateNavigableDisplayModeViewModel
{
    private readonly TimeUnit _unit;
    private readonly int _changeValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalendarDisplayModeViewModel"/> class.
    /// </summary>
    /// <param name="timeUnit">The navigation time unit.</param>
    /// <param name="changeValue">The navigation step value.</param>
    /// <param name="key">The display mode key.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    protected CalendarDisplayModeViewModel(TimeUnit timeUnit, int changeValue, string key, ICommandFactory? commandFactory = null)
        : base(key, commandFactory)
    {
        _unit = timeUnit;
        _changeValue = changeValue;

        DisplayDate = DateTime.Now;

        var commands = commandFactory ?? RelayCommandFactory.Default;
        MoveToPreviousDateCommand = commands.Create(MoveToPreviousDate);
        MoveToNextDateCommand = commands.Create(MoveToNextDate);
        MoveToTodayCommand = commands.Create(MoveToToday);
    }

    /// <inheritdoc />
    public override bool OverrideEmptySourceTemplate => true;

    /// <inheritdoc />
    public override bool OverrideEmptyItemsTemplate => true;

    /// <inheritdoc />
    public DateTime DisplayDate { get; protected set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the first day of week for calendar display.
    /// </summary>
    public virtual DayOfWeek FirstDayOfWeek => CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

    /// <inheritdoc />
    public override void Reset() => MoveToToday();

    /// <inheritdoc />
    public ICommand MoveToPreviousDateCommand { get; }

    /// <inheritdoc />
    public ICommand MoveToNextDateCommand { get; }

    /// <inheritdoc />
    public ICommand MoveToTodayCommand { get; }

    /// <summary>
    /// Moves the displayed date to today.
    /// </summary>
    public virtual void MoveToToday() => DisplayDate = DateTime.Now;

    /// <summary>
    /// Moves the displayed date to the next date range.
    /// </summary>
    public virtual void MoveToNextDate() => DisplayDate = GetNextDate(DisplayDate);

    /// <summary>
    /// Moves the displayed date to the previous date range.
    /// </summary>
    public virtual void MoveToPreviousDate() => DisplayDate = GetPreviousDate(DisplayDate);

    /// <summary>
    /// Computes the next display date.
    /// </summary>
    protected virtual DateTime GetNextDate(DateTime date) => date.Add(_changeValue, _unit);

    /// <summary>
    /// Computes the previous display date.
    /// </summary>
    protected virtual DateTime GetPreviousDate(DateTime date) => date.Add(-_changeValue, _unit);
}

/// <summary>
/// Represents an hour-based calendar display mode.
/// </summary>
public sealed class HourDisplayModeViewModel : CalendarDisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HourDisplayModeViewModel"/> class.
    /// </summary>
    public HourDisplayModeViewModel(ICommandFactory? commandFactory = null)
        : base(TimeUnit.Hour, 1, HourKey, commandFactory)
    {
    }
}

/// <summary>
/// Represents a day-based calendar display mode.
/// </summary>
public sealed class DayDisplayModeViewModel : CalendarDisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DayDisplayModeViewModel"/> class.
    /// </summary>
    /// <param name="displayDaysCount">The number of displayed days.</param>
    /// <param name="displayTimeStart">Optional displayed time start.</param>
    /// <param name="displayTimeEnd">Optional displayed time end.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    public DayDisplayModeViewModel(
        int displayDaysCount = 1,
        TimeSpan? displayTimeStart = null,
        TimeSpan? displayTimeEnd = null,
        ICommandFactory? commandFactory = null)
        : base(TimeUnit.Day, 1, DayKey, commandFactory)
    {
        DisplayDaysCount = displayDaysCount;

        if (displayTimeStart.HasValue)
            DisplayTimeStart = displayTimeStart.Value;

        if (displayTimeEnd.HasValue)
            DisplayTimeEnd = displayTimeEnd.Value;
    }

    /// <summary>
    /// Gets or sets the number of displayed days.
    /// </summary>
    public int DisplayDaysCount { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the displayed time start.
    /// </summary>
    public TimeSpan DisplayTimeStart { get; set => SetProperty(ref field, value); } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the displayed time end.
    /// </summary>
    public TimeSpan DisplayTimeEnd { get; set => SetProperty(ref field, value); } = TimeSpan.FromHours(24);

    /// <inheritdoc />
    protected override DateTime GetNextDate(DateTime date) => date.Add(DisplayDaysCount, TimeUnit.Day);

    /// <inheritdoc />
    protected override DateTime GetPreviousDate(DateTime date) => date.Add(-DisplayDaysCount, TimeUnit.Day);
}

/// <summary>
/// Represents a week-based calendar display mode.
/// </summary>
public sealed class WeekDisplayModeViewModel : CalendarDisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeekDisplayModeViewModel"/> class.
    /// </summary>
    public WeekDisplayModeViewModel(ICommandFactory? commandFactory = null)
        : base(TimeUnit.Week, 1, WeekKey, commandFactory)
    {
    }
}

/// <summary>
/// Represents a month-based calendar display mode.
/// </summary>
public sealed class MonthDisplayModeViewModel : CalendarDisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MonthDisplayModeViewModel"/> class.
    /// </summary>
    public MonthDisplayModeViewModel(ICommandFactory? commandFactory = null)
        : base(TimeUnit.Month, 1, MonthKey, commandFactory)
    {
    }
}

/// <summary>
/// Represents a year-based calendar display mode.
/// </summary>
public sealed class YearDisplayModeViewModel : CalendarDisplayModeViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YearDisplayModeViewModel"/> class.
    /// </summary>
    public YearDisplayModeViewModel(ICommandFactory? commandFactory = null)
        : base(TimeUnit.Year, 1, YearKey, commandFactory)
    {
    }
}
