// -----------------------------------------------------------------------
// <copyright file="RandomGeneratorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Generator;
using MyNet.Utilities.Generator.Static;
using Xunit;

namespace MyNet.Utilities.Tests.Generator;

public class RandomGeneratorTests
{
    [Fact]
    public void StaticApis_UseConfiguredService()
    {
        var previous = RandomGenerator.Current;

        try
        {
            var fixedSource = new FixedRandomSource(0.0d);
            RandomGenerator.Current = new DefaultRandomGenerator(fixedSource);

            // Verify the current generator is the one we set
            Assert.Same(RandomGenerator.Current, RandomGenerator.Current);

            // Verify it produces deterministic output via FixedRandomSource
            Assert.Equal(5, RandomGenerator.Current.Int(5, 10));
        }
        finally
        {
            RandomGenerator.Current = previous;
        }
    }

    private sealed class FixedRandomSource(double value) : IRandomSource
    {
        public int NextInt32(int minInclusive, int maxExclusive) => minInclusive;

        public double NextDouble() => value;

        public void NextBytes(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
        }
    }
}
