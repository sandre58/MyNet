// -----------------------------------------------------------------------
// <copyright file="GlobalizationServicesTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using MyNet.Globalization.Facade;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Tests.Data;
using Xunit;

namespace MyNet.Globalization.Tests.Facade;

[Collection("UseCultureSequential")]
public class GlobalizationServicesTests
{
    [Fact]
    public void CurrentCulture()
    {
        GlobalizationServices.Current.SetCulture("fr-FR");

        // Core assertion: culture propagated to CultureInfo.CurrentCulture
        Assert.Equal("fr-FR", CultureInfo.CurrentCulture.Name);
    }

    [Fact]
    public void SetCulture()
    {
        GlobalizationServices.Current.SetCulture("en-US");
        Assert.Equal("en-US", CultureInfo.CurrentCulture.Name);
    }

    [Fact]
    public void CultureNeutral()
    {
        var attr = Assembly.GetExecutingAssembly().GetCustomAttributes<NeutralResourcesLanguageAttribute>().FirstOrDefault();
        Assert.NotNull(attr);

        GlobalizationServices.Current.SetCulture(attr.CultureName);
        Assert.Equal("Valeur Une", DataResources.Value1);
    }

    [Fact]
    public void CultureEn()
    {
        GlobalizationServices.Current.SetCulture("en");
        Assert.Equal("Value One", DataResources.Value1);
    }

    [Fact]
    public void CultureEs()
    {
        GlobalizationServices.Current.SetCulture("es-ES");
        Assert.Equal("Valor Uno", DataResources.Value1);
    }

    [Fact]
    public void GetString()
    {
        GlobalizationServices.Current.SetCulture("fr-FR");
        Assert.Equal("Valeur Une", Localizer.Translation.Translate(nameof(DataResources.Value1), TranslationOptionsPresets.Default));
    }

    [Fact]
    public void GetStringEs()
    {
        GlobalizationServices.Current.SetCulture("es-ES");
        Assert.Equal("Valor Uno", Localizer.Translation.Translate(nameof(DataResources.Value1), TranslationOptionsPresets.Default));
    }

    [Fact]
    public void GetStringItWithResources()
    {
        GlobalizationServices.Current.SetCulture("it-IT");
        Assert.Equal("Valeur Une", Localizer.Translation.Translate(nameof(DataResources.Value1), TranslationOptionsPresets.Default));
    }

    [Fact]
    public void PureTranslator_ExplicitCulture_DoesNotDependOnCurrentCulture()
    {
        // ITranslationService accepts an optional culture override — culture is always explicit here, independent of the "current" culture.
        GlobalizationServices.Current.SetCulture("fr-FR");

        var en = Localizer.Translation.Translate(nameof(DataResources.Value1), TranslationOptionsPresets.Default, new("en-US"));
        var fr = Localizer.Translation.Translate(nameof(DataResources.Value1), TranslationOptionsPresets.Default, new("fr-FR"));

        Assert.Equal("Value One", en);
        Assert.Equal("Valeur Une", fr);
    }
}
