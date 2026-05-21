// -----------------------------------------------------------------------
// <copyright file="DateOnlyExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace MyNet.Utilities.Tests.Temporal;

[Collection("UseCultureSequential")]
public class DateOnlyExtensionsTests
{
    #region Navigation: NextDay / PreviousDay / NextYear / PreviousYear

    [Fact]
    public void NextDay_ReturnsFollowingDay()
    {
        var date = new DateOnly(2024, 3, 15);
        Assert.Equal(new(2024, 3, 16), date.NextDay());
    }

    [Fact]
    public void PreviousDay_ReturnsPrecedingDay()
    {
        var date = new DateOnly(2024, 3, 15);
        Assert.Equal(new(2024, 3, 14), date.PreviousDay());
    }

    [Fact]
    public void NextYear_ReturnsSameDayNextYear()
    {
        var date = new DateOnly(2023, 6, 15);
        Assert.Equal(new(2024, 6, 15), date.NextYear());
    }

    [Fact]
    public void NextYear_HandlesLeapYearEdge()
    {
        // Feb 29 in a leap year -> Feb moves forward
        var date = new DateOnly(2024, 2, 29);
        var result = date.NextYear();

        // 2025 has no Feb 29, so overflow takes it to Mar 1
        Assert.True(result > new DateOnly(2025, 2, 28));
    }

    [Fact]
    public void PreviousYear_ReturnsSameDayPreviousYear()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2023, 6, 15), date.PreviousYear());
    }

    #endregion

    #region Next / Previous DayOfWeek

    [Fact]
    public void Next_DayOfWeek_ReturnsNextOccurrence()
    {
        var date = new DateOnly(2024, 8, 19); // Monday
        var result = date.Next(DayOfWeek.Friday);
        Assert.Equal(DayOfWeek.Friday, result.DayOfWeek);
        Assert.True(result > date);
    }

    [Fact]
    public void Previous_DayOfWeek_ReturnsPreviousOccurrence()
    {
        var date = new DateOnly(2024, 8, 19); // Monday
        var result = date.Previous(DayOfWeek.Friday);
        Assert.Equal(DayOfWeek.Friday, result.DayOfWeek);
        Assert.True(result < date);
    }

    [Fact]
    public void Next_DayMonth_ReturnsNextOccurrenceOfDayMonth()
    {
        var date = new DateOnly(2024, 8, 19);
        var result = date.Next(1, 9); // next Sep 1
        Assert.Equal(new(2024, 9, 1), result);
    }

    [Fact]
    public void Previous_DayMonth_ReturnsPreviousOccurrenceOfDayMonth()
    {
        var date = new DateOnly(2024, 8, 19);
        var result = date.Previous(1, 8); // previous Aug 1
        Assert.Equal(new(2024, 8, 1), result);
    }

    #endregion

    #region WeekAfter / WeekEarlier

    [Fact]
    public void WeekAfter_AddsSeven_Days()
    {
        var date = new DateOnly(2024, 8, 1);
        Assert.Equal(new(2024, 8, 8), date.WeekAfter());
    }

    [Fact]
    public void WeekEarlier_SubtractsSeven_Days()
    {
        var date = new DateOnly(2024, 8, 8);
        Assert.Equal(new(2024, 8, 1), date.WeekEarlier());
    }

    #endregion

    #region SetYear / SetMonth / SetDay

    [Fact]
    public void SetYear_ChangesYearComponent()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2030, 6, 15), date.SetYear(2030));
    }

    [Fact]
    public void SetMonth_ChangesMonthComponent()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2024, 12, 15), date.SetMonth(12));
    }

    [Fact]
    public void SetDay_ChangesDayComponent()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2024, 6, 1), date.SetDay(1));
    }

    #endregion

    #region BeginningOfXxx / EndOfXxx

    [Fact]
    public void BeginningOfMonth_ReturnsFirstDay()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2024, 6, 1), date.BeginningOfMonth());
    }

    [Fact]
    public void EndOfMonth_ReturnsLastDay()
    {
        var date = new DateOnly(2024, 2, 5); // Feb in leap year 2024
        Assert.Equal(new(2024, 2, 29), date.EndOfMonth());
    }

    [Fact]
    public void BeginningOfYear_ReturnsJanuary1()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2024, 1, 1), date.BeginningOfYear());
    }

    [Fact]
    public void EndOfYear_ReturnsDecember31()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2024, 12, 31), date.EndOfYear());
    }

    [Fact]
    public void BeginningOfDecade_ReturnsFirstYearOfDecade()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2020, 1, 1), date.BeginningOfDecade());
    }

    [Fact]
    public void EndOfDecade_ReturnsLastYearOfDecade()
    {
        var date = new DateOnly(2024, 6, 15);
        Assert.Equal(new(2029, 12, 31), date.EndOfDecade());
    }

    [Fact]
    public void BeginningOfQuarter_ReturnsFirstDayOfQuarter()
    {
        var date = new DateOnly(2024, 11, 15); // Q4
        Assert.Equal(new(2024, 10, 1), date.BeginningOfQuarter());
    }

    [Fact]
    public void BeginningOfWeek_WithMondayFirstDay_ReturnsPreviousMonday()
    {
        using var culture = new CultureScope("fr-FR");
        var date = new DateOnly(2024, 8, 22); // Thursday
        var result = date.BeginningOfWeek();
        Assert.Equal(DayOfWeek.Monday, result.DayOfWeek);
        Assert.True(result <= date);
    }

    [Fact]
    public void EndOfWeek_ReturnsSixDaysAfterBeginningOfWeek()
    {
        using var culture = new CultureScope("fr-FR");
        var date = new DateOnly(2024, 8, 22); // Thursday
        var result = date.EndOfWeek();
        Assert.Equal(date.BeginningOfWeek().AddDays(6), result);
    }

    #endregion

    #region IsXxx predicates

    [Fact]
    public void IsWeekend_ReturnsTrueForSaturday()
        => Assert.True(new DateOnly(2024, 8, 24).IsWeekend()); // Saturday

    [Fact]
    public void IsWeekend_ReturnsFalseForMonday()
        => Assert.False(new DateOnly(2024, 8, 19).IsWeekend()); // Monday

    [Fact]
    public void SameMonth_ReturnsTrueForSameYearMonth()
    {
        var d1 = new DateOnly(2024, 8, 1);
        var d2 = new DateOnly(2024, 8, 31);
        Assert.True(d1.SameMonth(d2));
    }

    [Fact]
    public void SameMonth_ReturnsFalseForDifferentMonth()
    {
        var d1 = new DateOnly(2024, 8, 1);
        var d2 = new DateOnly(2024, 9, 1);
        Assert.False(d1.SameMonth(d2));
    }

    [Fact]
    public void SameYear_ReturnsTrueForSameYear()
    {
        var d1 = new DateOnly(2024, 1, 1);
        var d2 = new DateOnly(2024, 12, 31);
        Assert.True(d1.SameYear(d2));
    }

    [Fact]
    public void SameDecade_ReturnsTrueWhenSameDecade()
    {
        var d1 = new DateOnly(2021, 1, 1);
        var d2 = new DateOnly(2029, 12, 31);
        Assert.True(d1.SameDecade(d2));
    }

    [Fact]
    public void SameDecade_ReturnsFalseAcrossDecades()
    {
        var d1 = new DateOnly(2019, 12, 31);
        var d2 = new DateOnly(2020, 1, 1);
        Assert.False(d1.SameDecade(d2));
    }

    #endregion

    #region At

    [Fact]
    public void At_TimeOnly_CombinesDateAndTime()
    {
        var date = new DateOnly(2024, 8, 22);
        var time = new TimeOnly(14, 30, 0);
        var result = date.At(time);
        Assert.Equal(new(2024, 8, 22, 14, 30, 0), result);
    }

    [Fact]
    public void At_HourMinute_CombinesDateAndTime()
    {
        var date = new DateOnly(2024, 8, 22);
        var result = date.At(14, 30);
        Assert.Equal(new(2024, 8, 22, 14, 30, 0), result);
    }

    [Fact]
    public void BeginningOfDay_ReturnsMidnight()
    {
        var date = new DateOnly(2024, 8, 22);
        var result = date.BeginningOfDay();
        Assert.Equal(new(2024, 8, 22, 0, 0, 0), result);
    }

    [Fact]
    public void EndOfDay_ReturnsEndOfMidnight()
    {
        var date = new DateOnly(2024, 8, 22);
        var result = date.EndOfDay();

        // EndOfDay returns At(TimeOnly.MaxValue) = 23:59:59.9999999
        var expected = new DateOnly(2024, 8, 22).ToDateTime(TimeOnly.MaxValue);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Business days

    [Fact]
    public void AddBusinessDays_SkipsWeekend()
    {
        var date = new DateOnly(2024, 8, 23); // Friday
        var result = date.AddBusinessDays(1);
        Assert.Equal(new(2024, 8, 26), result); // Monday
    }

    [Fact]
    public void SubtractBusinessDays_SkipsWeekend()
    {
        var date = new DateOnly(2024, 8, 26); // Monday
        var result = date.SubtractBusinessDays(1);
        Assert.Equal(new(2024, 8, 23), result); // Friday
    }

    #endregion

    #region NumberOfXxx / CompareXxx

    [Fact]
    public void NumberOfDays_ReturnsAbsoluteDays()
    {
        var d1 = new DateOnly(2024, 1, 1);
        var d2 = new DateOnly(2024, 1, 11);
        Assert.Equal(10, d1.NumberOfDays(d2));
        Assert.Equal(10, d2.NumberOfDays(d1));
    }

    [Fact]
    public void NumberOfWeeks_ReturnsAbsoluteWeeks()
    {
        var d1 = new DateOnly(2024, 1, 1);
        var d2 = new DateOnly(2024, 1, 22);
        Assert.Equal(3, d1.NumberOfWeeks(d2));
    }

    [Fact]
    public void NumberOfMonths_ReturnsAbsoluteMonths()
    {
        var d1 = new DateOnly(2024, 1, 1);
        var d2 = new DateTime(2024, 4, 1);
        Assert.Equal(3, d1.NumberOfMonths(d2));
    }

    [Fact]
    public void CompareMonth_ReturnsSignedDifference()
    {
        var d1 = new DateOnly(2024, 1, 1);
        var d2 = new DateTime(2024, 4, 1);
        Assert.Equal(3, d1.CompareMonth(d2));
        Assert.Equal(-3, new DateOnly(2024, 4, 1).CompareMonth(new(2024, 1, 1)));
    }

    [Fact]
    public void NumberOfYears_ReturnsAbsoluteYears()
    {
        var d1 = new DateOnly(2020, 1, 1);
        var d2 = new DateTime(2024, 1, 1);
        Assert.Equal(4, d1.NumberOfYears(d2));
    }

    #endregion

    #region PreviousMonth / NextMonth

    [Fact]
    public void PreviousMonth_ReturnsCorrectDate()
    {
        var date = new DateOnly(2024, 3, 15);
        Assert.Equal(new(2024, 2, 15), date.PreviousMonth());
    }

    [Fact]
    public void PreviousMonth_JanuaryGoesToDecemberPreviousYear()
    {
        var date = new DateOnly(2024, 1, 15);
        Assert.Equal(new(2023, 12, 15), date.PreviousMonth());
    }

    [Fact]
    public void PreviousMonth_ClampsToLastDayOfPreviousMonth()
    {
        var date = new DateOnly(2024, 3, 31);
        var result = date.PreviousMonth(); // Feb has 29 days in 2024
        Assert.Equal(new(2024, 2, 29), result);
    }

    [Fact]
    public void NextMonth_ReturnsCorrectDate()
    {
        var date = new DateOnly(2024, 3, 15);
        Assert.Equal(new(2024, 4, 15), date.NextMonth());
    }

    [Fact]
    public void NextMonth_DecemberGoesToJanuaryNextYear()
    {
        var date = new DateOnly(2024, 12, 15);
        Assert.Equal(new(2025, 1, 15), date.NextMonth());
    }

    [Fact]
    public void NextMonth_ClampsToLastDayOfNextMonth()
    {
        var date = new DateOnly(2024, 1, 31);
        var result = date.NextMonth(); // Feb has 29 days in 2024
        Assert.Equal(new(2024, 2, 29), result);
    }

    #endregion

    #region Range

    [Fact]
    public void Range_ByDay_ReturnsExpectedDates()
    {
        var start = new DateOnly(2024, 8, 1);
        var end = new DateOnly(2024, 8, 5);
        var result = start.Range(end).ToList();
        Assert.Equal(5, result.Count);
        Assert.Equal(new(2024, 8, 1), result[0]);
        Assert.Equal(new(2024, 8, 5), result[4]);
    }

    [Fact]
    public void Range_ByWeek_ReturnsExpectedDates()
    {
        var start = new DateOnly(2024, 8, 1);
        var end = new DateOnly(2024, 8, 22);
        var result = start.Range(end, 1, TimeUnit.Week).ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Range_ByMonth_ReturnsExpectedCount()
    {
        var start = new DateOnly(2024, 1, 1);
        var end = new DateOnly(2024, 4, 1);
        var result = start.Range(end, 1, TimeUnit.Month).ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Range_ByYear_ReturnsExpectedCount()
    {
        var start = new DateOnly(2020, 1, 1);
        var end = new DateOnly(2024, 1, 1);
        var result = start.Range(end, 1, TimeUnit.Year).ToList();
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Range_WithZeroStep_ThrowsArgumentOutOfRangeException()
    {
        var start = new DateOnly(2024, 1, 1);
        var end = new DateOnly(2024, 12, 31);
        Assert.Throws<ArgumentOutOfRangeException>(() => start.Range(end, 0).ToList());
    }

    [Fact]
    public void Range_WithHourUnit_ThrowsInvalidOperationException()
    {
        var start = new DateOnly(2024, 1, 1);
        var end = new DateOnly(2024, 12, 31);
        Assert.Throws<InvalidOperationException>(() => start.Range(end, 1, TimeUnit.Hour).ToList());
    }

    [Fact]
    public void Range_WithNegativeStep_ReturnsDescendingSequence()
    {
        var start = new DateOnly(2024, 8, 5);
        var end = new DateOnly(2024, 8, 1);
        var result = start.Range(end, -1).ToList();
        Assert.Equal(5, result.Count);
        Assert.Equal(new(2024, 8, 5), result[0]);
        Assert.Equal(new(2024, 8, 1), result[4]);
    }

    #endregion

    // ── helper ─────────────────────────────────────────────────────────────
    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _prev;

        public CultureScope(string name)
        {
            _prev = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new(name);
        }

        public void Dispose() => CultureInfo.CurrentCulture = _prev;
    }
}
