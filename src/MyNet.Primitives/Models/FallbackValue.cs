// -----------------------------------------------------------------------
// <copyright file="FallbackValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Represents a value that can be obtained from a primary source, but falls back to an alternative source if the primary is unavailable. This class allows for an optional override value that takes precedence over both the primary and fallback sources when set. The fallback function is invoked only when the primary source does not provide a value, ensuring efficient retrieval of the value while maintaining flexibility in sourcing.
/// </summary>
/// <param name="fallback">The fallback source function.</param>
/// <typeparam name="T">The type of the value.</typeparam>
public class FallbackValue<T>(Func<T?> fallback)
{
    private bool _hasOverride;
    private T? _override;

    public T? Value => _hasOverride ? _override : fallback.Invoke();

    /// <summary>
    /// Sets an override value that takes precedence over both the primary and fallback sources. Once set, this value will be returned until the override is reset.
    /// </summary>
    /// <param name="value">The override value to set.</param>
    public void SetOverride(T value)
    {
        _override = value;
        _hasOverride = true;
    }

    /// <summary>
    /// Resets the override value, allowing the primary and fallback sources to determine the value again. After calling this method, the next access to <see cref="Value"/> will return the result of the primary source if available, or the fallback source if the primary is unavailable.
    /// </summary>
    public void ResetOverride()
    {
        _override = default;
        _hasOverride = false;
    }
}
