// -----------------------------------------------------------------------
// <copyright file="RandomGeneratorTestGate.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using MyNet.Generator.Facade;

namespace MyNet.Generator.Tests;

/// <summary>
/// Serializes access to <see cref="RandomGenerator.Current"/> during tests.
/// </summary>
internal static class RandomGeneratorTestGate
{
    private static readonly object Sync = new();

    public static IDisposable Replace(IRandomGenerator generator)
    {
        Monitor.Enter(Sync);

        try
        {
            var previous = RandomGenerator.Current;
            RandomGenerator.Current = generator;
            return new RestoreScope(previous);
        }
        catch
        {
            Monitor.Exit(Sync);
            throw;
        }
    }

    private sealed class RestoreScope(IRandomGenerator previous) : IDisposable
    {
        public void Dispose()
        {
            RandomGenerator.Current = previous;
            Monitor.Exit(Sync);
        }
    }
}
