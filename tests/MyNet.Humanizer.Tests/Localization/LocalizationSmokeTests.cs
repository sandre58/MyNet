// -----------------------------------------------------------------------
// <copyright file="LocalizationSmokeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Static;
using Xunit;

namespace MyNet.Humanizer.Tests.Localization;

[Collection("UseCultureSequential")]
public class LocalizationSmokeTests
{
    [Fact]
    public void RegisteredListResources_CanTranslateAndSeparator()
    {
        const string resourceKey = "ListResources";
        var value = Localizer.Translation.Translate("AndSeparator", TranslationOptionsPresets.Default, resourceKey, new("en-US"));

        Assert.Equal(" and ", value);
    }
}
