// -----------------------------------------------------------------------
// <copyright file="DateTimeHumanize.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using MyNet.Humanizer.DateTimes;
using MyNet.Utilities.Units;
using Xunit;

namespace MyNet.Humanizer.UnitTests;

internal static class DateTimeHumanize
{
    private static readonly Lock LockObject = new();

    public static void Verify(string expectedString, string expectedCultureName, int unit, TimeUnit timeUnit, Tense tense, CultureInfo? culture = null, DateTime? baseDate = null, DateTime? baseDateUtc = null)
    {
        // We lock this as these tests can be multi-threaded and we're setting a static
        lock (LockObject)
        {
            var deltaFromNow = TimeSpan.Zero;
            unit = Math.Abs(unit);

            if (tense == Tense.Past)
                unit = -unit;

            deltaFromNow = timeUnit switch
            {
                TimeUnit.Millisecond => TimeSpan.FromMilliseconds(unit),
                TimeUnit.Second => TimeSpan.FromSeconds(unit),
                TimeUnit.Minute => TimeSpan.FromMinutes(unit),
                TimeUnit.Hour => TimeSpan.FromHours(unit),
                TimeUnit.Day => TimeSpan.FromDays(unit),
                TimeUnit.Month => TimeSpan.FromDays(unit * 30),
                TimeUnit.Year => TimeSpan.FromDays(unit * 365),
                TimeUnit.Week => TimeSpan.FromDays(unit * 7),
                _ => deltaFromNow
            };

            if (baseDate == null)
            {
                VerifyWithCurrentDate(expectedString, expectedCultureName, deltaFromNow, culture);
                VerifyWithDateInjection(expectedString, expectedCultureName, deltaFromNow, culture);
            }
            else
            {
                VerifyWithDate(expectedString, expectedCultureName, deltaFromNow, culture, baseDate, baseDateUtc);
            }
        }
    }

    private static void VerifyWithCurrentDate(string expectedString, string expectedCultureName, TimeSpan deltaFromNow, CultureInfo? culture)
    {
        var utcNow = DateTime.UtcNow;
        var localNow = DateTime.Now;

        // feels like the only way to avoid breaking tests because CPU ticks over is to inject the base date
        VerifyWithDate(expectedString, expectedCultureName, deltaFromNow, culture, localNow, utcNow);
    }

    private static void VerifyWithDateInjection(string expectedString, string expectedCultureName, TimeSpan deltaFromNow, CultureInfo? culture)
    {
        var utcNow = new DateTime(2013, 6, 20, 9, 58, 22, DateTimeKind.Utc);
        var now = new DateTime(2013, 6, 20, 11, 58, 22, DateTimeKind.Local);

        VerifyWithDate(expectedString, expectedCultureName, deltaFromNow, culture, now, utcNow);
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static void VerifyWithDate(string expectedString, string expectedCultureName, TimeSpan deltaFromBase, CultureInfo? culture, DateTime? baseDate, DateTime? baseDateUtc)
    {
        Assert.Equal(expectedCultureName, culture?.Name ?? CultureInfo.CurrentCulture.Name);

        Assert.Equal(expectedString, culture == null ? baseDateUtc?.Add(deltaFromBase).Humanize(dateToCompareAgainst: baseDateUtc, utcDate: true) : baseDateUtc?.Add(deltaFromBase).Humanize(dateToCompareAgainst: baseDateUtc, utcDate: true, culture: culture));
        Assert.Equal(expectedString, baseDate?.Add(deltaFromBase).Humanize(baseDate, utcDate: false, culture: culture));
    }
}
