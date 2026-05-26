// -----------------------------------------------------------------------
// <copyright file="RandomGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace MyNet.Generator.Facade;

/// <summary>
/// Provides a thread-safe and lock-free random number generator instance.
/// </summary>
[SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Using System.Random.Shared for non-cryptographic purposes.")]
public static class RandomGenerator
{
    private static IRandomGenerator _current = new DefaultRandomGenerator(new SystemRandomSource());

    /// <summary>
    /// Gets or sets the service used by static Random* helpers.
    /// </summary>
    public static IRandomGenerator Current
    {
        get => Volatile.Read(ref _current);
        set => Volatile.Write(ref _current,
            value ?? throw new ArgumentNullException(nameof(value)));
    }

    /// <summary>
    /// Gets the underlying shared random instance.
    /// </summary>
    public static Random Instance => Random.Shared;

    /// <summary>
    /// Resets static random helpers to the default service implementation.
    /// </summary>
    public static void Reset() => _current = new DefaultRandomGenerator(new SystemRandomSource());
}
