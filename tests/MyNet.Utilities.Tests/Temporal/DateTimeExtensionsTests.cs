// -----------------------------------------------------------------------
// <copyright file="DateTimeExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Xunit;

namespace MyNet.Utilities.Tests.Temporal;

[Collection("UseCultureSequential")]
public class DateTimeExtensionsTests
{
    private const int DaysPerWeek = 7;

    [Fact]
    public void ToTimeZone_Should_Convert_Utc_To_Eastern()
    {
        // Arrange
        var utcDateTime = new DateTime(2024, 8, 20, 12, 0, 0, DateTimeKind.Utc);
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        // Act
        var easternDateTime = utcDateTime.ToTimeZone(easternTimeZone);

        // Assert
        var expectedDateTime = new DateTime(2024, 8, 20, 8, 0, 0, DateTimeKind.Unspecified);
        Assert.Equal(expectedDateTime, easternDateTime);
    }

    [Fact]
    public void ToTimeZone_Should_Convert_Utc_To_Pacific()
    {
        // Arrange
        var utcDateTime = new DateTime(2024, 8, 20, 12, 0, 0, DateTimeKind.Utc);
        var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        // Act
        var pacificDateTime = utcDateTime.ToTimeZone(pacificTimeZone);

        // Assert
        var expectedDateTime = new DateTime(2024, 8, 20, 5, 0, 0, DateTimeKind.Unspecified);
        Assert.Equal(expectedDateTime, pacificDateTime);
    }

    [Fact]
    public void ToTimeZone_Should_Convert_Local_To_Utc()
    {
        // Arrange
        var localDateTime = new DateTime(2024, 8, 20, 8, 0, 0, DateTimeKind.Local);
        var utcTimeZone = TimeZoneInfo.Utc;

        // Act
        var utcDateTime = localDateTime.ToTimeZone(utcTimeZone);

        // Assert
        var expectedDateTime = TimeZoneInfo.ConvertTime(localDateTime, TimeZoneInfo.Local, utcTimeZone);
        Assert.Equal(expectedDateTime, utcDateTime);
    }

    [Fact]
    public void ToTimeZone_Should_Convert_Utc_To_Local()
    {
        // Arrange
        var utcDateTime = new DateTime(2024, 8, 20, 12, 0, 0, DateTimeKind.Utc);
        var localTimeZone = TimeZoneInfo.Local;

        // Act
        var localDateTime = utcDateTime.ToTimeZone(localTimeZone);

        // Assert
        var expectedDateTime = TimeZoneInfo.ConvertTime(utcDateTime, TimeZoneInfo.Utc, localTimeZone);
        Assert.Equal(expectedDateTime, localDateTime);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(32)]
    [InlineData(40)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(0)]
    public void AgoFromFixedDateTime(int agoValue)
    {
        var originalPointInTime = new DateTime(1976, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc);

        Assert.Equal(agoValue.Years().Before(originalPointInTime), originalPointInTime.AddYears(-agoValue));
        Assert.Equal(agoValue.Months().Before(originalPointInTime), originalPointInTime.AddMonths(-agoValue));
        Assert.Equal(agoValue.Weeks().Before(originalPointInTime), originalPointInTime.AddDays(-agoValue * DaysPerWeek));
        Assert.Equal(agoValue.Days().Before(originalPointInTime), originalPointInTime.AddDays(-agoValue));

        Assert.Equal(agoValue.Hours().Before(originalPointInTime), originalPointInTime.AddHours(-agoValue));
        Assert.Equal(agoValue.Minutes().Before(originalPointInTime), originalPointInTime.AddMinutes(-agoValue));
        Assert.Equal(agoValue.Seconds().Before(originalPointInTime), originalPointInTime.AddSeconds(-agoValue));
        Assert.Equal(agoValue.Milliseconds().Before(originalPointInTime), originalPointInTime.AddMilliseconds(-agoValue));
        Assert.Equal(agoValue.Ticks().Before(originalPointInTime), originalPointInTime.AddTicks(-agoValue));
    }

    [Fact]
    public void AgoFromOneMonth()
    {
        var originalPointInTime = new DateTime(1976, 4, 30, 0, 0, 0, DateTimeKind.Utc);

        Assert.Equal(1.Months().Before(originalPointInTime), new(1976, 3, 30, 0, 0, 0, DateTimeKind.Utc));
        Assert.Equal(1.Months().From(originalPointInTime), new(1976, 5, 30, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void AddFluentTimeSpan()
    {
        var originalPointInTime = new DateTime(1976, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var fluentTimeSpan = 1.Months();
        Assert.Equal(new(1976, 2, 1, 0, 0, 0, DateTimeKind.Utc), originalPointInTime.AddFluentTimeSpan(fluentTimeSpan));
    }

    [Fact]
    public void SubtractFluentTimeSpan()
    {
        var originalPointInTime = new DateTime(1976, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        var fluentTimeSpan = 1.Months();
        Assert.Equal(new(1976, 1, 1, 0, 0, 0, DateTimeKind.Utc), originalPointInTime.SubtractFluentTimeSpan(fluentTimeSpan));
    }

    [Fact]
    public void AgoFromOneYearLeap()
    {
        var originalPointInTime = new DateTime(2004, 2, 29, 0, 0, 0, DateTimeKind.Utc);

        Assert.Equal(1.Years().Before(originalPointInTime), new(2003, 2, 28, 0, 0, 0, DateTimeKind.Utc));
        Assert.Equal(1.Years().From(originalPointInTime), new(2005, 2, 28, 0, 0, 0, DateTimeKind.Utc));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(32)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(0)]
    public void FromFromFixedDateTime(int value)
    {
        var originalPointInTime = new DateTime(1976, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc);

        Assert.Equal(value.Years().From(originalPointInTime), originalPointInTime.AddYears(value));
        Assert.Equal(value.Months().From(originalPointInTime), originalPointInTime.AddMonths(value));
        Assert.Equal(value.Weeks().From(originalPointInTime), originalPointInTime.AddDays(value * DaysPerWeek));
        Assert.Equal(value.Days().From(originalPointInTime), originalPointInTime.AddDays(value));

        Assert.Equal(value.Hours().From(originalPointInTime), originalPointInTime.AddHours(value));
        Assert.Equal(value.Minutes().From(originalPointInTime), originalPointInTime.AddMinutes(value));
        Assert.Equal(value.Seconds().From(originalPointInTime), originalPointInTime.AddSeconds(value));
        Assert.Equal(value.Milliseconds().From(originalPointInTime), originalPointInTime.AddMilliseconds(value));
        Assert.Equal(value.Ticks().From(originalPointInTime), originalPointInTime.AddTicks(value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(23)]
    public void ChangeTimeHour(int value)
    {
        var toChange = new DateTime(2008, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc);

        var result = toChange.SetHour(value);
        var expected = new DateTime(2008, 10, 25, value, 0, 0, 0, DateTimeKind.Utc);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(24)]
    [InlineData(-1)]
    public void ChangeTimeHourArgChecks(int value)
    {
        var toChange = new DateTime(2008, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc);

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => toChange.SetHour(value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    [InlineData(59)]
    public void ChangeTimeMinute(int value)
    {
        var toChange = new DateTime(2008, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc);

        var expected = new DateTime(2008, 10, 25, 0, value, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, toChange.At(0, value));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(60)]
    public void ChangeTimeMinuteArgChecks(int value)
    {
        var toChange = new DateTime(2008, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc);
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => toChange.At(0, value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    [InlineData(59)]
    public void ChangeTimeSecond(int value)
    {
        var toChange = new DateTime(2008, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc);

        var changed = toChange.At(0, 0, value);

        var expected = new DateTime(2008, 10, 25, 0, 0, value, 0, DateTimeKind.Utc);

        Assert.Equal(expected, changed);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(60)]
    public void ChangeTimeSecondArgChecks(int value)
    {
        var toChange = new DateTime(2008, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc);

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => toChange.At(0, 0, value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(999)]
    public void ChangeTimeMillisecond(int value)
    {
        var toChange = new DateTime(2008, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc);

        var expected = new DateTime(2008, 10, 25, 0, 0, 0, value, DateTimeKind.Utc);
        Assert.Equal(expected, toChange.At(0, 0, 0, value));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1000)]
    public void ChangeTimeMillisecondArgCheck(int value)
    {
        var toChange = new DateTime(2008, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc);
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => toChange.At(0, 0, 0, value));
    }

    [Fact]
    public void TimeZoneTests()
    {
        /* story:
         * 1. a web client submits a request to the server for "today",
         * 2. a developer uses :BeginningOfDay and :EndOfDay to perform clamping server-side.
         *
         * expected:
         * 3. user expects a timezone-correct utc responses from the server,
         *
         * actual:
         * 4. user receives a utc response that is too early (:BeginningOfDay), or
         * 5. user receives a utc response that is too late (:EndOfDay)
         */
        for (var i = -11; i <= 12; i++)
        {
            var beginningOfDayUtc = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var actualDayStart = beginningOfDayUtc.BeginningOfDay(i);
            var actualDayEnd = beginningOfDayUtc.EndOfDay(i);
            var expectedDayStart = beginningOfDayUtc
                .AddHours(i);
            var expectedDayEnd = beginningOfDayUtc
                .SetHour(23).SetMinute(59).SetSecond(59).SetMillisecond(999)
                .AddHours(i);
            Assert.Equal(expectedDayStart, actualDayStart);
            Assert.Equal(expectedDayEnd, actualDayEnd);
        }
    }

    [Fact]
    public void BasicTests()
    {
        var now = DateTime.Now;
        Assert.Equal(new(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Utc), DateTime.Now.EndOfDay());
        Assert.Equal(new(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Utc), DateTime.Now.BeginningOfDay());

        var firstBirthDay = new DateTime(1977, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(firstBirthDay + new TimeSpan(1, 0, 5, 0, 0), firstBirthDay + 1.Days() + 5.Minutes());

        Assert.Equal(now + TimeSpan.FromDays(1), now.NextDay());
        Assert.Equal(now - TimeSpan.FromDays(1), now.PreviousDay());

        Assert.Equal(now + TimeSpan.FromDays(7), now.WeekAfter());
        Assert.Equal(now - TimeSpan.FromDays(7), now.WeekEarlier());

        Assert.Equal(new(2009, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2008, 12, 31, 0, 0, 0, DateTimeKind.Utc).Add(1.Days()));
        Assert.Equal(new(2009, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2009, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Add(1.Days()));

        var sampleDate = new DateTime(2009, 1, 1, 13, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(new(2009, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), sampleDate.Noon());
        Assert.Equal(new(2009, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), sampleDate.Midnight());

        Assert.Equal(3.Days() + 3.Days(), 6.Days());
        Assert.Equal(102.Days() - 3.Days(), 99.Days());

        Assert.Equal(24.Hours(), 1.Days());

        sampleDate = new(2008, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(3.Days().Since(sampleDate), sampleDate + 3.Days());

        var saturday = new DateTime(2008, 10, 25, 12, 0, 0, DateTimeKind.Utc);
        Assert.Equal(new(2008, 11, 1, 12, 0, 0, DateTimeKind.Utc), saturday.Next(DayOfWeek.Saturday));

        Assert.Equal(new(2008, 10, 18, 12, 0, 0, DateTimeKind.Utc), saturday.Previous(DayOfWeek.Saturday));
    }

    [Fact]
    public void NextYearReturnsTheSameDateButNextYear()
    {
        var birthday = new DateTime(1976, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc);
        var nextYear = birthday.NextYear();
        var expected = new DateTime(1977, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, nextYear);
    }

    [Fact]
    public void PreviousYearReturnsTheSameDateButPreviousYear()
    {
        var birthday = new DateTime(1976, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc);
        var previousYear = birthday.PreviousYear();
        var expected = new DateTime(1975, 12, 31, 17, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, previousYear);
    }

    [Fact]
    public void NextYearIfNextYearDoesNotHaveTheSameDayInTheSameMonthThenCalculateHowManyDaysIsMissingAndAddThatToTheLastDayInTheSameMonthNextYear()
    {
        var someBirthday = new DateTime(2008, 2, 29, 17, 0, 0, 0, DateTimeKind.Utc);
        var nextYear = someBirthday.NextYear();
        var expected = new DateTime(2009, 3, 1, 17, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, nextYear);
    }

    [Fact]
    public void PreviousYearIfPreviousYearDoesNotHaveTheSameDayInTheSameMonthThenCalculateHowManyDaysIsMissingAndAddThatToTheLastDayInTheSameMonthPreviousYear()
    {
        var someBirthday = new DateTime(2012, 2, 29, 17, 0, 0, 0, DateTimeKind.Utc);
        var previousYear = someBirthday.PreviousYear();
        var expected = new DateTime(2011, 3, 1, 17, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, previousYear);
    }

    [Fact]
    public void NextReturnsNextFridayProperly()
    {
        var friday = new DateTime(2009, 7, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var reallyNextFriday = new DateTime(2009, 7, 17, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextFriday = friday.Next(DayOfWeek.Friday);

        Assert.Equal(reallyNextFriday, nextFriday);
    }

    [Fact]
    public void NextReturnsPreviousFridayProperly()
    {
        var friday = new DateTime(2009, 7, 17, 1, 0, 0, 0, DateTimeKind.Utc);
        var reallyPreviousFriday = new DateTime(2009, 7, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var previousFriday = friday.Previous(DayOfWeek.Friday);

        Assert.Equal(reallyPreviousFriday, previousFriday);
    }

    [Fact]
    public void IsBeforeReturnsTrueForGivenDateThatIsInTheFuture()
    {
        // arrange
        var toCompareWith = DateTime.Today + 1.Days();

        // assert
        Assert.True(DateTime.Today.IsBefore(toCompareWith));
    }

    [Fact]
    public void IsBeforeReturnsFalseForGivenDateThatIsSame()
    {
        // arrange
        var toCompareWith = DateTime.Today;

        // assert
        Assert.False(toCompareWith.IsBefore(toCompareWith));
    }

    [Fact]
    public void IsAfterReturnsTrueForGivenDateThatIsInThePast()
    {
        // arrange
        var toCompareWith = DateTime.Today - 1.Days();

        // assert
        Assert.True(DateTime.Today.IsAfter(toCompareWith));
    }

    [Fact]
    public void IsAfterReturnsFalseForGivenDateThatIsSame()
    {
        // arrange
        var toCompareWith = DateTime.Today;

        // assert
        Assert.False(toCompareWith.IsAfter(toCompareWith));
    }

    [Fact]
    public void AtSetsHourAndMinutesProperly()
    {
        var expected = new DateTime(2002, 12, 17, 18, 06, 00, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2002, 12, 17, 17, 05, 01, DateTimeKind.Utc).At(18, 06));
    }

    [Fact]
    public void AtSetsHourAndMinutesAndSecondsProperly()
    {
        var expected = new DateTime(2002, 12, 17, 18, 06, 02, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2002, 12, 17, 17, 05, 01, DateTimeKind.Utc).At(18, 06, 02));
    }

    [Fact]
    public void AtSetsHourAndMinutesAndMillisecondsProperly()
    {
        var expected = new DateTime(2002, 12, 17, 18, 06, 02, 03, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2002, 12, 17, 17, 05, 01, DateTimeKind.Utc).At(18, 06, 02, 03));
    }

    [Fact]
    public void FirstDayOfMonthSetsTheDayToOne()
    {
        var expected = new DateTime(2002, 12, 1, 17, 05, 01, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2002, 12, 17, 17, 05, 01, DateTimeKind.Utc).FirstDayOfMonth());
    }

    [Fact]
    public void PreviousQuarterFirstDaySetsTheDayToOne()
    {
        var expected = new DateTime(2001, 10, 1, 3, 5, 6, DateTimeKind.Utc);
        Assert.Equal(expected.BeginningOfDay(), 1.Quarters().Ago(new DateTime(2002, 1, 10, 4, 5, 6, DateTimeKind.Utc).FirstDayOfQuarter().BeginningOfDay()));
    }

    [Fact]
    public void PreviousQuarterLastDaySetsTheDayToLastDayOfQuarter()
    {
        var expected = new DateTime(2001, 12, 31, 3, 5, 6, DateTimeKind.Utc);
        Assert.Equal(expected.BeginningOfDay(), 1.Quarters().Ago(new DateTime(2002, 1, 10, 4, 5, 6, DateTimeKind.Utc).LastDayOfQuarter().BeginningOfDay()));
    }

    [Fact]
    public void NextQuarterFirstDaySetsTheDayToOne()
    {
        var expected = new DateTime(2002, 4, 1, 3, 5, 6, DateTimeKind.Utc);
        Assert.Equal(expected.BeginningOfDay(), 1.Quarters().From(new DateTime(2002, 1, 10, 4, 5, 6, DateTimeKind.Utc).FirstDayOfQuarter().BeginningOfDay()));
    }

    [Fact]
    public void NextQuarterLastDaySetsTheDayToLastDayOfQuarter()
    {
        var expected = new DateTime(2002, 6, 30, 3, 5, 6, DateTimeKind.Utc);
        Assert.Equal(expected.BeginningOfDay(), 1.Quarters().From(new DateTime(2002, 1, 10, 4, 5, 6, DateTimeKind.Utc).LastDayOfQuarter().BeginningOfDay()));
    }

    [Fact]
    public void FirstDayOfQuarterSetsTheDayToOne()
    {
        var expected = new DateTime(2002, 1, 1, 7, 8, 9, DateTimeKind.Utc);
        Assert.Equal(expected.BeginningOfDay(), new DateTime(2002, 3, 22, 12, 12, 12, DateTimeKind.Utc).FirstDayOfQuarter().BeginningOfDay());
    }

    [Fact]
    public void LastDayOfQuarterSetsTheDayToLastDayOfQuarter()
    {
        var expected = new DateTime(2002, 3, 31, 4, 5, 6, DateTimeKind.Utc);
        Assert.Equal(expected.BeginningOfDay(), new DateTime(2002, 3, 22, 12, 12, 12, DateTimeKind.Utc).LastDayOfQuarter().BeginningOfDay());
    }

    [Fact]
    public void FirstDayOfQuarterQ4SetsDayToFirstDay()
    {
        var expected = new DateTime(2002, 10, 1, 7, 8, 9, DateTimeKind.Utc);
        Assert.Equal(expected.BeginningOfDay(), new DateTime(2002, 11, 22, 12, 12, 12, DateTimeKind.Utc).FirstDayOfQuarter().BeginningOfDay());
    }

    [Fact]
    public void LastDayOfQuarterQ4SetsTheDayToLastDayOfQuarter()
    {
        var expected = new DateTime(2002, 12, 31, 4, 5, 6, DateTimeKind.Utc);
        Assert.Equal(expected.BeginningOfDay(), new DateTime(2002, 11, 22, 12, 12, 12, DateTimeKind.Utc).LastDayOfQuarter().BeginningOfDay());
    }

    [Fact]
    public void LastDayOfMonthSetsTheDayToLastDayInThatMonth()
    {
        var expected = new DateTime(2002, 1, 31, 17, 05, 01, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2002, 1, 1, 17, 05, 01, DateTimeKind.Utc).LastDayOfMonth());
    }

    [Fact]
    public void AddBusinessDaysAdsDaysProperlyWhenThereIsWeekendAhead()
    {
        var expected = new DateTime(2009, 7, 13, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2009, 7, 9, 0, 0, 0, DateTimeKind.Utc).AddBusinessDays(2));
    }

    [Fact]
    public void AddBusinessDaysNegative()
    {
        var expected = new DateTime(2009, 7, 9, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2009, 7, 13, 0, 0, 0, DateTimeKind.Utc).AddBusinessDays(-2));
    }

    [Fact]
    public void SubtractBusinessDaysSubtractsDaysProperlyWhenThereIsWeekend()
    {
        var expected = new DateTime(2009, 7, 9, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2009, 7, 13, 0, 0, 0, DateTimeKind.Utc).SubtractBusinessDays(2));
    }

    [Fact]
    public void SubtractBusinessDaysNegative()
    {
        var expected = new DateTime(2009, 7, 13, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2009, 7, 9, 0, 0, 0, DateTimeKind.Utc).SubtractBusinessDays(-2));
    }

    [Fact]
    public void IsInFuture()
    {
        var now = DateTime.Now;
        Assert.False(now.Subtract(2.Seconds()).IsInFuture());
        Assert.False(now.IsInFuture());
        Assert.True(now.Add(2.Seconds()).IsInFuture());
    }

    [Fact]
    public void IsInPast()
    {
        var now = DateTime.Now;
        Assert.True(now.Subtract(2.Seconds()).IsInPast());
        Assert.False(now.Add(2.Seconds()).IsInPast());
    }

    [Theory]
    [InlineData(24)]
    [InlineData(25)]
    [InlineData(26)]
    [InlineData(27)]
    [InlineData(28)]
    [InlineData(29)]
    [InlineData(30)]
    public void FirstDayOfWeekFirstDayOfWeekIsMonday(int value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        var expected = new DateTime(2011, 1, 24, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2011, 1, value, 0, 0, 0, DateTimeKind.Utc).FirstDayOfWeek());
    }

    [Theory]
    [InlineData(23)]
    [InlineData(24)]
    [InlineData(25)]
    [InlineData(26)]
    [InlineData(27)]
    [InlineData(28)]
    [InlineData(29)]
    public void FirstDayOfWeekFirstDayOfWeekIsSunday(int value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Sunday;
        CultureInfo.CurrentCulture = culture;
        var expected = new DateTime(2011, 1, 23, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2011, 1, value, 0, 0, 0, DateTimeKind.Utc).FirstDayOfWeek());
    }

    [Theory]
    [InlineData("2011-06-22T06:40:20.005")]
    [InlineData("2011-12-31T06:40:20.005")]
    [InlineData("2011-01-01T06:40:20.005")]
    public void FirstDayOfYearBasicTest(string value) => Assert.Equal(new(2011, 1, 1, 6, 40, 20, 5, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).FirstDayOfYear());

    [Theory]
    [InlineData("2011-12-24T06:40:20.005")]
    [InlineData("2011-12-19T06:40:20.005")]
    [InlineData("2011-12-25T06:40:20.005")]
    public void LastDayOfWeekBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 25, 06, 40, 20, 5, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).LastDayOfWeek());
    }

    [Theory]
    [InlineData("2011-02-13T06:40:20.005")]
    [InlineData("2011-01-01T06:40:20.005")]
    [InlineData("2011-12-31T06:40:20.005")]
    public void LastDayOfYearBasicTest(string value) => Assert.Equal(new(2011, 12, 31, 06, 40, 20, 5, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).LastDayOfYear());

    [Fact]
    public void PreviousMonthBasicTest()
    {
        var expected = new DateTime(2009, 12, 20, 06, 40, 20, 5, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2010, 1, 20, 06, 40, 20, 5, DateTimeKind.Utc).PreviousMonth());
    }

    [Fact]
    public void PreviousMonthPreviousMonthDoesntHaveThatManyDays()
    {
        var expected = new DateTime(2009, 2, 28, 06, 40, 20, 5, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2009, 3, 31, 06, 40, 20, 5, DateTimeKind.Utc).PreviousMonth());
    }

    [Fact]
    public void NextMonthBasicTest()
    {
        var expected = new DateTime(2013, 1, 5, 06, 40, 20, 5, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2012, 12, 5, 06, 40, 20, 5, DateTimeKind.Utc).NextMonth());
    }

    [Fact]
    public void PreviousMonthNextMonthDoesntHaveThatManyDays()
    {
        var expected = new DateTime(2013, 2, 28, 06, 40, 20, 5, DateTimeKind.Utc);
        Assert.Equal(expected, new DateTime(2013, 1, 31, 06, 40, 20, 5, DateTimeKind.Utc).NextMonth());
    }

    [Theory]
    [InlineData("2015-11-25T00:00:00.000")]
    [InlineData("2015-12-25T00:00:00.000")]
    [InlineData("2015-10-25T00:00:00.000")]
    public void SameYearY(string dateValue)
    {
        var other = DateTime.Parse(dateValue, CultureInfo.InvariantCulture);
        var date = new DateTime(2015, 11, 25, 0, 0, 0, DateTimeKind.Utc);

        Assert.True(date.SameYear(other));
    }

    [Theory]
    [InlineData("2014-11-25T00:00:00.000")]
    [InlineData("2013-11-25T00:00:00.000")]
    [InlineData("1995-11-25T00:00:00.000")]
    public void SameYearN(string dateValue)
    {
        var other = DateTime.Parse(dateValue, CultureInfo.InvariantCulture);
        var date = new DateTime(2015, 11, 25, 0, 0, 0, DateTimeKind.Utc);

        Assert.False(date.SameYear(other));
    }

    [Theory]
    [InlineData("2015-11-25T00:00:00.000")]
    [InlineData("2015-11-01T00:00:00.000")]
    [InlineData("2015-11-15T00:00:00.000")]
    public void SameMonthY(string dateValue)
    {
        var other = DateTime.Parse(dateValue, CultureInfo.InvariantCulture);
        var date = new DateTime(2015, 11, 25, 0, 0, 0, DateTimeKind.Utc);

        Assert.True(date.SameMonth(other));
    }

    [Theory]
    [InlineData("2016-11-25T00:00:00.000")]
    [InlineData("2014-11-01T12:00:00.000")]
    [InlineData("2015-10-15T18:00:00.000")]
    public void SameMonthN(string dateValue)
    {
        var other = DateTime.Parse(dateValue, CultureInfo.InvariantCulture);
        var date = new DateTime(2015, 11, 25, 0, 0, 0, DateTimeKind.Utc);

        Assert.False(date.SameMonth(other));
    }

    [Theory]
    [InlineData("2015-11-25T12:25:00.000")]
    [InlineData("2015-11-25T23:59:59.999")]
    public void SameDayY(string dateValue)
    {
        var other = DateTime.Parse(dateValue, CultureInfo.InvariantCulture);
        var date = new DateTime(2015, 11, 25, 0, 0, 0, DateTimeKind.Utc);

        Assert.True(date.SameDay(other));
    }

    [Theory]
    [InlineData("2014-11-25T12:25:00.000")]
    [InlineData("2015-12-25T23:59:59.999")]
    [InlineData("2015-10-25T23:59:59.999")]
    public void SameDayN(string dateValue)
    {
        var other = DateTime.Parse(dateValue, CultureInfo.InvariantCulture);
        var date = new DateTime(2015, 11, 25, 0, 0, 0, DateTimeKind.Utc);

        Assert.False(date.SameDay(other));
    }

    [Theory]
    [InlineData("2011-12-19T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void BeginningOfWeekMondayBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).BeginningOfWeek());
    }

    [Theory]
    [InlineData("2011-12-19T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void EndOfWeekMondayBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 25, 23, 59, 59, 999, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).EndOfWeek());
    }

    [Theory]
    [InlineData("2011-12-18T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void BeginningOfWeekSundayBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Sunday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 18, 0, 0, 0, 0, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).BeginningOfWeek());
    }

    [Theory]
    [InlineData("2011-12-18T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void EndOfWeekSundayBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Sunday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 24, 23, 59, 59, 999, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).EndOfWeek());
    }

    [Theory]
    [InlineData("2011-12-19T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void BeginningOfMonthBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 01, 0, 0, 0, 0, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).BeginningOfMonth());
    }

    [Theory]
    [InlineData("2011-12-18T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void EndOfMonthBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Sunday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).EndOfMonth());
    }

    [Theory]
    [InlineData("2011-12-18T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void BeginningOfQuarterBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 10, 01, 0, 0, 0, 0, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).BeginningOfQuarter());
    }

    [Theory]
    [InlineData("2011-12-18T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void EndOfQuarterBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).EndOfQuarter());
    }

    [Theory]
    [InlineData("2011-12-18T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void BeginningOfYearBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).BeginningOfYear());
    }

    [Theory]
    [InlineData("2011-12-18T06:40:20.005")]
    [InlineData("2011-12-20T06:40:20.005")]
    [InlineData("2011-12-24T06:40:20.005")]
    public void EndOfYearBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2011, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).EndOfYear());
    }

    [Theory]
    [InlineData("2010-12-18T06:40:20.005")]
    [InlineData("2019-12-20T06:40:20.005")]
    [InlineData("2015-07-24T06:40:20.005")]
    public void BeginningOfDecadeBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2010, 01, 01, 00, 00, 00, 000, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).BeginningOfDecade());
    }

    [Theory]
    [InlineData("2010-12-18T06:40:20.005")]
    [InlineData("2019-12-20T06:40:20.005")]
    [InlineData("2015-07-24T06:40:20.005")]
    public void EndOfDecadeBasicTest(string value)
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        CultureInfo.CurrentCulture = culture;
        Assert.Equal(new(2019, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc), DateTime.Parse(value, CultureInfo.InvariantCulture).EndOfDecade());
    }

    [Theory]
    [InlineData("2010-12-18T06:40:20.005", "2010-12-14T06:40:20.005", 4)]
    [InlineData("2019-12-20T06:40:20.005", "2019-12-20T04:40:20.005", 0)]
    [InlineData("2015-07-24T06:40:20.005", "2015-08-03T06:40:20.005", 10)]
    public void NumberOfDaysBasicTest(string date1, string date2, int result) => Assert.Equal(result, DateTime.Parse(date1, CultureInfo.InvariantCulture).NumberOfDays(DateTime.Parse(date2, CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("2010-12-18T06:40:20.005", "2010-10-14T06:40:20.005", -2)]
    [InlineData("2019-12-20T06:40:20.005", "2019-03-20T04:40:20.005", -9)]
    [InlineData("2015-07-24T06:40:20.005", "2017-08-03T06:40:20.005", 25)]
    [InlineData("2015-07-24T06:40:20.005", "2015-07-01T06:40:20.005", 0)]
    public void CompareMonthBasicTest(string date1, string date2, int result) => Assert.Equal(result, DateTime.Parse(date1, CultureInfo.InvariantCulture).CompareMonth(DateTime.Parse(date2, CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("2010-12-18T06:40:20.005", "2010-10-14T06:40:20.005", 2)]
    [InlineData("2019-12-20T06:40:20.005", "2019-03-20T04:40:20.005", 9)]
    [InlineData("2015-07-24T06:40:20.005", "2017-08-03T06:40:20.005", 25)]
    [InlineData("2015-07-24T06:40:20.005", "2015-07-01T06:40:20.005", 0)]
    public void NumberOfMonthsBasicTest(string date1, string date2, int result) => Assert.Equal(result, DateTime.Parse(date1, CultureInfo.InvariantCulture).NumberOfMonths(DateTime.Parse(date2, CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("2010-12-18T06:40:20.005", "2012-10-14T06:40:20.005", 2)]
    [InlineData("2019-12-20T06:40:20.005", "2015-03-20T04:40:20.005", -4)]
    [InlineData("2015-07-24T06:40:20.005", "2024-08-03T06:40:20.005", 9)]
    [InlineData("2015-07-24T06:40:20.005", "2015-07-01T06:40:20.005", 0)]
    public void CompareYearBasicTest(string date1, string date2, int result) => Assert.Equal(result, DateTime.Parse(date1, CultureInfo.InvariantCulture).CompareYear(DateTime.Parse(date2, CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("2010-12-18T06:40:20.005", "2012-10-14T06:40:20.005", 2)]
    [InlineData("2019-12-20T06:40:20.005", "2015-03-20T04:40:20.005", 4)]
    [InlineData("2015-07-24T06:40:20.005", "2024-08-03T06:40:20.005", 9)]
    [InlineData("2015-07-24T06:40:20.005", "2015-07-01T06:40:20.005", 0)]
    public void NumberOfYearsBasicTest(string date1, string date2, int result) => Assert.Equal(result, DateTime.Parse(date1, CultureInfo.InvariantCulture).NumberOfYears(DateTime.Parse(date2, CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("2010-12-18T06:40:20.005", "2012-10-14T06:40:20.005", 95)]
    [InlineData("2019-12-20T06:40:20.005", "2015-03-20T04:40:20.005", -248)]
    [InlineData("2015-07-24T06:40:20.005", "2024-08-03T06:40:20.005", 471)]
    [InlineData("2015-07-24T06:40:20.005", "2015-07-01T06:40:20.005", -3)]
    public void CompareWeekBasicTest(string date1, string date2, int result) => Assert.Equal(result, DateTime.Parse(date1, CultureInfo.InvariantCulture).CompareWeek(DateTime.Parse(date2, CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("2010-12-18T06:40:20.005", "2012-10-14T06:40:20.005", 95)]
    [InlineData("2019-12-20T06:40:20.005", "2015-03-20T04:40:20.005", 248)]
    [InlineData("2015-07-24T06:40:20.005", "2024-08-03T06:40:20.005", 471)]
    [InlineData("2015-07-24T06:40:20.005", "2015-07-01T06:40:20.005", 3)]
    public void NumberOfWeeksBasicTest(string date1, string date2, int result) => Assert.Equal(result, DateTime.Parse(date1, CultureInfo.InvariantCulture).NumberOfWeeks(DateTime.Parse(date2, CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData("2011-10-14T06:40:20.005", "2010-12-18T06:40:20.005", "2012-10-14T06:40:20.005", true)]
    [InlineData("2016-03-20T04:40:20.005", "2019-12-20T06:40:20.005", "2015-03-20T04:40:20.005", true)]
    [InlineData("2026-08-03T06:40:20.005", "2015-07-24T06:40:20.005", "2024-08-03T06:40:20.005", false)]
    [InlineData("2015-07-01T06:40:20.005", "2015-07-24T06:40:20.005", "2015-07-01T06:40:20.005", true)]
    [InlineData("2012-08-03T06:40:20.005", "2015-07-24T06:40:20.005", "2024-08-03T06:40:20.005", false)]
    [InlineData("2015-07-01T09:40:20.005", "2014-07-24T06:40:20.005", "2015-07-01T06:40:20.005", true)]
    [InlineData("2014-07-24T05:40:20.005", "2014-07-24T06:40:20.005", "2015-07-01T06:40:20.005", true)]
    public void InRangeWithDiscardDateBasicTest(string date1, string date2, string date3, bool result)
    {
        var value = DateTime.Parse(date1, CultureInfo.InvariantCulture);
        var bound1 = DateTime.Parse(date2, CultureInfo.InvariantCulture);
        var bound2 = DateTime.Parse(date3, CultureInfo.InvariantCulture);
        var start = bound1 <= bound2 ? bound1 : bound2;
        var end = bound1 <= bound2 ? bound2 : bound1;

        Assert.Equal(result, value.InRange(start, end));
    }
}
