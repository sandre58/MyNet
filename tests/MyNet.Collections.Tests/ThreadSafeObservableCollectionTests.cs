// -----------------------------------------------------------------------
// <copyright file="ThreadSafeObservableCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyNet.Collections.Tests;

public class ThreadSafeObservableCollectionTests
{
    [Fact]
    public void Add_FromMultipleThreads_ShouldBeThreadSafe()
    {
        var collection = new ObservableRangeCollection<int>();
        var sut = new SynchronizedObservableCollection<int>(collection);
        const int threadCount = 10;
        const int itemsPerThread = 100;

        Parallel.For(0, threadCount, i =>
        {
            for (var j = 0; j < itemsPerThread; j++)
            {
                sut.Add((i * itemsPerThread) + j);
            }
        });

        Assert.Equal(threadCount * itemsPerThread, sut.Count);
    }

    [Fact]
    public void AddRange_FromMultipleThreads_ShouldBeThreadSafe()
    {
        var collection = new ObservableRangeCollection<int>(1000);
        var sut = new SynchronizedObservableCollection<int>(collection);
        const int threadCount = 10;
        const int itemsPerThread = 100;

        Parallel.For(0, threadCount, i =>
        {
            var items = Enumerable.Range(i * itemsPerThread, itemsPerThread);
            sut.AddRange(items);
        });

        Assert.Equal(threadCount * itemsPerThread, sut.Count);
    }

    [Fact]
    public void Remove_FromMultipleThreads_ShouldBeThreadSafe()
    {
        var collection = new ObservableRangeCollection<int>();
        var sut = new SynchronizedObservableCollection<int>(collection);

        for (var i = 0; i < 1000; i++)
            sut.Add(i);

        var initialCount = sut.Count;

        var removedCount = 0;

        Parallel.For(0, 50, _ =>
        {
            for (var j = 0; j < 10; j++)
            {
                if (sut.Count > 0)
                {
                    sut.RemoveAt(0);
                    Interlocked.Increment(ref removedCount);
                }
            }
        });

        Assert.Equal(500, removedCount);
        Assert.Equal(initialCount - 500, sut.Count);
    }

    [Fact]
    public async Task Enumerator_ShouldUseSnapshot_WhenCollectionChangesConcurrentlyAsync()
    {
        var collection = new ObservableRangeCollection<int>();
        var sut = new SynchronizedObservableCollection<int>(collection);

        for (var i = 0; i < 100; i++)
            sut.Add(i);

        var enumerationTask = Task.Run(() =>
        {
            var result = new List<int>();
            using var enumerator = sut.GetEnumerator();
            while (enumerator.MoveNext())
                result.Add(enumerator.Current);

            return result;
        });
        var writerTask = Task.Run(() =>
        {
            for (var i = 100; i < 200; i++)
                sut.Add(i);
        });

        await Task.WhenAll(enumerationTask, writerTask);

        var snapshot = await enumerationTask.ConfigureAwait(true);

        Assert.InRange(snapshot.Count, 100, 200);
        Assert.Equal(200, sut.Count);
    }

    [Fact]
    public void Adapter_ShouldForwardNotifications()
    {
        var collection = new ObservableRangeCollection<int>();
        var sut = new SynchronizedObservableCollection<int>(collection);
        var count = 0;

        sut.CollectionChanged += (_, _) => count++;
        sut.Add(1);

        Assert.Equal(1, count);
    }

    [Fact]
    public void SynchronizedExtension_ShouldCreateAdapter()
    {
        var sut = new ObservableRangeCollection<int>().Synchronized();
        sut.Add(42);

        Assert.Single(sut);
    }
}
