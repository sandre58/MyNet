// -----------------------------------------------------------------------
// <copyright file="GlobalizationServiceProxy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;

namespace MyNet.Globalization.Facade;

/// <summary>
/// Stable <see cref="IGlobalizationService"/> facade whose target can be swapped at startup.
/// </summary>
internal sealed class GlobalizationServiceProxy : IGlobalizationService
{
    private IGlobalizationService _inner;

    private event EventHandler<CultureChangedEventArgs>? CultureChangedInternal;

    private event EventHandler<TimeZoneChangedEventArgs>? TimeZoneChangedInternal;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalizationServiceProxy"/> class.
    /// </summary>
    /// <param name="inner">Initial globalization service implementation.</param>
    public GlobalizationServiceProxy(IGlobalizationService inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        WireInnerEvents();
    }

    /// <inheritdoc />
    public CultureInfo CurrentCulture => _inner.CurrentCulture;

    /// <inheritdoc />
    public TimeZoneInfo CurrentTimeZone => _inner.CurrentTimeZone;

    /// <inheritdoc />
    public System.DateTime Now => _inner.Now;

    /// <inheritdoc />
    public event EventHandler<CultureChangedEventArgs> CultureChanged
    {
        add => CultureChangedInternal += value;
        remove => CultureChangedInternal -= value;
    }

    /// <inheritdoc />
    public event EventHandler<TimeZoneChangedEventArgs> TimeZoneChanged
    {
        add => TimeZoneChangedInternal += value;
        remove => TimeZoneChangedInternal -= value;
    }

    /// <summary>
    /// Replaces the forwarded globalization service implementation.
    /// </summary>
    /// <param name="target">The configured globalization service.</param>
    public void SetTarget(IGlobalizationService target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (ReferenceEquals(_inner, target))
            return;

        UnwireInnerEvents();
        _inner = target;
        WireInnerEvents();
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) => _inner.SetCulture(culture);

    /// <inheritdoc />
    public void SetCulture(string cultureCode) => _inner.SetCulture(cultureCode);

    /// <inheritdoc />
    public void SetTimeZone(TimeZoneInfo timeZone) => _inner.SetTimeZone(timeZone);

    /// <inheritdoc />
    public void SetTimeZone(string timeZoneId) => _inner.SetTimeZone(timeZoneId);

    /// <inheritdoc />
    public DateTimeOffset FromUtc(DateTimeOffset utcDateTime) => _inner.FromUtc(utcDateTime);

    /// <inheritdoc />
    public DateTimeOffset ToUtc(DateTimeOffset localDateTime) => _inner.ToUtc(localDateTime);

    /// <inheritdoc />
    public DateTimeOffset Convert(DateTimeOffset dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
        => _inner.Convert(dateTime, sourceTimeZone, destinationTimeZone);

    /// <inheritdoc />
    public IDisposable UseCulture(CultureInfo culture) => _inner.UseCulture(culture);

    /// <inheritdoc />
    public IDisposable UseTimeZone(TimeZoneInfo timeZone) => _inner.UseTimeZone(timeZone);

    private void WireInnerEvents()
    {
        _inner.CultureChanged += ForwardCultureChanged;
        _inner.TimeZoneChanged += ForwardTimeZoneChanged;
    }

    private void UnwireInnerEvents()
    {
        _inner.CultureChanged -= ForwardCultureChanged;
        _inner.TimeZoneChanged -= ForwardTimeZoneChanged;
    }

    private void ForwardCultureChanged(object? sender, CultureChangedEventArgs e) => CultureChangedInternal?.Invoke(this, e);

    private void ForwardTimeZoneChanged(object? sender, TimeZoneChangedEventArgs e) => TimeZoneChangedInternal?.Invoke(this, e);
}
