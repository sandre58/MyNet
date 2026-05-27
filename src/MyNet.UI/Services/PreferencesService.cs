// -----------------------------------------------------------------------
// <copyright file="PreferencesService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Collections;

namespace MyNet.UI.Services;

/// <summary>
/// Service that manages persistent user preferences by delegating to multiple settings groups.
/// Implements <see cref="IPersistentPreferencesService"/> and <see cref="IDisposable"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PreferencesService"/> class.
/// </remarks>
/// <param name="groups">The collection of settings groups to manage.</param>
public class PreferencesService(IEnumerable<IPersistentSettingsService> groups) : IPersistentPreferencesService, IDisposable
{
    private readonly IList<IPersistentSettingsService> _groups = [.. groups];
    private bool _disposedValue;

    /// <inheritdoc/>
    public void Reload() => _groups.ForEach(x => x.Reload());

    /// <inheritdoc/>
    public void Reset() => _groups.ForEach(x => x.Reset());

    /// <inheritdoc/>
    public void Save() => _groups.ForEach(x => x.Save());

    /// <summary>
    /// Releases resources used by the service and disposes all disposable settings groups.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;
        if (disposing)
        {
            _groups.OfType<IDisposable>().ForEach(x => x.Dispose());
        }

        _disposedValue = true;
    }

    /// <summary>
    /// Disposes the service and its managed resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
