// -----------------------------------------------------------------------
// <copyright file="TimeHumanizerOptionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Formatting.Collections;
using MyNet.Humanizer.Temporal;
using Xunit;

namespace MyNet.Humanizer.Tests.Temporal;

public class TimeHumanizerOptionsTests
{
    [Fact]
    public void TimeHumanizer_CanBeCreatedWithValidArguments()
    {
        var translator = new NullTranslator();
        var translationService = new TranslationService(translator, new FixedCultureContext(new("en-US")));
        var formatter = new NullListFormatter();
        var culture = CultureInfo.GetCultureInfo("en-US");
        var localizationOptions = new TimeLocalizationOptions();

        var humanizer = new TimeHumanizer(translationService, new StubListFormatterSource(formatter), culture, localizationOptions);

        Assert.NotNull(humanizer);
        Assert.Equal(culture, humanizer.Culture);
    }

    [Fact]
    public void TimeHumanizationOptions_DefaultModeIsRelative()
    {
        var options = new TimeHumanizationOptions();

        Assert.Equal(TimeHumanizationMode.Duration, options.Mode);
    }

    [Fact]
    public void TimeHumanizerBase_SupportsInheritance()
    {
        var translator = new NullTranslator();
        var formatter = new NullListFormatter();
        var translationService = new TranslationService(translator, new FixedCultureContext(new("en-US")));
        var culture = CultureInfo.GetCultureInfo("en-US");
        var localizationOptions = new TimeLocalizationOptions();

        var baseHumanizer = new TimeHumanizer(translationService, new StubListFormatterSource(formatter), culture, localizationOptions);

        Assert.NotNull(baseHumanizer);
        Assert.IsType<ITimeHumanizer>(baseHumanizer, exactMatch: false);
    }

    private sealed class NullTranslator : ITranslator
    {
        public string Translate(string key, TranslationOptions options, CultureInfo culture) => key;

        public string Translate(string key, TranslationOptions options, CultureInfo culture, string resourceKey) => key;
    }

    private sealed class NullListFormatter : IListFormatter
    {
        public CultureInfo Culture => CultureInfo.InvariantCulture;

        public string Format(IEnumerable<string?>? items, ListFormattingOptions? options = null) => string.Join(options?.Separator ?? ", ", items ?? []);
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
