// -----------------------------------------------------------------------
// <copyright file="UiBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization;
using MyNet.Humanizer;
using MyNet.UI.Dialogs;
using MyNet.UI.Loading;
using MyNet.UI.Locators;
using MyNet.UI.Locators.Conventions;
using MyNet.UI.Navigation;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Processors;
using MyNet.UI.Toasting;
using MyNet.UI.Toasting.Settings;
using MyNet.UI.ViewModels;

namespace MyNet.UI.Extensions;

/// <summary>
/// Configures optional UI registrations inside <c>AddUi(Action&lt;UiBuilder&gt;)</c>.
/// </summary>
public sealed class UiBuilder(IServiceCollection services)
{
    private Action<DialogServiceBuilder>? _configureDialogs;
    private Action<ITypeResolver>? _configureViewLocators;
    private Action<IServiceCollection>? _configureNavigation;
    private Action<IList<INotificationProcessor>>? _configureNotifications;
    private Action<ToastManagerOptions>? _configureToasting;
    private bool _shellPreferences;

    /// <summary>
    /// Gets the underlying service collection.
    /// </summary>
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// Sets cultures offered in shell chrome (e.g. status bar language selector).
    /// </summary>
    public UiBuilder WithSupportedCultures(IEnumerable<CultureInfo>? supportedCultures)
    {
        SupportedCultures = supportedCultures;
        return this;
    }

    /// <summary>
    /// Registers a platform dialog presenter, file dialog service, or other dialog overrides.
    /// </summary>
    public UiBuilder ConfigureDialogs(Action<DialogServiceBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _configureDialogs = configure;
        return this;
    }

    /// <summary>
    /// Registers manual ViewModel ↔ View mappings on the type resolver.
    /// </summary>
    public UiBuilder ConfigureViewLocators(Action<ITypeResolver> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _configureViewLocators = configure;
        return this;
    }

    /// <summary>
    /// Adds navigation guards or middleware after the core navigation stack is registered.
    /// </summary>
    public UiBuilder ConfigureNavigation(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _configureNavigation = configure;
        return this;
    }

    /// <summary>
    /// Customizes notification processors applied before publication.
    /// </summary>
    public UiBuilder ConfigureNotifications(Action<IList<INotificationProcessor>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _configureNotifications = configure;
        return this;
    }

    /// <summary>
    /// Customizes toast manager options.
    /// </summary>
    public UiBuilder ConfigureToasting(Action<ToastManagerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _configureToasting = configure;
        return this;
    }

    /// <summary>
    /// Registers preferences page view models used by the shell preferences workspace.
    /// </summary>
    public UiBuilder AddShellPreferences()
    {
        _shellPreferences = true;
        return this;
    }

    internal IEnumerable<CultureInfo>? SupportedCultures { get; private set; }

    internal void Apply()
    {
        Services.AddGlobalization()
            .AddLocalization()
            .AddInflection()
            .AddHumanizer()
            .AddBusy()
            .AddNavigation();

        _configureNavigation?.Invoke(Services);

        Services.AddViewLocators(_configureViewLocators)
            .AddDialogs(_configureDialogs)
            .AddNotifications(_configureNotifications)
            .AddToasting(_configureToasting)
            .AddShell(SupportedCultures)
            .AddUiTranslations();

        if (_shellPreferences)
            Services.AddShellPreferences();
    }
}
