// -----------------------------------------------------------------------
// <copyright file="UiCollectionsIntegrationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using MyNet.Utilities.Collections;
using Xunit;

namespace MyNet.Utilities.Tests.Collections;

/// <summary>
/// Integration tests for UI collections (ScheduleObservableCollection, UiObservableCollection).
/// Scheduling is tested independently from threading concerns.
/// </summary>
public class UiCollectionsIntegrationTests
{
    [Fact]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local", Justification = "Testing scheduling, not collection behavior")]
    public void ScheduleObservableCollection_ShouldScheduleNotifications()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(scheduler: scheduler);

        var sw = Stopwatch.StartNew();

        // Act - Add 10 items with slow scheduler
        for (var i = 0; i < 10; i++)
        {
            collection.Add(i);
        }

        sw.Stop();

        // Assert
        Assert.True(sw.ElapsedMilliseconds < 250, $"Expected <250ms, got {sw.ElapsedMilliseconds}ms");
        Assert.Equal(10, scheduler.ScheduledCount);
    }

    [Fact]
    public void ScheduleObservableCollection_BulkOperations_ShouldComplete()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
            capacity: 1000,
            scheduler: scheduler);

        // Act
        for (var i = 0; i < 10; i++)
            collection.AddRange(Enumerable.Range(i * 100, 100));

        Assert.Equal(1000, collection.Count);
    }

    [Fact]
    public void ScheduleObservableCollection_Load_ShouldReplaceAndSchedule()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
    scheduler: scheduler);
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
              scheduler: scheduler);
        collection.AddRange(Enumerable.Range(0, 100));

        // Act
        var removed = collection.RemoveAll(x => x % 2 == 0);

        // Assert
        Assert.Equal(50, removed);
        Assert.Equal(50, collection.Count);
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
               scheduler: scheduler);

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
        scheduler: scheduler);

        // Assert
        Assert.Equal(100, collection.Count);
    }

    [Fact]
    public void ScheduleObservableCollection_SetCapacity_ShouldWork()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var collection = new TestScheduleObservableCollection<int>(
            scheduler: scheduler);

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

    private sealed class TestScheduleObservableCollection<T> : ObservableRangeCollection<T>
    {
        private readonly TestScheduler _scheduler;

        public TestScheduleObservableCollection(TestScheduler scheduler)
            => _scheduler = scheduler;

        public TestScheduleObservableCollection(int capacity, TestScheduler scheduler)
            : base(capacity) => _scheduler = scheduler;

        public TestScheduleObservableCollection(IList<T> list, TestScheduler scheduler)
            : base(list) => _scheduler = scheduler;

        public TestScheduleObservableCollection(IEnumerable<T> collection, TestScheduler scheduler)
            : base(collection) => _scheduler = scheduler;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            => _scheduler.Schedule(() => base.OnCollectionChanged(e));
    }
}
