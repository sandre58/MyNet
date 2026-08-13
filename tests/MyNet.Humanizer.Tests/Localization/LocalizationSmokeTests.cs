// -----------------------------------------------------------------------
// <copyright file="LocalizationSmokeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Facade;
using MyNet.Globalization.Localization.Translation;
using Xunit;

namespace MyNet.Humanizer.Tests.Localization;

[Collection("UseCultureSequential")]
public class LocalizationSmokeTests
{
    [Fact]
    public void RegisteredListResources_CanTranslateAndSeparator()
    {
        const string resourceKey = "ListResources";
        var value = Localizer.Translation.Translate("AndSeparator", TranslationOptionsPresets.Default, resourceKey, new CultureInfo("en-US"));

        Assert.Equal(" and ", value);
    }
}
