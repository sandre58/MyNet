// -----------------------------------------------------------------------
// <copyright file="LockCollectionSynchronizerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace MyNet.Collections.Tests;

public sealed class LockCollectionSynchronizerTests
{
    [Fact]
    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Not relevant for testing purposes.")]
    public async Task Write_SerializesConcurrentUpdatesAsync()
    {
        using var sut = new LockCollectionSynchronizer();
        var bag = new ConcurrentBag<int>();

        await Task.WhenAll(
            Task.Run(() => sut.Write(() => bag.Add(1))),
            Task.Run(() => sut.Write(() => bag.Add(2))),
            Task.Run(() => sut.Write(() => bag.Add(3))));

        Assert.Equal(3, bag.Count);
    }

    [Fact]
    public void Read_ReturnsFuncResult()
    {
        using var sut = new LockCollectionSynchronizer();

        var result = sut.Read(() => 42);

        Assert.Equal(42, result);
    }
}
