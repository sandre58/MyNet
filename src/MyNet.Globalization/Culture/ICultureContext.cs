// -----------------------------------------------------------------------
// <copyright file="ICultureContext.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Culture;

/// <summary>
/// Provides read-only access to the application's current culture.
/// Represents the culture at the <b>application level</b> — the culture the application is currently
/// configured to operate in — as opposed to the culture of an individual thread.
/// <para>
/// In DI scenarios, this interface is implemented by <see cref="ICultureService"/>
/// (registered by <c>AddGlobalization()</c>), which holds the application-level culture and
/// raises change notifications when the culture is switched at runtime.
/// </para>
/// <para>
/// When the full globalization stack is not needed (e.g. unit tests, non-DI code),
/// a thread-based fallback reads <see cref="CultureInfo.CurrentCulture"/>.
/// </para>
/// Use this interface when a component only needs to <b>read</b> the current culture, not change it.
/// To change the application culture, use <see cref="ICultureService"/>.
/// </summary>
public interface ICultureContext
{
    /// <summary>
    /// Gets the application's current culture.
    /// </summary>
    CultureInfo CurrentCulture { get; }
}
