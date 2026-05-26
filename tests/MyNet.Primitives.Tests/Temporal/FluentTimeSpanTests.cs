// -----------------------------------------------------------------------
// <copyright file="FluentTimeSpanTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public class FluentTimeSpanTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(32)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(0)]
    public void YearsMonthsWeeksDaysHoursMinutesSecondsMilliseconds(int value)
    {
        Assert.Equal(value.Years(), new()
        {
            Years = value
        });
        Assert.Equal(value.Months(), new()
        {
            Months = value
        });
        Assert.Equal(value.Weeks(), new()
        {
            TimeSpan = TimeSpan.FromDays(value * 7)
        });
        Assert.Equal(value.Days(), new()
        {
            TimeSpan = TimeSpan.FromDays(value)
        });

        Assert.Equal(value.Hours(), new()
        {
            TimeSpan = TimeSpan.FromHours(value)
        });
        Assert.Equal(value.Minutes(), new()
        {
            TimeSpan = TimeSpan.FromMinutes(value)
        });
        Assert.Equal(value.Seconds(), new()
        {
            TimeSpan = TimeSpan.FromSeconds(value)
        });
        Assert.Equal(value.Milliseconds(), new()
        {
            TimeSpan = TimeSpan.FromMilliseconds(value)
        });
        Assert.Equal(value.Ticks(), new()
        {
            TimeSpan = TimeSpan.FromTicks(value)
        });
    }

    [Fact]
    public void Subtract()
    {
        var halfDay = .5.Days();
        Assert.Equal(3, 3.5.Days().Subtract(halfDay).Days);
        var timeSpan = new TimeSpan(3, 12, 0, 0);
        Assert.Equal(3, timeSpan.SubtractFluentTimeSpan(halfDay).Days);
        Assert.Equal(3, (timeSpan - halfDay).Days);
        Assert.Equal(-3, (halfDay - timeSpan).Days);
    }

    [Fact]
    public void GetHashCodeTest() => Assert.Equal(343024320, 3.5.Days().GetHashCode());

    [Fact]
    public void CompareToFluentTimeSpan()
    {
        Assert.Equal(0, 3.Days().CompareTo(3.Days()));
        Assert.Equal(-1, 3.Days().CompareTo(4.Days()));
        Assert.Equal(1, 4.Days().CompareTo(3.Days()));
    }

    [Fact]
    public void CompareToTimeSpan()
    {
        Assert.Equal(0, 3.Days().CompareTo(TimeSpan.FromDays(3)));
        Assert.Equal(-1, 3.Days().CompareTo(TimeSpan.FromDays(4)));
        Assert.Equal(1, 4.Days().CompareTo(TimeSpan.FromDays(3)));
    }

    [Fact]
    public void CompareToObject()
    {
        Assert.Equal(0, 3.Days().CompareTo((object)TimeSpan.FromDays(3)));
        Assert.Equal(-1, 3.Days().CompareTo((object)TimeSpan.FromDays(4)));
        Assert.Equal(1, 4.Days().CompareTo((object)TimeSpan.FromDays(3)));
    }

    [Fact]
    public void EqualsFluentTimeSpan()
    {
        Assert.True(3.Days().Equals(3.Days()));
        Assert.False(4.Days().Equals(3.Days()));
    }

    [Fact]
    public void EqualsTimeSpan()
    {
        Assert.True(3.Days().Equals(TimeSpan.FromDays(3)));
        Assert.False(4.Days().Equals(TimeSpan.FromDays(3)));
    }

    [Fact]
    public void AreEquals() => Assert.False(3.Days().Equals(null));

    [Fact]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Testing Equals(object) method")]
    public void EqualsTimeSpanAsObject() => Assert.True(3.Days().Equals((object)TimeSpan.FromDays(3)));

    [Fact]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Testing Equals(object) method")]
    public void EqualsObject() => Assert.False(3.Days().Equals(1));

    [Fact]
    public void Add()
    {
        var halfDay = .5.Days();
        Assert.Equal(4, 3.5.Days().Add(halfDay).Days);
        var timeSpan = new TimeSpan(3, 12, 0, 0);
        Assert.Equal(4, timeSpan.AddFluentTimeSpan(halfDay).Days);
        Assert.Equal(4, (timeSpan + halfDay).Days);
        Assert.Equal(4, (halfDay + timeSpan).Days);
    }

    [Fact]
    public void ToStringTest() => Assert.Equal("3.12:00:00", 3.5.Days().ToString());

    [Fact]
    public void Clone()
    {
        var timeSpan = 3.Milliseconds();
        var clone = timeSpan.Clone();
        Assert.Equal(timeSpan, clone);
    }

    [Fact]
    public void Ticks() => Assert.Equal(30000, 3.Milliseconds().Ticks);

    [Fact]
    public void Milliseconds() => Assert.Equal(100, 1100.Milliseconds().Milliseconds);

    [Fact]
    public void TotalMilliseconds() => Assert.Equal(1100, 1100.Milliseconds().TotalMilliseconds);

    [Fact]
    public void Seconds() => Assert.Equal(1, 61.Seconds().Seconds);

    [Fact]
    public void TotalSeconds() => Assert.Equal(61, 61.Seconds().TotalSeconds);

    [Fact]
    public void Minutes() => Assert.Equal(1, 61.Minutes().Minutes);

    [Fact]
    public void TotalMinutes() => Assert.Equal(61, 61.Minutes().TotalMinutes);

    [Fact]
    public void Hours() => Assert.Equal(1, 25.Hours().Hours);

    [Fact]
    public void TotalHours() => Assert.Equal(25, 25.Hours().TotalHours);

    [Fact]
    public void Days() => Assert.Equal(366, 366.Days().Days);

    [Fact]
    public void TotalDays() => Assert.Equal(366, 366.Days().TotalDays);

    [Fact]
    public void Years()
    {
        var fluentTimeSpan = 3.Years();
        Assert.Equal(3, fluentTimeSpan.Years);
    }

    [Fact]
    public void EnsureWhenConvertedIsCorrect()
    {
        TimeSpan timeSpan = 10.Years();
        Assert.Equal(3650d, timeSpan.TotalDays);
    }
}
