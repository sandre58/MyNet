// -----------------------------------------------------------------------
// <copyright file="SystemRandomSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace MyNet.Generator;

/// <summary>
/// Current random source backed by <see cref="System.Random.Shared"/>.
/// </summary>
[SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Using System.Random.Shared for non-cryptographic purposes.")]
public sealed class SystemRandomSource : IRandomSource
{
    /// <inheritdoc />
    public int NextInt32(int minInclusive, int maxExclusive) => System.Random.Shared.Next(minInclusive, maxExclusive);

    /// <inheritdoc />
    public double NextDouble() => System.Random.Shared.NextDouble();

    /// <inheritdoc />
    public void NextBytes(byte[] buffer) => System.Random.Shared.NextBytes(buffer);
}
