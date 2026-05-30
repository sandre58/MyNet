// -----------------------------------------------------------------------
// <copyright file="RandomGeneratorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Generator.Facade;
using Xunit;

namespace MyNet.Generator.Tests;

[Collection(RandomGeneratorTestCollection.Name)]
public class RandomGeneratorTests
{
    [Fact]
    public void StaticApis_UseConfiguredService()
    {
        var generator = new DefaultRandomGenerator(new FixedRandomSource(0.0d));

        using (RandomGeneratorTestGate.Replace(generator))
        {
            Assert.Same(generator, RandomGenerator.Current);
            Assert.Equal(5, generator.Int(5, 10));
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
