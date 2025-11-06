// -----------------------------------------------------------------------
// <copyright file="DateTimeHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Units;
using Xunit;

namespace MyNet.Utilities.Tests.Helpers;

[Collection("UseCultureSequential")]
public class DateTimeHelperTests
{
    #region GetCurrentDateTimeFormatInfo

    [Fact]
    [UseCulture("en-US")]
    public void GetCurrentDateTimeFormatInfo_WithGregorianCalendar_ReturnsCurrentCultureFormat()
    {
        // Arrange & Act
        var result = DateTimeHelper.GetCurrentDateTimeFormatInfo();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GregorianCalendar>(result.Calendar);
    }

    [Fact]
    [UseCulture("ar-SA")] // Arabic uses Hijri calendar by default
    public void GetCurrentDateTimeFormatInfo_WithNonGregorianCalendar_ReturnsGregorianCalendarFormat()
    {
        // Arrange & Act
        var result = DateTimeHelper.GetCurrentDateTimeFormatInfo();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GregorianCalendar>(result.Calendar);
    }

    #endregion

    #region Max - DateTime

    [Fact]
    public void Max_DateTime_ReturnsLaterDate()
    {
        // Arrange
        var date1 = new DateTime(2024, 1, 1);
        var date2 = new DateTime(2024, 12, 31);

        // Act
        var result = DateTimeHelper.Max(date1, date2);

        // Assert
        Assert.Equal(date2, result);
    }

    [Fact]
    public void Max_DateTime_WithEqualDates_ReturnsEitherDate()
    {
        // Arrange
        var date1 = new DateTime(2024, 6, 15);
        var date2 = new DateTime(2024, 6, 15);

        // Act
        var result = DateTimeHelper.Max(date1, date2);

        // Assert
        Assert.Equal(date1, result);
    }

    [Fact]
    public void Max_DateTime_WithFirstDateLater_ReturnsFirstDate()
    {
        // Arrange
        var date1 = new DateTime(2025, 1, 1);
        var date2 = new DateTime(2024, 1, 1);

        // Act
        var result = DateTimeHelper.Max(date1, date2);

        // Assert
        Assert.Equal(date1, result);
    }

    #endregion

    #region Max - DateOnly

    [Fact]
    public void Max_DateOnly_ReturnsLaterDate()
    {
        // Arrange
        var date1 = new DateOnly(2024, 1, 1);
        var date2 = new DateOnly(2024, 12, 31);

        // Act
        var result = DateTimeHelper.Max(date1, date2);

        // Assert
        Assert.Equal(date2, result);
    }

    [Fact]
    public void Max_DateOnly_WithEqualDates_ReturnsEitherDate()
    {
        // Arrange
        var date1 = new DateOnly(2024, 6, 15);
        var date2 = new DateOnly(2024, 6, 15);

        // Act
        var result = DateTimeHelper.Max(date1, date2);

        // Assert
        Assert.Equal(date1, result);
    }

    #endregion

    #region Max - TimeOnly

    [Fact]
    public void Max_TimeOnly_ReturnsLaterTime()
    {
        // Arrange
        var time1 = new TimeOnly(10, 30);
        var time2 = new TimeOnly(15, 45);

        // Act
        var result = DateTimeHelper.Max(time1, time2);

        // Assert
        Assert.Equal(time2, result);
    }

    [Fact]
    public void Max_TimeOnly_WithEqualTimes_ReturnsEitherTime()
    {
        // Arrange
        var time1 = new TimeOnly(12, 0);
        var time2 = new TimeOnly(12, 0);

        // Act
        var result = DateTimeHelper.Max(time1, time2);

        // Assert
        Assert.Equal(time1, result);
    }

    #endregion

    #region Max - TimeSpan

    [Fact]
    public void Max_TimeSpan_ReturnsLongerTimeSpan()
    {
        // Arrange
        var timeSpan1 = TimeSpan.FromHours(2);
        var timeSpan2 = TimeSpan.FromHours(5);

        // Act
        var result = DateTimeHelper.Max(timeSpan1, timeSpan2);

        // Assert
        Assert.Equal(timeSpan2, result);
    }

    [Fact]
    public void Max_TimeSpan_WithNegativeValues_ReturnsGreaterValue()
    {
        // Arrange
        var timeSpan1 = TimeSpan.FromHours(-5);
        var timeSpan2 = TimeSpan.FromHours(-2);

        // Act
        var result = DateTimeHelper.Max(timeSpan1, timeSpan2);

        // Assert
        Assert.Equal(timeSpan2, result);
    }

    #endregion

    #region Min - DateTime

    [Fact]
    public void Min_DateTime_ReturnsEarlierDate()
    {
        // Arrange
        var date1 = new DateTime(2024, 1, 1);
        var date2 = new DateTime(2024, 12, 31);

        // Act
        var result = DateTimeHelper.Min(date1, date2);

        // Assert
        Assert.Equal(date1, result);
    }

    [Fact]
    public void Min_DateTime_WithEqualDates_ReturnsEitherDate()
    {
        // Arrange
        var date1 = new DateTime(2024, 6, 15);
        var date2 = new DateTime(2024, 6, 15);

        // Act
        var result = DateTimeHelper.Min(date1, date2);

        // Assert
        Assert.Equal(date1, result);
    }

    #endregion

    #region Min - DateOnly

    [Fact]
    public void Min_DateOnly_ReturnsEarlierDate()
    {
        // Arrange
        var date1 = new DateOnly(2024, 1, 1);
        var date2 = new DateOnly(2024, 12, 31);

        // Act
        var result = DateTimeHelper.Min(date1, date2);

        // Assert
        Assert.Equal(date1, result);
    }

    #endregion

    #region Min - TimeOnly

    [Fact]
    public void Min_TimeOnly_ReturnsEarlierTime()
    {
        // Arrange
        var time1 = new TimeOnly(10, 30);
        var time2 = new TimeOnly(15, 45);

        // Act
        var result = DateTimeHelper.Min(time1, time2);

        // Assert
        Assert.Equal(time1, result);
    }

    #endregion

    #region Min - TimeSpan

    [Fact]
    public void Min_TimeSpan_ReturnsShorterTimeSpan()
    {
        // Arrange
        var timeSpan1 = TimeSpan.FromHours(2);
        var timeSpan2 = TimeSpan.FromHours(5);

        // Act
        var result = DateTimeHelper.Min(timeSpan1, timeSpan2);

        // Assert
        Assert.Equal(timeSpan1, result);
    }

    [Fact]
    public void Min_TimeSpan_WithNegativeValues_ReturnsLesserValue()
    {
        // Arrange
        var timeSpan1 = TimeSpan.FromHours(-5);
        var timeSpan2 = TimeSpan.FromHours(-2);

        // Act
        var result = DateTimeHelper.Min(timeSpan1, timeSpan2);

        // Assert
        Assert.Equal(timeSpan1, result);
    }

    #endregion

    #region IsConsecutiveDays

    [Fact]
    public void IsConsecutiveDays_WithConsecutiveDates_ReturnsTrue()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 2),
            new DateTime(2024, 1, 3),
            new DateTime(2024, 1, 4)
        };

        // Act
        var result = dates.IsConsecutiveDays();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsConsecutiveDays_WithNonConsecutiveDates_ReturnsFalse()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 2),
            new DateTime(2024, 1, 4),
            new DateTime(2024, 1, 5)
        };

        // Act
        var result = dates.IsConsecutiveDays();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsConsecutiveDays_WithUnorderedConsecutiveDates_ReturnsTrue()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2024, 1, 3),
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 2),
            new DateTime(2024, 1, 4)
        };

        // Act
        var result = dates.IsConsecutiveDays();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsConsecutiveDays_WithSingleDate_ReturnsTrue()
    {
        // Arrange
        var dates = new[] { new DateTime(2024, 1, 1) };

        // Act
        var result = dates.IsConsecutiveDays();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsConsecutiveDays_WithEmptyList_ReturnsTrue()
    {
        // Arrange
        var dates = Array.Empty<DateTime>();

        // Act
        var result = dates.IsConsecutiveDays();

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Range - DateTime

    [Fact]
    public void Range_DateTime_WithDays_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateTime(2024, 1, 1);
        var max = new DateTime(2024, 1, 5);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Day).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(new DateTime(2024, 1, 1), result[0]);
        Assert.Equal(new DateTime(2024, 1, 2), result[1]);
        Assert.Equal(new DateTime(2024, 1, 5), result[4]);
    }

    [Fact]
    public void Range_DateTime_WithHours_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateTime(2024, 1, 1, 0, 0, 0);
        var max = new DateTime(2024, 1, 1, 5, 0, 0);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Hour).ToList();

        // Assert
        Assert.Equal(6, result.Count);
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0), result[0]);
        Assert.Equal(new DateTime(2024, 1, 1, 5, 0, 0), result[5]);
    }

    [Fact]
    public void Range_DateTime_WithMonths_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateTime(2024, 1, 15);
        var max = new DateTime(2024, 6, 15);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Month).ToList();

        // Assert
        Assert.Equal(6, result.Count);
        Assert.Equal(new DateTime(2024, 1, 15), result[0]);
        Assert.Equal(new DateTime(2024, 6, 15), result[5]);
    }

    [Fact]
    public void Range_DateTime_WithYears_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateTime(2020, 6, 15);
        var max = new DateTime(2024, 6, 15);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Year).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(new DateTime(2020, 6, 15), result[0]);
        Assert.Equal(new DateTime(2024, 6, 15), result[4]);
    }

    [Fact]
    public void Range_DateTime_WithWeeks_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateTime(2024, 1, 1);
        var max = new DateTime(2024, 1, 22);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Week).ToList();

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(new DateTime(2024, 1, 1), result[0]);
        Assert.Equal(new DateTime(2024, 1, 22), result[3]);
    }

    [Fact]
    public void Range_DateTime_WithStep2Days_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateTime(2024, 1, 1);
        var max = new DateTime(2024, 1, 10);

        // Act
        var result = DateTimeHelper.Range(min, max, 2, TimeUnit.Day).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(new DateTime(2024, 1, 1), result[0]);
        Assert.Equal(new DateTime(2024, 1, 3), result[1]);
        Assert.Equal(new DateTime(2024, 1, 9), result[4]);
    }

    [Fact]
    public void Range_DateTime_WithMinutesAndSeconds_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateTime(2024, 1, 1, 12, 0, 0);
        var max = new DateTime(2024, 1, 1, 12, 3, 0);

        // Act
        var resultMinutes = DateTimeHelper.Range(min, max, 1, TimeUnit.Minute).ToList();

        // Assert
        Assert.Equal(4, resultMinutes.Count);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), resultMinutes[0]);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 3, 0), resultMinutes[3]);
    }

    [Fact]
    public void Range_DateTime_WithMilliseconds_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateTime(2024, 1, 1, 12, 0, 0, 0);
        var max = new DateTime(2024, 1, 1, 12, 0, 0, 5);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Millisecond).ToList();

        // Assert
        Assert.Equal(6, result.Count);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0, 0), result[0]);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0, 5), result[5]);
    }

    #endregion

    #region Range - DateOnly

    [Fact]
    public void Range_DateOnly_WithDays_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateOnly(2024, 1, 1);
        var max = new DateOnly(2024, 1, 5);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Day).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(new DateOnly(2024, 1, 1), result[0]);
        Assert.Equal(new DateOnly(2024, 1, 5), result[4]);
    }

    [Fact]
    public void Range_DateOnly_WithWeeks_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateOnly(2024, 1, 1);
        var max = new DateOnly(2024, 1, 29);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Week).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(new DateOnly(2024, 1, 1), result[0]);
        Assert.Equal(new DateOnly(2024, 1, 29), result[4]);
    }

    [Fact]
    public void Range_DateOnly_WithMonths_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateOnly(2024, 1, 15);
        var max = new DateOnly(2024, 4, 15);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Month).ToList();

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(new DateOnly(2024, 1, 15), result[0]);
        Assert.Equal(new DateOnly(2024, 4, 15), result[3]);
    }

    [Fact]
    public void Range_DateOnly_WithYears_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new DateOnly(2020, 6, 15);
        var max = new DateOnly(2023, 6, 15);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Year).ToList();

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(new DateOnly(2020, 6, 15), result[0]);
        Assert.Equal(new DateOnly(2023, 6, 15), result[3]);
    }

    [Fact]
    public void Range_DateOnly_WithInvalidTimeUnit_ThrowsException()
    {
        // Arrange
        var min = new DateOnly(2024, 1, 1);
        var max = new DateOnly(2024, 1, 5);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => DateTimeHelper.Range(min, max, 1, TimeUnit.Hour).ToList());
    }

    #endregion

    #region Range - TimeOnly

    [Fact]
    public void Range_TimeOnly_WithHours_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new TimeOnly(8, 0);
        var max = new TimeOnly(12, 0);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Hour).ToList();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(new TimeOnly(8, 0), result[0]);
        Assert.Equal(new TimeOnly(12, 0), result[4]);
    }

    [Fact]
    public void Range_TimeOnly_WithMinutes_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new TimeOnly(10, 0);
        var max = new TimeOnly(10, 5);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Minute).ToList();

        // Assert
        Assert.Equal(6, result.Count);
        Assert.Equal(new TimeOnly(10, 0), result[0]);
        Assert.Equal(new TimeOnly(10, 5), result[5]);
    }

    [Fact]
    public void Range_TimeOnly_WithSeconds_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new TimeOnly(10, 0, 0);
        var max = new TimeOnly(10, 0, 3);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Second).ToList();

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(new TimeOnly(10, 0, 0), result[0]);
        Assert.Equal(new TimeOnly(10, 0, 3), result[3]);
    }

    [Fact]
    public void Range_TimeOnly_WithMilliseconds_ReturnsCorrectSequence()
    {
        // Arrange
        var min = new TimeOnly(10, 0, 0, 0);
        var max = new TimeOnly(10, 0, 0, 5);

        // Act
        var result = DateTimeHelper.Range(min, max, 1, TimeUnit.Millisecond).ToList();

        // Assert
        Assert.Equal(6, result.Count);
        Assert.Equal(new TimeOnly(10, 0, 0, 0), result[0]);
        Assert.Equal(new TimeOnly(10, 0, 0, 5), result[5]);
    }

    [Fact]
    public void Range_TimeOnly_WithInvalidTimeUnit_ThrowsException()
    {
        // Arrange
        var min = new TimeOnly(10, 0);
        var max = new TimeOnly(12, 0);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => DateTimeHelper.Range(min, max, 1, TimeUnit.Day).ToList());
    }

    #endregion

    #region GetDecade

    [Theory]
    [InlineData(2020, 2020, 2030)]
    [InlineData(2025, 2020, 2030)]
    [InlineData(2029, 2020, 2030)]
    [InlineData(1995, 1990, 2000)]
    [InlineData(2000, 2000, 2010)]
    [InlineData(2010, 2010, 2020)]
    public void GetDecade_ReturnsCorrectDecadeInterval(int year, int expectedStart, int expectedEnd)
    {
        // Act
        var result = DateTimeHelper.GetDecade(year);

        // Assert
        Assert.Equal(expectedStart, result.Start);
        Assert.Equal(expectedEnd, result.End);
    }

    #endregion

    #region GetCentury

    [Theory]
    [InlineData(2020, 2000, 2100)]
    [InlineData(2099, 2000, 2100)]
    [InlineData(2000, 2000, 2100)]
    [InlineData(1995, 1900, 2000)]
    [InlineData(1900, 1900, 2000)]
    [InlineData(2100, 2100, 2200)]
    public void GetCentury_ReturnsCorrectCenturyInterval(int year, int expectedStart, int expectedEnd)
    {
        // Act
        var result = DateTimeHelper.GetCentury(year);

        // Assert
        Assert.Equal(expectedStart, result.Start);
        Assert.Equal(expectedEnd, result.End);
    }

    #endregion

    #region NumberOfDaysInWeek

    [Fact]
    public void NumberOfDaysInWeek_Returns7()
    {
        // Act
        var result = DateTimeHelper.NumberOfDaysInWeek();

        // Assert
        Assert.Equal(7, result);
    }

    #endregion

    #region MaxNumberOfWeeksPerMonth

    [Fact]
    public void MaxNumberOfWeeksPerMonth_Returns6()
    {
        // Act
        var result = DateTimeHelper.MaxNumberOfWeeksPerMonth();

        // Assert
        Assert.Equal(6, result);
    }

    #endregion

    #region MinNumberOfWeeksPerMonth

    [Fact]
    public void MinNumberOfWeeksPerMonth_Returns4()
    {
        // Act
        var result = DateTimeHelper.MinNumberOfWeeksPerMonth();

        // Assert
        Assert.Equal(4, result);
    }

    #endregion

    #region TranslateDatePattern

    [Fact]
    [UseCulture("en-US")]
    public void TranslateDatePattern_WithValidProperty_ReturnsPropertyValue()
    {
        // Arrange
        var culture = CultureInfo.CurrentCulture;

        // Act
        var result = DateTimeHelper.TranslateDatePattern("ShortDatePattern", culture);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(culture.DateTimeFormat.ShortDatePattern, result);
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void TranslateDatePattern_WithDifferentCulture_ReturnsCorrectPattern()
    {
        // Arrange
        var culture = new CultureInfo("fr-FR");

        // Act
        var result = DateTimeHelper.TranslateDatePattern("LongDatePattern", culture);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(culture.DateTimeFormat.LongDatePattern, result);
    }

    [Fact]
    [UseCulture("en-US")]
    public void TranslateDatePattern_WithInvalidProperty_ReturnsTranslationServiceValue()
    {
        // Arrange
        var culture = CultureInfo.CurrentCulture;
        var key = "NonExistentProperty";

        // Act
        var result = DateTimeHelper.TranslateDatePattern(key, culture);

        // Assert
        Assert.NotNull(result);
    }

    #endregion
}
