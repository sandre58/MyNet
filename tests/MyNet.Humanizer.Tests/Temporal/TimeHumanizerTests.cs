// -----------------------------------------------------------------------
// <copyright file="TimeHumanizerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Formatting.Collections;
using MyNet.Humanizer.Static;
using MyNet.Humanizer.Temporal;
using MyNet.Utilities;
using Xunit;

namespace MyNet.Humanizer.Tests.Temporal;

[Collection("UseCultureSequential")]
public class TimeHumanizerTests
{
    [Fact]
    public void TimeHumanizer_RelativeTime_Works()
    {
        var humanizer = CreateTimeHumanizer(new("en-US"));

        // Test relative time humanization
        var result = humanizer.HumanizeRelativeTime(1, TimeUnit.Day, Tense.Past);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void TimeHumanizer_ZeroQuantity_ReturnsNow()
    {
        var humanizer = CreateTimeHumanizer(new("en-US"));

        var result = humanizer.HumanizeRelativeTime(0, TimeUnit.Second, Tense.Future);

        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(Tense.Past)]
    [InlineData(Tense.Future)]
    public void TimeHumanizer_BothTenses_Work(Tense tense)
    {
        var humanizer = CreateTimeHumanizer(new("en-US"));

        var result = humanizer.HumanizeRelativeTime(5, TimeUnit.Hour, tense);

        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(TimeUnit.Second)]
    [InlineData(TimeUnit.Minute)]
    [InlineData(TimeUnit.Hour)]
    [InlineData(TimeUnit.Day)]
    [InlineData(TimeUnit.Month)]
    [InlineData(TimeUnit.Year)]
    public void TimeHumanizer_AllTimeUnits_Work(TimeUnit unit)
    {
        var humanizer = CreateTimeHumanizer(new("en-US"));

        var result = humanizer.HumanizeRelativeTime(1, unit, Tense.Past);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void ListFormatterAndTimeHumanizer_Combined_Work()
    {
        var listFormatter = CreateListFormatter(new("en-US"));
        var timeHumanizer = CreateTimeHumanizer(new("en-US"));

        // Use list formatter to format items
        var formattedList = listFormatter.Format(["task1", "task2", "task3"]);
        Assert.NotEmpty(formattedList);

        // Use time humanizer
        var humanizedTime = timeHumanizer.HumanizeRelativeTime(2, TimeUnit.Hour, Tense.Past);
        Assert.NotEmpty(humanizedTime);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void TimeHumanizer_VariousQuantities_AllWork(int quantity)
    {
        var humanizer = CreateTimeHumanizer(new("en-US"));

        var result = humanizer.HumanizeRelativeTime(quantity, TimeUnit.Day, Tense.Past);

        Assert.NotEmpty(result);
    }

    private static ListFormatter CreateListFormatter(CultureInfo culture)
    {
        // Create a simple translator implementation
        var translator = new SimpleTranslator();
        var translationService = new TranslationService(translator, new FixedCultureContext(culture));
        return new(translationService, culture);
    }

    private static TimeHumanizer CreateTimeHumanizer(CultureInfo culture)
    {
        var translator = new SimpleTranslator();
        var translationService = new TranslationService(translator, new FixedCultureContext(culture));
        var listFormatter = new ListFormatter(translationService, culture);
        var options = new TimeLocalizationOptions();
        return new(translationService, new StubListFormatterSource(listFormatter), culture, options);
    }

    /// <summary>
    /// Simple translator mock for testing humanizers without full translation setup.
    /// </summary>
    private sealed class SimpleTranslator : ITranslator
    {
        public string Translate(string key, TranslationOptions options, CultureInfo culture) => key;

        public string Translate(string key, TranslationOptions options, CultureInfo culture, string resourceKey) => key;
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }

    private sealed class StubListFormatterSource(IListFormatter formatter) : ICultureScopedServiceSource<IListFormatter>
    {
        public IListFormatter Get(CultureInfo? culture = null) => formatter;

        public bool TryGet(CultureInfo? culture, [NotNullWhen(true)] out IListFormatter? service)
        {
            service = formatter;
            return true;
        }
    }
}

[UseCulture(Culture)]
[Collection("UseCultureSequential")]
public class TimeHumanizerEnTests
{
    public const string Culture = "en-US";

    [Theory]
    [InlineData(1, "1 second ago")]
    [InlineData(10, "10 seconds ago")]
    [InlineData(59, "59 seconds ago")]
    [InlineData(60, "1 minute ago")]
    public void SecondsAgo(int seconds, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, seconds, TimeUnit.Second, Tense.Past);

    [Theory]
    [InlineData(1, "1 second from now")]
    [InlineData(10, "10 seconds from now")]
    [InlineData(59, "59 seconds from now")]
    [InlineData(60, "1 minute from now")]
    public void SecondsFromNow(int seconds, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, seconds, TimeUnit.Second, Tense.Future);

    [Theory]
    [InlineData(1, "1 minute ago")]
    [InlineData(10, "10 minutes ago")]
    [InlineData(44, "44 minutes ago")]
    [InlineData(45, "45 minutes ago")]
    [InlineData(59, "59 minutes ago")]
    [InlineData(60, "1 hour ago")]
    [InlineData(119, "1 hour ago")]
    [InlineData(120, "2 hours ago")]
    public void MinutesAgo(int minutes, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, minutes, TimeUnit.Minute, Tense.Past);

    [Theory]
    [InlineData(1, "1 minute from now")]
    [InlineData(10, "10 minutes from now")]
    [InlineData(44, "44 minutes from now")]
    [InlineData(45, "45 minutes from now")]
    [InlineData(119, "1 hour from now")]
    [InlineData(120, "2 hours from now")]
    public void MinutesFromNow(int minutes, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, minutes, TimeUnit.Minute, Tense.Future);

    [Theory]
    [InlineData(1, "1 hour ago")]
    [InlineData(10, "10 hours ago")]
    [InlineData(23, "23 hours ago")]
    [InlineData(24, "yesterday")]
    public void HoursAgo(int hours, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, hours, TimeUnit.Hour, Tense.Past);

    [Theory]
    [InlineData(1, "1 hour from now")]
    [InlineData(10, "10 hours from now")]
    [InlineData(23, "23 hours from now")]
    [InlineData(24, "tomorrow")]
    public void HoursFromNow(int hours, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, hours, TimeUnit.Hour, Tense.Future);

    [Theory]
    [InlineData(1, "yesterday")]
    [InlineData(10, "1 week ago")]
    [InlineData(27, "3 weeks ago")]
    [InlineData(32, "1 month ago")]
    public void DaysAgo(int days, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, days, TimeUnit.Day, Tense.Past);

    [Theory]
    [InlineData(1, "tomorrow")]
    [InlineData(10, "1 week from now")]
    [InlineData(27, "3 weeks from now")]
    [InlineData(32, "1 month from now")]
    public void DaysFromNow(int days, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, days, TimeUnit.Day, Tense.Future);

    [Theory]
    [InlineData(1, "4 weeks ago")]
    [InlineData(10, "9 months ago")]
    [InlineData(11, "11 months ago")]
    [InlineData(12, "11 months ago")]
    public void MonthsAgo(int months, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, months, TimeUnit.Month, Tense.Past);

    [Theory]
    [InlineData(1, "4 weeks from now")]
    [InlineData(10, "9 months from now")]
    [InlineData(11, "11 months from now")]
    [InlineData(12, "11 months from now")]
    public void MonthsFromNow(int months, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, months, TimeUnit.Month, Tense.Future);

    [Theory]
    [InlineData(1, "11 months ago")]
    [InlineData(2, "1 year ago")]
    public void YearsAgo(int years, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, years, TimeUnit.Year, Tense.Past);

    [Theory]
    [InlineData(1, "11 months from now")]
    [InlineData(2, "1 year from now")]
    public void YearsFromNow(int years, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, years, TimeUnit.Year, Tense.Future);

    [Theory]
    [InlineData(1, TimeUnit.Year, Tense.Future, "en-US", "11 months from now")]
    [InlineData(40, TimeUnit.Second, Tense.Past, "fr-FR", "il y a 40 secondes")]
    public void CanSpecifyCultureExplicitly(int unit, TimeUnit timeUnit, Tense tense, string culture, string expected) => TimeHumanizerTestsHelper.Verify(expected, culture, unit, timeUnit, tense, culture: new(culture));
}

[UseCulture(Culture)]
[Collection("UseCultureSequential")]
public class TimeHumanizerExtensionsFrTests
{
    public const string Culture = "fr-FR";

    [Theory]
    [InlineData(1, "il y a 1 seconde")]
    [InlineData(2, "il y a 2 secondes")]
    [InlineData(10, "il y a 10 secondes")]
    public void SecondsAgo(int seconds, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, seconds, TimeUnit.Second, Tense.Past);

    [Theory]
    [InlineData(1, "dans 1 seconde")]
    [InlineData(2, "dans 2 secondes")]
    [InlineData(10, "dans 10 secondes")]
    public void SecondsFromNow(int seconds, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, seconds, TimeUnit.Second, Tense.Future);

    [Theory]
    [InlineData(1, "il y a 1 minute")]
    [InlineData(2, "il y a 2 minutes")]
    [InlineData(10, "il y a 10 minutes")]
    [InlineData(60, "il y a 1 heure")]
    public void MinutesAgo(int minutes, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, minutes, TimeUnit.Minute, Tense.Past);

    [Theory]
    [InlineData(1, "dans 1 minute")]
    [InlineData(2, "dans 2 minutes")]
    [InlineData(10, "dans 10 minutes")]
    public void MinutesFromNow(int minutes, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, minutes, TimeUnit.Minute, Tense.Future);

    [Theory]
    [InlineData(1, "il y a 1 heure")]
    [InlineData(2, "il y a 2 heures")]
    [InlineData(10, "il y a 10 heures")]
    public void HoursAgo(int hours, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, hours, TimeUnit.Hour, Tense.Past);

    [Theory]
    [InlineData(1, "dans 1 heure")]
    [InlineData(2, "dans 2 heures")]
    [InlineData(10, "dans 10 heures")]
    public void HoursFromNow(int hours, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, hours, TimeUnit.Hour, Tense.Future);

    [Theory]
    [InlineData(1, "hier")]
    [InlineData(2, "il y a 2 jours")]
    [InlineData(10, "il y a 1 semaine")]
    public void DaysAgo(int days, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, days, TimeUnit.Day, Tense.Past);

    [Theory]
    [InlineData(1, "demain")]
    [InlineData(2, "dans 2 jours")]
    [InlineData(10, "dans 1 semaine")]
    public void DaysFromNow(int days, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, days, TimeUnit.Day, Tense.Future);

    [Theory]
    [InlineData(1, "il y a 4 semaines")]
    [InlineData(2, "il y a 2 mois")]
    [InlineData(10, "il y a 9 mois")]
    public void MonthsAgo(int months, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, months, TimeUnit.Month, Tense.Past);

    [Theory]
    [InlineData(1, "dans 4 semaines")]
    [InlineData(2, "dans 2 mois")]
    [InlineData(10, "dans 9 mois")]
    public void MonthsFromNow(int months, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, months, TimeUnit.Month, Tense.Future);

    [Theory]
    [InlineData(1, "il y a 11 mois")]
    [InlineData(2, "il y a 1 an")]
    public void YearsAgo(int years, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, years, TimeUnit.Year, Tense.Past);

    [Theory]
    [InlineData(1, "dans 11 mois")]
    [InlineData(2, "dans 1 an")]
    public void YearsFromNow(int years, string expected) => TimeHumanizerTestsHelper.Verify(expected, Culture, years, TimeUnit.Year, Tense.Future);
}

internal static class TimeHumanizerTestsHelper
{
    private static readonly Lock LockObject = new();

    public static void Verify(string expectedString, string expectedCultureName, int unit, TimeUnit timeUnit, Tense tense, CultureInfo? culture = null)
    {
        // We lock this as these tests can be multi-threaded and we're setting a static
        lock (LockObject)
        {
            unit = Math.Abs(unit);

            Assert.Equal(expectedCultureName, culture?.Name ?? CultureInfo.CurrentCulture.Name);

            var result = unit.ToTimeSpan(timeUnit).Humanize(
                new TimeHumanizationOptions
                {
                    Mode = TimeHumanizationMode.Relative,
                    Tense = tense,
                    MaxComponents = 1
                },
                culture);
            Assert.Equal(expectedString, result);
        }
    }
}
