// -----------------------------------------------------------------------
// <copyright file="DateTimeHelperModernTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Primitives.Temporal;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public sealed class DateTimeHelperModernTests
{
    [Fact]
    public void Constants_AreExpected()
    {
        Assert.Equal(7, DateTimeHelper.DaysPerWeek);
        Assert.Equal(6, DateTimeHelper.MaxNumberOfWeeksPerMonth);
        Assert.Equal(4, DateTimeHelper.MinNumberOfWeeksPerMonth);
        Assert.True(DateTimeHelper.DaysPerYear > 365d);
        Assert.True(DateTimeHelper.DaysPerMonth > 30d);
    }

    [Fact]
    public void GetCurrentDateTimeFormatInfo_ReturnsGregorianCalendar()
    {
        var format = DateTimeHelper.GetCurrentDateTimeFormatInfo();

        Assert.NotNull(format);
        Assert.IsType<GregorianCalendar>(format.Calendar);
    }
}
