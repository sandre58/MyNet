// -----------------------------------------------------------------------
// <copyright file="ShellThemeViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Moq;
using MyNet.UI.Theming;
using MyNet.UI.ViewModels.Shell.Chrome;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class ShellThemeViewModelTests
{
    [Fact]
    public void IsDark_WhenSet_AppliesDarkBaseTheme()
    {
        var lightBase = new Mock<IThemeBase>();
        lightBase.Setup(x => x.IsDark).Returns(false);
        var darkBase = new Mock<IThemeBase>();
        darkBase.Setup(x => x.IsDark).Returns(true);

        var themeService = new Mock<IThemeService>();
        themeService.Setup(x => x.CurrentTheme).Returns(new Theme(lightBase.Object, "#000000", "#000000"));

        var registry = new Mock<IThemeBaseRegistry>();
        registry.Setup(x => x.Light).Returns(lightBase.Object);
        registry.Setup(x => x.Dark).Returns(darkBase.Object);

        var sut = new ShellThemeViewModel(themeService.Object, registry.Object);

        sut.IsDark = true;

        themeService.Verify(x => x.ApplyBaseTheme(darkBase.Object), Times.Once);
    }
}
