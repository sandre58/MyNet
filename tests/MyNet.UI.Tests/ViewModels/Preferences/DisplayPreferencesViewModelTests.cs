// -----------------------------------------------------------------------
// <copyright file="DisplayPreferencesViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Moq;
using MyNet.UI.Theming;
using MyNet.UI.ViewModels.Preferences;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Preferences;

public class DisplayPreferencesViewModelTests
{
    [Fact]
    public void ThemeBase_WhenChanged_AppliesBaseTheme()
    {
        var light = new Mock<IThemeBase>();
        light.Setup(x => x.IsDark).Returns(false);
        var dark = new Mock<IThemeBase>();
        dark.Setup(x => x.IsDark).Returns(true);

        var currentBase = light.Object;
        var theme = new Mock<IThemeService>();
        theme.Setup(x => x.CurrentTheme).Returns(() => new(currentBase, "#111111", "#222222"));

        var registry = new Mock<IThemeBaseRegistry>();
        registry.Setup(x => x.AvailableBases).Returns([light.Object, dark.Object]);

        var sut = new DisplayPreferencesViewModel(theme.Object, registry.Object);

        sut.ThemeBase = dark.Object;

        theme.Verify(x => x.ApplyBaseTheme(dark.Object), Times.Once);
        sut.ThemeBase.Should().BeSameAs(dark.Object);
    }
}
