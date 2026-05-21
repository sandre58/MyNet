// -----------------------------------------------------------------------
// <copyright file="IRandomSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Generator;

/// <summary>
/// Abstraction over random generation primitives to make higher-level generators deterministic in tests.
/// </summary>
public interface IRandomSource
{
    /// <summary>
    /// Returns a random integer greater than or equal to minInclusive and less than maxExclusive.
    /// </summary>
    /// <param name="minInclusive">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxExclusive">The exclusive upper bound of the random number returned.</param>
    /// <returns>A random integer greater than or equal to minInclusive and less than maxExclusive.</returns>
    int NextInt32(int minInclusive, int maxExclusive);

    /// <summary>
    /// Returns a random double greater than or equal to 0.0 and less than 1.0.
    /// </summary>
    /// <returns>A random double greater than or equal to 0.0 and less than 1.0.</returns>
    double NextDouble();

    /// <summary>
    /// Fills the provided byte array with random bytes.
    /// </summary>
    /// <param name="buffer">The array to fill with random bytes.</param>
    void NextBytes(byte[] buffer);
}
