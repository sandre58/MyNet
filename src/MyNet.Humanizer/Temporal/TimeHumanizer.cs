// -----------------------------------------------------------------------
// <copyright file="TimeHumanizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Formatting.Collections;
using MyNet.Humanizer.Resources;
using MyNet.Utilities;
using MyNet.Utilities.Temporal.Decomposition;

namespace MyNet.Humanizer.Temporal;

/// <summary>
/// Current implementation of <see cref="ITimeHumanizer"/> that provides culture-specific humanization of relative dates and durations.
/// Supports specialization through inheritance for cultures with complex grammatical rules.
/// </summary>
/// <param name="translationService">The translation service used to translate resource keys.</param>
/// <param name="listFormatter">The list formatter used to format lists of humanized time units.</param>
/// <param name="culture">The culture used for humanization and formatting.</param>
/// <param name="options">The options used for time localization resource keys.</param>
public sealed class TimeHumanizer(ITranslationService translationService, ICultureScopedServiceSource<IListFormatter> listFormatter, CultureInfo culture, TimeLocalizationOptions options)
    : TimeHumanizerBase(translationService, listFormatter, culture, options);

/// <summary>
/// Current implementation of ITimeHumanizer that uses a translation service and culture information to provide localized humanization of relative dates and durations.
/// </summary>
/// <param name="translationService">The translation service used to translate resource keys.</param>
/// <param name="listFormatter">The list formatter used to format lists of humanized time units.</param>
/// <param name="supportedCulture">The culture used to format the humanized strings.</param>
/// <param name="options">The options used for time localization.</param>
public abstract class TimeHumanizerBase(ITranslationService translationService, ICultureScopedServiceSource<IListFormatter> listFormatter, CultureInfo supportedCulture, TimeLocalizationOptions options) : ITimeHumanizer
{
    /// <summary>
    /// Gets the supported culture for this humanizer.
    /// </summary>
    public CultureInfo Culture => supportedCulture;

    /// <inheritdoc/>
    public virtual string HumanizeRelativeTime(int count, TimeUnit unit, Tense tense)
    {
        if (count == 0)
        {
            return translationService.Translate(tense == Tense.Past ? options.NeverKey : options.NowKey, TranslationOptionsPresets.Default, nameof(DateTimeResources), supportedCulture);
        }

        var key = GetRelativeDateKey(count, unit, tense);

        return translationService.Translate(key, new() { Quantity = count, UseInflectionFallback = false }, nameof(DateTimeResources), supportedCulture);
    }

    /// <inheritdoc/>
    public string HumanizeRelativeTime(IReadOnlyList<TimeUnitValue> values, Tense tense)
    {
        switch (values.Count)
        {
            case 0:
                return string.Empty;
            case 1:
                return HumanizeRelativeTime(values[0].Value, values[0].Unit, tense);
        }

        var key = GetMultipleRelativeDateKey(tense);

        var parts = values.Where(x => x.Value > 0).Select(v => HumanizeDuration(v.Value, v.Unit)).ToArray();

        var durationStr = listFormatter.Get(Culture).Format(parts);

        return translationService.Translate(key, new TranslationOptionsBuilder().WithArgument("duration", durationStr).WithoutInflectionFallback().Build(), nameof(DateTimeResources), supportedCulture);
    }

    /// <inheritdoc/>
    public virtual string HumanizeDuration(int count, TimeUnit unit)
    {
        if (count == 0)
        {
            // "no time" for tiny units, explicit "0 <unit>" for larger constrained units.
            return unit is TimeUnit.Minute or TimeUnit.Hour or TimeUnit.Day or TimeUnit.Week or TimeUnit.Month or TimeUnit.Year
                ? translationService.Translate(GetDurationKey(count, unit), new() { Quantity = count, UseInflectionFallback = false }, nameof(DateTimeResources), supportedCulture)
                : translationService.Translate(options.ZeroKey, TranslationOptionsPresets.Default, nameof(DateTimeResources), supportedCulture);
        }

        var resourceKey = GetDurationKey(count, unit);

        return translationService.Translate(resourceKey, new() { Quantity = count, UseInflectionFallback = false }, nameof(DateTimeResources), supportedCulture);
    }

    /// <inheritdoc/>
    public string HumanizeDuration(IReadOnlyList<TimeUnitValue> values, ListFormattingOptions? listFormattingOptions = null)
    {
        switch (values.Count)
        {
            case 0:
                return string.Empty;
            case 1:
                return HumanizeDuration(values[0].Value, values[0].Unit);
        }

        var key = GetMultipleDurationKey();

        var parts = values.Where(x => x.Value > 0).Select(v => HumanizeDuration(v.Value, v.Unit)).ToArray();

        var durationStr = listFormatter.Get(Culture).Format(parts, listFormattingOptions);

        return string.IsNullOrWhiteSpace(key)
            ? durationStr
            : translationService.Translate(key, new TranslationOptionsBuilder().WithArgument("duration", durationStr).WithoutInflectionFallback().Build(), nameof(DateTimeResources), supportedCulture);
    }

    /// <summary>
    /// Gets the resource key used for relative date humanization based on the time unit, count, and tense.
    /// Override this method for cultures with complex grammatical rules.
    /// </summary>
    /// <param name="count">The number of time units.</param>
    /// <param name="unit">The time unit.</param>
    /// <param name="tense">The tense of the time expression.</param>
    /// <returns>The resource key for relative date humanization.</returns>
    protected virtual string GetRelativeDateKey(int count, TimeUnit unit, Tense tense)
        => count == 1 && unit == TimeUnit.Day
            ? tense == Tense.Future ? options.TomorrowKey : options.YesterdayKey
            : string.Format(CultureInfo.InvariantCulture, options.RelativeDateKeyFormat, tense, unit);

    protected virtual string GetMultipleRelativeDateKey(Tense tense) => string.Format(CultureInfo.InvariantCulture, options.RelativeDateKeyFormat, tense, string.Empty);

    /// <summary>
    /// Gets the resource key used for duration humanization.
    /// Override this method for cultures with complex grammatical rules.
    /// </summary>
    /// <param name="count">The number of time units.</param>
    /// <param name="unit">The time unit.</param>
    /// <returns>The resource key for duration humanization.</returns>
    protected virtual string GetDurationKey(int count, TimeUnit unit) => string.Format(CultureInfo.InvariantCulture, options.DurationKeyFormat, unit);

    /// <summary>
    /// Gets the resource key used for multiple duration humanization.
    /// Override this method for cultures with complex grammatical rules.
    /// </summary>
    /// <returns>The resource key for multiple duration humanization.</returns>
    protected virtual string GetMultipleDurationKey() => string.Empty;
}
