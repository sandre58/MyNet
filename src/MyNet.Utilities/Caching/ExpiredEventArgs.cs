// -----------------------------------------------------------------------
// <copyright file="ExpiredEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Caching;

/// <summary>
/// The expired event args.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ExpiredEventArgs{TKey, TValue}" /> class.
/// </remarks>
/// <param name="key">The key.</param>
/// <param name="value">The value.</param>
/// <param name="dispose">The value indicating whether the expired value should be disposed after removal from cache.</param>
public class ExpiredEventArgs<TKey, TValue>(TKey key, TValue value, bool dispose) : EventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether the expired value should be disposed after removal from cache.
    /// </summary>
    /// <value><c>true</c> if item should be disposed; otherwise, <c>false</c>.</value>
    /// <remarks>Current value of this property is equal to <see cref="ICacheStorage{TKey, TValue}.DisposeValuesOnRemoval"/> value.</remarks>
    public bool Dispose { get; set; } = dispose;

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The key.</value>
    public TKey Key { get; private set; } = key;

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public TValue Value { get; private set; } = value;
}
