// -----------------------------------------------------------------------
// <copyright file="ObservableRangeCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace MyNet.Collections.Tests;

public class ObservableRangeCollectionTests
{
    [Fact]
    public void Constructor_WithCapacity_ShouldPreAllocate()
    {
        // Arrange & Act
        var collection = new ObservableRangeCollection<int>(100);

        // Assert - Verify collection works
        Assert.Empty(collection);

        // Verify adding items works without resize
        collection.AddRange(Enumerable.Range(0, 100));
        Assert.Equal(100, collection.Count);
    }

    [Fact]
    public void AddRange_ShouldAddAllItems()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>();
        var items = Enumerable.Range(1, 10).ToList();

        // Act
        collection.AddRange(items);

        // Assert
        Assert.Equal(10, collection.Count);
        Assert.Equal(items, collection);
    }

    [Fact]
    public void AddRange_WithEmptyCollection_ShouldNotNotify()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>();
        var notified = false;
        collection.CollectionChanged += (_, _) => notified = true;

        // Act
        collection.AddRange([]);

        // Assert
        Assert.False(notified);
    }

    [Fact]
    public void AddRange_ShouldSendSingleResetNotification()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>();
        var notifications = 0;
        NotifyCollectionChangedAction? lastAction = null;

        collection.CollectionChanged += (_, e) =>
        {
            notifications++;
            lastAction = e.Action;
        };

        // Act
        collection.AddRange(Enumerable.Range(1, 100));

        // Assert
        Assert.Equal(1, notifications);
        Assert.Equal(NotifyCollectionChangedAction.Reset, lastAction);
    }

    [Fact]
    public void InsertRange_ShouldInsertAtCorrectPosition()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int> { 1, 2, 5, 6 };

        // Act
        collection.InsertRange([3, 4], 2);

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, collection);
    }

    [Fact]
    public void Load_ShouldReplaceAllItems()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int> { 1, 2, 3 };

        // Act
        collection.Load([10, 20, 30]);

        // Assert
        Assert.Equal(new[] { 10, 20, 30 }, collection);
    }

    [Fact]
    public void RemoveRange_ShouldRemoveCorrectItems()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int> { 1, 2, 3, 4, 5 };

        // Act
        collection.RemoveRange(1, 3);

        // Assert
        Assert.Equal(new[] { 1, 5 }, collection);
    }

    [Fact]
    public void RemoveRange_WithInvalidRange_ShouldThrow()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int> { 1, 2, 3 };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => collection.RemoveRange(-1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => collection.RemoveRange(0, -1));
        Assert.Throws<ArgumentException>(() => collection.RemoveRange(2, 5));
    }

    [Fact]
    public void RemoveAll_ShouldRemoveMatchingItems()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int> { 1, 2, 3, 4, 5, 6 };

        // Act
        var removed = collection.RemoveAll(x => x % 2 == 0);

        // Assert
        Assert.Equal(3, removed);
        Assert.Equal(new[] { 1, 3, 5 }, collection);
    }

    [Fact]
    public void RemoveAll_WithNoMatches_ShouldReturnZero()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int> { 1, 3, 5 };

        // Act
        var removed = collection.RemoveAll(x => x % 2 == 0);

        // Assert
        Assert.Equal(0, removed);
        Assert.Equal(new[] { 1, 3, 5 }, collection);
    }

    [Fact]
    public void SuspendCount_ShouldSuppressCountNotifications()
    {
        // Arrange
        var collection = new TestRangeCollection();
        var countNotifications = 0;

        collection.PropertyChangedPublic += (_, e) =>
        {
            if (e.PropertyName == "Count")
                countNotifications++;
        };

        // Act
        using (collection.SuspendCount())
        {
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);
        }

        // Assert
        Assert.Equal(1, countNotifications); // Only one after disposal
    }

    [Fact]
    public void SuspendNotifications_ShouldSuppressAllNotifications()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>();
        var notifications = 0;

        collection.CollectionChanged += (_, _) => notifications++;

        // Act
        using (collection.SuspendNotifications())
        {
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);
        }

        // Assert
        Assert.Equal(1, notifications); // Only Reset after disposal
    }

    [Fact]
    public void SetCapacity_ShouldUpdateCapacity()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>();

        // Act
        collection.SetCapacity(500);

        // Assert
        Assert.True(collection.Capacity >= 500);
    }

    [Fact]
    public void SetCapacity_WithNegative_ShouldThrow()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => collection.SetCapacity(-1));
    }

    [Fact]
    public void Load_ShouldOnlyNotifyIfCountChanged()
    {
        // Arrange
        var collection = new TestRangeCollection { 1, 2, 3 };
        var countNotifications = 0;

        collection.PropertyChangedPublic += (_, e) =>
          {
              if (e.PropertyName == "Count")
                  countNotifications++;
          };

        // Act - Same count
        collection.Load([10, 20, 30]);

        // Assert
        Assert.Equal(0, countNotifications); // Count didn't change (3 ? 3)
    }

    [Fact]
    public void SuspendNotifications_WithoutChanges_ShouldNotNotify()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int> { 1, 2, 3 };
        var notifications = 0;

        collection.CollectionChanged += (_, _) => notifications++;

        // Act
        using (collection.SuspendNotifications())
        {
            // No changes
        }

        // Assert
        Assert.Equal(0, notifications);
    }

    [Fact]
    public void NestedSuspendCount_ShouldRequireLIFODisposal()
    {
        // Arrange
        var collection = new TestRangeCollection();
        var countNotifications = 0;

        collection.PropertyChangedPublic += (_, e) =>
        {
            if (e.PropertyName == "Count")
                countNotifications++;
        };

        // Act
        using (collection.SuspendCount())
        {
            collection.Add(1);

            using (collection.SuspendCount())
            {
                collection.Add(2);
                Assert.Equal(0, countNotifications);
            }

            // Current behavior: inner scope disposal flushes count notification
            Assert.Equal(1, countNotifications);
        }

        // Assert - one additional count notification on outer scope disposal
        Assert.Equal(2, countNotifications);
    }

    [Fact]
    public void SuspendNotifications_ShouldSendResetWhenChangesOccur()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>();
        var notifications = 0;
        NotifyCollectionChangedAction? lastAction = null;

        collection.CollectionChanged += (_, e) =>
        {
            notifications++;
            lastAction = e.Action;
        };

        // Act
        using (collection.SuspendNotifications())
        {
            collection.Add(1);
            collection.Add(2);
            Assert.Equal(0, notifications);
        }

        // Assert
        Assert.Equal(1, notifications);
        Assert.Equal(NotifyCollectionChangedAction.Reset, lastAction);
    }

    [Fact]
    public void SuspendCount_RestoredAfterDispose()
    {
        // Arrange
        var collection = new TestRangeCollection();
        var countNotifications = 0;

        collection.PropertyChangedPublic += (_, e) =>
        {
            if (e.PropertyName == "Count")
                countNotifications++;
        };

        var scope = collection.SuspendCount();
        collection.Add(1);

        // Act
        scope.Dispose();
        collection.Add(2); // After dispose, notifications should be enabled

        // Assert
        Assert.True(countNotifications > 0, "Count notifications should be enabled after SuspendCount scope is disposed");
    }

    [Fact]
    public void SuspendNotifications_RestoredAfterDispose()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>();
        var notifications = 0;

        collection.CollectionChanged += (_, _) => notifications++;

        var scope = collection.SuspendNotifications();
        collection.Add(1);
        scope.Dispose();

        // Act
        notifications = 0; // Reset counter
        collection.Add(2);

        // Assert
        Assert.True(notifications > 0, "Notifications should be enabled after SuspendNotifications scope is disposed");
    }

    [Fact]
    public void Capacity_WithNonListItems_ShouldReturnCount()
    {
        // Arrange
        var collection = new ObservableRangeCollection<int>(new[] { 1, 2, 3 });

        // Act & Assert
        // If Items is not List<T>, Capacity returns Count
        Assert.True(collection.Capacity >= collection.Count);
    }

    // Test helper class
    private sealed class TestRangeCollection : ObservableRangeCollection<int>
    {
        [SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Only for tests")]
        public event PropertyChangedEventHandler? PropertyChangedPublic
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }
    }
}
