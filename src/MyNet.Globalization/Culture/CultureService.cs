// -----------------------------------------------------------------------
// <copyright file="CultureService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;

namespace MyNet.Globalization.Culture;

/// <summary>
/// Provides services for managing the current culture of the application.
/// Implements both <see cref="ICultureService"/> (read + write) and <see cref="ICultureContext"/> (read-only).
/// When the culture changes, <see cref="CultureChanged"/> is raised outside the internal lock to prevent
/// deadlocks in subscribers, and <see cref="CultureInfo.CurrentCulture" /> /
/// <see cref="CultureInfo.CurrentUICulture" /> of the calling thread are updated automatically.
/// </summary>
public sealed class CultureService(CultureInfo initialCulture) : ICultureService
{
    private readonly Lock _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureService"/> class with the current culture of the application.
    /// </summary>
    public CultureService()
        : this(CultureInfo.CurrentCulture) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureService"/> class with the default culture specified in the provided <see cref="GlobalizationOptions"/>.
    /// </summary>
    public CultureService(GlobalizationOptions options)
        : this(options.DefaultCulture) { }

    /// <inheritdoc />
    public CultureInfo CurrentCulture { get; private set; } = initialCulture;

    /// <inheritdoc />
    public event EventHandler<CultureChangedEventArgs>? CultureChanged;

    /// <inheritdoc />
    /// <remarks>
    /// This method is thread-safe. The <see cref="CultureChanged"/> event is raised <b>outside</b> the internal lock.
    /// <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/> of the calling
    /// thread are updated so that standard .NET formatting APIs reflect the new culture immediately.
    /// </remarks>
    public void SetCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        CultureChangedEventArgs? args;
        lock (_lockObject)
        {
            if (Equals(CurrentCulture, culture)) return;

            var old = CurrentCulture;
            CurrentCulture = culture;
            args = new(old, culture);
        }

        // Update the calling thread's culture so that formatting APIs (e.g. ResourceManager, string.Format)
        // reflect the change without requiring callers to pass the culture explicitly.
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        // Raise event outside the lock to prevent deadlocks when subscribers call back into this service.
        CultureChanged?.Invoke(this, args);
    }

    /// <inheritdoc />
    public void SetCulture(string cultureCode) => SetCulture(CultureInfo.GetCultureInfo(cultureCode));
}
