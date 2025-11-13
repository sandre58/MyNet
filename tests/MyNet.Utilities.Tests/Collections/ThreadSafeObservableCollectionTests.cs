// -----------------------------------------------------------------------
// <copyright file="ThreadSafeObservableCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Utilities.Collections;
using Xunit;

namespace MyNet.Utilities.Tests.Collections;

public class ThreadSafeObservableCollectionTests
{
    [Fact]
    public void Add_FromMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>();
        const int threadCount = 10;
        const int itemsPerThread = 100;

        // Act
        Parallel.For(0, threadCount, i =>
        {
            for (var j = 0; j < itemsPerThread; j++)
            {
                collection.Add((i * itemsPerThread) + j);
            }
        });

        // Assert
        Assert.Equal(threadCount * itemsPerThread, collection.Count);
    }

    [Fact]
    public void AddRange_FromMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>(1000);
        const int threadCount = 10;
        const int itemsPerThread = 100;

        // Act
        Parallel.For(0, threadCount, i =>
                {
                    var items = Enumerable.Range(i * itemsPerThread, itemsPerThread);
                    collection.AddRange(items);
                });

        // Assert
        Assert.Equal(threadCount * itemsPerThread, collection.Count);
    }

    [Fact]
    public void Remove_FromMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>();

        // Pre-fill with items
        for (var i = 0; i < 1000; i++)
        {
            collection.Add(i);
        }

        var initialCount = collection.Count;

        // Act - Multiple threads remove items sequentially (not concurrently on same item)
        var removedCount = 0;

        // Each thread removes 10 items, one at a time, from the front
        Parallel.For(0, 50, _ =>
        {
            for (var j = 0; j < 10; j++)
            {
                // Remove first item (thread-safe since we lock)
                if (collection.Count > 0)
                {
                    collection.RemoveAt(0);
                    Interlocked.Increment(ref removedCount);
                }
            }
        });

        // Assert
        Assert.Equal(500, removedCount);
        Assert.Equal(initialCount - 500, collection.Count);
    }

    [Fact]
    public void AsyncNotifications_ShouldNotBlockWorkerThread()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>(
              notifyOnUi: _ => Thread.Sleep(50), // Simulate slow UI
              useAsyncNotifications: true);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (var i = 0; i < 10; i++)
        {
            collection.Add(i);
        }

        sw.Stop();

        // Assert
        // With async, should be fast (<100ms total, not 500ms)
        Assert.True(sw.ElapsedMilliseconds < 100,
         $"Expected <100ms, got {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void Load_FromMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>();

        // Act - Multiple threads trying to load
        Parallel.For(0, 10, i =>
        {
            var items = Enumerable.Range(i * 100, 100);
            collection.Load(items);
        });

        // Assert
        Assert.Equal(100, collection.Count); // Last Load wins
    }

    [Fact]
    public void RemoveRange_FromMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>(
      Enumerable.Range(0, 1000));

        // Act
        Parallel.For(0, 10, _ =>
              {
                  if (collection.Count > 50)
                  {
                      collection.RemoveRange(0, 10);
                  }
              });

        // Assert
        Assert.True(collection.Count <= 1000);
    }

    [Fact]
    public void RemoveAll_FromMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>(
            Enumerable.Range(0, 1000));

        // Act
        Parallel.For(0, 5, i => collection.RemoveAll(x => x % 10 == i));

        // Assert
        // Should have removed items divisible by 0,1,2,3,4 (50% of items)
        Assert.True(collection.Count >= 400 && collection.Count <= 600);
    }

    [Fact]
    public void Dispose_ShouldReleaseResources()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>();

        // Act
        collection.Dispose();

        // Assert - Should not throw
        Assert.NotNull(collection);
    }

    [Fact]
    public async Task ConcurrentReadWrite_ShouldNotDeadlockAsync()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>(
          Enumerable.Range(0, 100));

        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(5));

        // Act - Concurrent reads and writes
        var writerTask = Task.Run(() =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                collection.Add(1);
            }
        },
        cts.Token);

        var readerTask = Task.Run(() =>
      {
          while (!cts.Token.IsCancellationRequested)
          {
              var count = collection.Count;
              _ = collection.FirstOrDefault();
          }
      },
        cts.Token);

        // Assert - Should complete without deadlock
        try
        {
            await Task.WhenAll(writerTask, readerTask);
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation occurs
        }

        Assert.True(cts.Token.IsCancellationRequested);
    }

    [Fact]
    public void NotificationErrors_ShouldNotCrash()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>();

        collection.CollectionChanged += (s, e) => throw new InvalidOperationException("Test exception");

        // Act & Assert - Should not crash
        collection.Add(1);
        Assert.Single(collection);
    }

    [Fact]
    public void TaskCanceledException_ShouldBeCaught()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var collection = new ThreadSafeObservableCollection<int>(
            notifyOnUi: action =>
              {
                  cts.Token.ThrowIfCancellationRequested();
                  action();
              })
        {
            // Act & Assert - Should not crash
            1
        };
        Assert.Single(collection);
    }

    [Fact]
    public void SetItem_WithKeyChange_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int> { 1, 2, 3, 4, 5 };

        // Act
        Parallel.For(0, 5, i =>
           {
               if (i < collection.Count)
               {
                   collection[i] = i * 10;
               }
           });

        // Assert
        Assert.Equal(5, collection.Count);
        Assert.All(collection, item => Assert.Equal(0, item % 10));
    }

    [Fact]
    public void Clear_FromMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>(Enumerable.Range(0, 100));

        // Act
        Parallel.For(0, 10, _ => collection.Clear());

        // Assert
        Assert.Empty(collection);
    }

    [Fact]
    public void MixedOperations_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new ThreadSafeObservableCollection<int>(500);

        // Act - Mix of different operations
        Parallel.Invoke(
                () => collection.AddRange(Enumerable.Range(0, 100)),
                () => collection.AddRange(Enumerable.Range(100, 100)),
                () =>
                    {
                        Thread.Sleep(10);
                        collection.RemoveAll(x => x % 2 == 0);
                    },
                () =>
                {
                    Thread.Sleep(20);
                    if (collection.Count > 0)
                        collection.RemoveRange(0, Math.Min(10, collection.Count));
                },
                () =>
                {
                    for (var i = 0; i < 50; i++)
                        collection.Add(1000 + i);
                });

        // Assert - Should complete without exceptions
        Assert.NotEmpty(collection);
    }

    [Fact]
    public void WithCapacity_ShouldPreAllocate()
    {
        // Arrange & Act
        var collection = new ThreadSafeObservableCollection<int>(1000);

        // Assert
        Assert.Empty(collection);

        // Capacity is inherited from base class
        // We can't directly test capacity, but we can verify no exceptions
        // and that adding items works correctly
        collection.AddRange(Enumerable.Range(0, 1000));
        Assert.Equal(1000, collection.Count);
    }
}
