// -----------------------------------------------------------------------
// <copyright file="ThemeManagerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Theming;
using Xunit;

namespace MyNet.UI.Tests.Theming;

public class ThemeManagerTests : IDisposable
{
    public ThemeManagerTests() => ThemeManager.ResetForTesting();

    public void Dispose() => ThemeManager.ResetForTesting();

    [Fact]
    public void ApplyBase_before_configure_throws()
    {
        var act = () => ThemeManager.ApplyBase(new FakeThemeBase("Dark", true, false));

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Configure_applies_pending_theme_changed_handlers()
    {
        var service = new FakeThemeService();
        var registry = new FakeThemeBaseRegistry();
        var raised = false;

        ThemeManager.ThemeChanged += (_, _) => raised = true;
        ThemeManager.Configure(service, registry);

        service.RaiseThemeChanged();

        raised.Should().BeTrue();
    }

    [Fact]
    public void UseThemeManager_configures_bridge()
    {
        var service = new FakeThemeService();
        var registry = new FakeThemeBaseRegistry();
        var provider = new ServiceCollection()
            .AddSingleton<IThemeService>(service)
            .AddSingleton<IThemeBaseRegistry>(registry)
            .BuildServiceProvider();

        provider.UseThemeManager();

        ThemeManager.IsConfigured.Should().BeTrue();
        ThemeManager.CurrentTheme.Should().BeSameAs(service.CurrentTheme);
    }

    private sealed class FakeThemeBase(string name, bool isDark, bool isHighContrast) : IThemeBase
    {
        public string Name { get; } = name;

        public bool IsDark { get; } = isDark;

        public bool IsHighContrast { get; } = isHighContrast;
    }

    private sealed class FakeThemeBaseRegistry : IThemeBaseRegistry
    {
        private readonly FakeThemeBase _dark = new("Dark", true, false);
        private readonly FakeThemeBase _light = new("Light", false, false);
        private readonly FakeThemeBase _highContrast = new("HighContrast", true, true);

        public IThemeBase Light => _light;

        public IThemeBase Dark => _dark;

        public IThemeBase HighContrast => _highContrast;

        public IReadOnlyCollection<IThemeBase> AvailableBases => [_dark, _light, _highContrast];

        public void Register(IThemeBase themeBase)
        {
        }

        public IThemeBase? Get(string name) => name switch
        {
            "Dark" => _dark,
            "Light" => _light,
            "HighContrast" => _highContrast,
            _ => null
        };
    }

    private sealed class FakeThemeService : IThemeService
    {
        private Theme _current = new(new FakeThemeBase("Light", false, false), "#000000", "#FFFFFF");

        public Theme CurrentTheme => _current;

        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        public void ApplyTheme(Theme theme) => _current = theme;

        public void ApplyBaseTheme(IThemeBase baseTheme) => _current = _current with { Base = baseTheme };

        public void ApplyPrimary(string color, string? foreground = null)
            => _current = _current with { PrimaryColor = color, PrimaryForegroundColor = foreground };

        public void ApplyAccent(string color, string? foreground = null)
            => _current = _current with { AccentColor = color, AccentForegroundColor = foreground };

        public void UpdateTheme(Func<Theme, Theme> update) => _current = update(_current);

        public IThemeService AddBaseExtension(IThemeExtension extension) => this;

        public IThemeService AddPrimaryExtension(IThemeExtension extension) => this;

        public IThemeService AddAccentExtension(IThemeExtension extension) => this;

        public void RaiseThemeChanged() => ThemeChanged?.Invoke(this, new(_current));
    }
}
