// -----------------------------------------------------------------------
// <copyright file="UiCollectionsIntegrationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Utilities.Collections;
using Xunit;

namespace MyNet.Utilities.Tests.Collections;

/// <summary>
/// Integration tests for UI collections (ScheduleObservableCollection, UiObservableCollection).
/// These are tested via ThreadSafeObservableCollection since they inherit from it.
/// </summary>
public class UiCollectionsIntegrationTests
{
    [Fact]
    public void ScheduleObservableCollection_WithAsyncNotifications_ShouldNotBlock()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
            scheduler: () => scheduler,
            useAsyncNotifications: true);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Act - Add 10 items with slow scheduler
        for (var i = 0; i < 10; i++)
        {
            collection.Add(i);
        }

        sw.Stop();

        // Assert - Should be fast (async)
        Assert.True(sw.ElapsedMilliseconds < 100,
        $"Expected <100ms with async, got {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void ScheduleObservableCollection_ConcurrentOperations_ShouldBeThreadSafe()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
            capacity: 1000,
            scheduler: () => scheduler);

        // Act
        Parallel.For(0, 10, i => collection.AddRange(Enumerable.Range(i * 100, 100)));

        // Assert
        Assert.Equal(1000, collection.Count);
    }

    [Fact]
    public void ScheduleObservableCollection_Load_ShouldReplaceAndSchedule()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
    scheduler: () => scheduler);
        collection.AddRange([1, 2, 3]);

        // Act
        collection.Load([10, 20, 30]);

        // Assert
        Assert.Equal([10, 20, 30], collection);
    }

    [Fact]
    public void ScheduleObservableCollection_RemoveAll_ShouldSchedule()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
              scheduler: () => scheduler);
        collection.AddRange(Enumerable.Range(0, 100));

        // Act
        var removed = collection.RemoveAll(x => x % 2 == 0);

        // Assert
        Assert.Equal(50, removed);
        Assert.Equal(50, collection.Count);
    }

    [Fact]
    public void ScheduleObservableCollection_Dispose_ShouldNotThrow()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
          scheduler: () => scheduler);
        collection.AddRange([1, 2, 3]);

        // Act & Assert
        collection.Dispose(); // Should not throw
    }

    [Fact]
    public void ScheduleObservableCollection_WithDifferentList_ShouldWork()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var list = new List<int> { 1, 2, 3 };

        // Act
        var collection = new TestScheduleObservableCollection<int>(
               list,
               scheduler: () => scheduler);

        // Assert
        Assert.Equal(list, collection);
    }

    [Fact]
    public void ScheduleObservableCollection_WithEnumerable_ShouldWork()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var items = Enumerable.Range(0, 100);

        // Act
        var collection = new TestScheduleObservableCollection<int>(
        items,
        scheduler: () => scheduler);

        // Assert
        Assert.Equal(100, collection.Count);
    }

    [Fact]
    public void ScheduleObservableCollection_SetCapacity_ShouldWork()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
            scheduler: () => scheduler);

        // Act
        collection.SetCapacity(500);

        // Assert
        Assert.True(collection.Capacity >= 500);
    }

    // Test helper classes
    private sealed class TestScheduler
    {
        public int ScheduledCount { get; private set; }

        public void Schedule(Action action)
        {
            ScheduledCount++;

            // Simulate some work
            Thread.Sleep(5);
            action();
        }
    }

    private sealed class TestScheduleObservableCollection<T> : ThreadSafeObservableCollection<T>
    {
        public TestScheduleObservableCollection(Func<TestScheduler> scheduler, bool useAsyncNotifications = true)
            : base(x => scheduler().Schedule(x), useAsyncNotifications) { }

        public TestScheduleObservableCollection(int capacity, Func<TestScheduler> scheduler, bool useAsyncNotifications = true)
            : base(capacity, x => scheduler().Schedule(x), useAsyncNotifications) { }

        public TestScheduleObservableCollection(IList<T> list, Func<TestScheduler> scheduler, bool useAsyncNotifications = true)
        : base(list, x => scheduler().Schedule(x), useAsyncNotifications) { }

        public TestScheduleObservableCollection(IEnumerable<T> collection, Func<TestScheduler> scheduler, bool useAsyncNotifications = true)
        : base(collection, x => scheduler().Schedule(x), useAsyncNotifications) { }
    }
}
