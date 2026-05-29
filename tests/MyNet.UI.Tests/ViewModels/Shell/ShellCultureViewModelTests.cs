// -----------------------------------------------------------------------
// <copyright file="ShellCultureViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.Globalization.Culture;
using MyNet.UI.ViewModels.Shell;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class ShellCultureViewModelTests
{
    [Fact]
    public void Constructor_SyncsSelectedCultureFromService()
    {
        var cultureService = new CultureService(SupportedCultures.English);
        var sut = new ShellCultureViewModel(cultureService, [SupportedCultures.English, SupportedCultures.French]);

        sut.SelectedCulture.Should().Be(SupportedCultures.English);
        sut.Cultures.Should().HaveCount(2);
    }

    [Fact]
    public void SelectedCulture_WhenSet_UpdatesCultureService()
    {
        var cultureService = new CultureService(SupportedCultures.English);
        var sut = new ShellCultureViewModel(cultureService, [SupportedCultures.English, SupportedCultures.French]);

        sut.SelectedCulture = SupportedCultures.French;

        cultureService.CurrentCulture.Should().Be(SupportedCultures.French);
    }
}
