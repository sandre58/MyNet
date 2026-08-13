// -----------------------------------------------------------------------
// <copyright file="CalendarAmountTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MyNet.Temporal;
using Xunit;

namespace MyNet.Temporal.Tests.Temporal;

public class CalendarAmountTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(32)]
    [InlineData(100)]
    [InlineData(-1)]
    [InlineData(0)]
    public void YearsMonthsReturnCalendarAmount(int value)
    {
        Assert.Equal(new CalendarAmount { Years = value }, value.Years());
        Assert.Equal(new CalendarAmount { Months = value }, value.Months());
        Assert.Equal(new CalendarAmount { Months = value * 3 }, value.Quarters());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(32)]
    [InlineData(100)]
    [InlineData(-1)]
    [InlineData(0)]
    public void FixedUnitsReturnTimeSpan(int value)
    {
        Assert.Equal(TimeSpan.FromDays(value * 7), value.Weeks());
        Assert.Equal(TimeSpan.FromDays(value), value.Days());
        Assert.Equal(TimeSpan.FromHours(value), value.Hours());
        Assert.Equal(TimeSpan.FromMinutes(value), value.Minutes());
        Assert.Equal(TimeSpan.FromSeconds(value), value.Seconds());
        Assert.Equal(TimeSpan.FromMilliseconds(value), value.Milliseconds());
        Assert.Equal(TimeSpan.FromTicks(value), value.Ticks());
    }

    [Fact]
    public void DoubleWeeks_PreservesFractionalDays() =>
        Assert.Equal(TimeSpan.FromDays(3.5), 0.5.Weeks());

    [Fact]
    public void Subtract()
    {
        var halfDay = .5.Days();
        Assert.Equal(3, 3.5.Days().Subtract(halfDay).Days);
        var timeSpan = new TimeSpan(3, 12, 0, 0);
        Assert.Equal(3, timeSpan.SubtractCalendarAmount(halfDay).Days);
        Assert.Equal(3, (timeSpan - halfDay).Days);
        Assert.Equal(-3, (halfDay - timeSpan).Days);
    }

    [Fact]
    public void CompareToCalendarAmount_IsStructural()
    {
        var left = new CalendarAmount { Months = 1 };
        var right = new CalendarAmount { TimeSpan = TimeSpan.FromDays(30) };

        Assert.True(left.CompareTo(right) > 0);
        Assert.False(left == right);
        Assert.False(left <= right && left >= right);
    }

    [Fact]
    public void CompareToTimeSpan_WhenNoCalendarComponents_IsExact()
    {
        CalendarAmount amount = TimeSpan.FromDays(3);
        Assert.Equal(0, amount.CompareTo(TimeSpan.FromDays(3)));
        Assert.Equal(-1, amount.CompareTo(TimeSpan.FromDays(4)));
        Assert.Equal(1, amount.CompareTo(TimeSpan.FromDays(2)));
    }

    [Fact]
    public void CompareToObject_AcceptsCalendarAmount()
    {
        var amount = new CalendarAmount { Years = 1 };
        Assert.Equal(0, amount.CompareTo((object)new CalendarAmount { Years = 1 }));
        Assert.Equal(-1, amount.CompareTo((object)new CalendarAmount { Years = 2 }));
    }

    [Fact]
    public void EqualsCalendarAmount()
    {
        Assert.True(new CalendarAmount { Months = 1 }.Equals(new CalendarAmount { Months = 1 }));
        Assert.False(new CalendarAmount { Months = 1 }.Equals(new CalendarAmount { TimeSpan = TimeSpan.FromDays(30) }));
    }

    [Fact]
    public void EqualsTimeSpan_OnlyWhenNoCalendarComponents()
    {
        CalendarAmount amount = TimeSpan.FromDays(3);
        Assert.True(amount.Equals(TimeSpan.FromDays(3)));
        Assert.False(new CalendarAmount { Months = 1 }.Equals(TimeSpan.FromDays(30)));
    }

    [Fact]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Testing Equals(object) method")]
    public void EqualsTimeSpanAsObject() =>
        Assert.True(((CalendarAmount)TimeSpan.FromDays(3)).Equals((object)TimeSpan.FromDays(3)));

    [Fact]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Testing Equals(object) method")]
    public void EqualsObject() => Assert.False(new CalendarAmount { Years = 1 }.Equals(1));

    [Fact]
    public void Add()
    {
        var halfDay = .5.Days();
        Assert.Equal(4, 3.5.Days().Add(halfDay).Days);
        var timeSpan = new TimeSpan(3, 12, 0, 0);
        Assert.Equal(4, timeSpan.AddCalendarAmount(halfDay).Days);
        Assert.Equal(4, (timeSpan + halfDay).Days);
        Assert.Equal(4, (halfDay + timeSpan).Days);
    }

    [Fact]
    public void Clone()
    {
        var amount = 3.Years();
        var clone = amount.Clone();
        Assert.Equal(amount, clone);
    }

    [Fact]
    public void Years()
    {
        var amount = 3.Years();
        Assert.Equal(3, amount.Years);
    }

    [Fact]
    public void EnsureWhenConvertedIsCorrect()
    {
        var timeSpan = 10.Years().ToApproximateTimeSpan();
        Assert.Equal(3650d, timeSpan.TotalDays);
    }

    [Fact]
    public void AddCalendarAmount_AppliesYearsBeforeMonths()
    {
        var start = new DateTime(2023, 1, 31);
        var result = start.AddCalendarAmount(new CalendarAmount { Years = 1, Months = 1 });

        Assert.Equal(new DateTime(2024, 2, 29), result);
    }
}
