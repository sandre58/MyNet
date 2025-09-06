// -----------------------------------------------------------------------
// <copyright file="AsyncValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities;

/// <summary>
/// Provides a lazily-evaluated value produced by a factory delegate.
/// The value is computed on first access and cached for subsequent accesses.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="AsyncValue{T}"/> class with the specified value provider.
/// </remarks>
/// <param name="provideValue">A factory that produces the value when required.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="provideValue"/> is <c>null</c>.</exception>
public class AsyncValue<T>(Func<T> provideValue)
{
    private readonly Func<T> _provideValue = provideValue ?? throw new ArgumentNullException(nameof(provideValue));
    private T? _field;

    /// <summary>
    /// Gets the lazily-computed value. The factory is invoked only once on the first access.
    /// </summary>
    public T Value => _field ??= _provideValue();
}
