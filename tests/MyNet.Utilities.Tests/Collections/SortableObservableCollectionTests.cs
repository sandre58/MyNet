// -----------------------------------------------------------------------
// <copyright file="SortableObservableCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MyNet.Utilities.Collections;
using Xunit;

namespace MyNet.Utilities.Tests.Collections;

public class SortableObservableCollectionTests
{
    [Fact]
    public void Sort_ShouldSortAllItems()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x);
        collection.AddRange([5, 3, 8, 1, 9, 2]);

        // Act
        collection.Sort();

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 5, 8, 9 }, collection);
    }

    [Fact]
    public void Sort_Descending_ShouldSortCorrectly()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(
     x => x,
     ListSortDirection.Descending);
        collection.AddRange([5, 3, 8, 1, 9, 2]);

        // Act
        collection.Sort();

        // Assert
        Assert.Equal(new[] { 9, 8, 5, 3, 2, 1 }, collection);
    }

    [Fact]
    public void Add_WithAutoSort_ShouldInsertAtCorrectPosition()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x)
        {
            // Act
            5,
            2,
            8,
            1
        };

        // Assert
        Assert.Equal(new[] { 1, 2, 5, 8 }, collection);
    }

    [Fact]
    public void Add_WithAutoSortDisabled_ShouldNotSort()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x)
        {
            AutoSort = false
        };

        // Act
        collection.Add(5);
        collection.Add(2);
        collection.Add(8);

        // Assert
        Assert.Equal(new[] { 5, 2, 8 }, collection); // Not sorted
    }

    [Fact]
    public void AddRange_ShouldSortOnceAtEnd()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x);
        var sortCalls = 0;

        // Count notifications (Reset = sort happened)
        collection.CollectionChanged += (s, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                sortCalls++;
        };

        // Act
        collection.AddRange([5, 3, 8, 1, 9, 2]);

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 5, 8, 9 }, collection);
        Assert.Equal(1, sortCalls); // Only one sort at the end
    }

    [Fact]
    public void Load_ShouldSortAfterLoad()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x);
        collection.AddRange([1, 2, 3]);

        // Act
        collection.Load([9, 5, 3, 7, 1]);

        // Assert
        Assert.Equal(new[] { 1, 3, 5, 7, 9 }, collection);
    }

    [Fact]
    public void ChangeSortSelector_ShouldUpdateComparer()
    {
        // Arrange
        var collection = new SortableObservableCollection<string>(x => x.Length);
        collection.AddRange(["aaa", "b", "cc"]);
        collection.Sort();
        Assert.Equal(new[] { "b", "cc", "aaa" }, collection); // Sorted by length

        // Act - Change selector
        collection.SortSelector = x => x;
        collection.Sort();

        // Assert
        Assert.Equal(new[] { "aaa", "b", "cc" }, collection); // Sorted alphabetically
    }

    [Fact]
    public void ChangeSortDirection_ShouldUpdateComparer()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x);
        collection.AddRange([5, 3, 8, 1]);
        collection.Sort();
        Assert.Equal(new[] { 1, 3, 5, 8 }, collection);

        // Act - Change direction
        collection.SortDirection = ListSortDirection.Descending;
        collection.Sort();

        // Assert
        Assert.Equal(new[] { 8, 5, 3, 1 }, collection);
    }

    [Fact]
    public void Sort_WithNoSelector_ShouldNotThrow()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>();
        collection.AddRange([5, 3, 8]);

        // Act & Assert
        collection.Sort(); // Should not throw Without selector, no sorting occurs
    }

    [Fact]
    public void Sort_WithLessThanTwoItems_ShouldNotSort()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x)
        {
            5
        };

        // Act
        collection.Sort();

        // Assert
        Assert.Single(collection);
        Assert.Equal(5, collection[0]);
    }

    [Fact]
    public void Sort_Reentrancy_ShouldBeProtected()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x);
        collection.AddRange([5, 3, 8, 1]);

        var sortCount = 0;
        collection.CollectionChanged += (s, e) =>
           {
               if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
               {
                   sortCount++;

                   // Try to sort again during notification
                   if (sortCount == 1)
                       collection.Sort(); // Should be ignored
               }
           };

        // Act
        collection.Sort();

        // Assert
        Assert.Equal(1, sortCount); // Only one sort, reentrancy protected
    }

    [Fact]
    public void BinarySearch_ShouldFindCorrectIndex()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x);
        collection.AddRange([1, 3, 5, 7, 9]);

        // Act - Insert 6
        collection.Add(6);

        // Assert
        Assert.Equal(new[] { 1, 3, 5, 6, 7, 9 }, collection);
    }

    [Fact]
    public void ComplexObjectSort_ShouldWork()
    {
        // Arrange
        var collection = new SortableObservableCollection<Product>(p => p.Price)
        {
            // Act
            new() { Name = "A", Price = 50 },
            new() { Name = "B", Price = 20 },
            new() { Name = "C", Price = 80 },
            new() { Name = "D", Price = 10 }
        };

        // Assert
        Assert.Equal(10, collection[0].Price);
        Assert.Equal(20, collection[1].Price);
        Assert.Equal(50, collection[2].Price);
        Assert.Equal(80, collection[3].Price);
    }

    [Fact]
    public void ManualSort_WithAutoSortOff_ShouldWork()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x)
        {
            AutoSort = false
        };
        collection.AddRange([5, 3, 8, 1, 9, 2]);

        // Act
        collection.Sort();
        collection.AutoSort = true;

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 5, 8, 9 }, collection);
    }

    [Fact]
    public void AddRange_WithAutoSortOff_ThenOn_ShouldSortOnce()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x);
        var notifications = 0;

        collection.CollectionChanged += (s, e) => notifications++;

        // Act
        collection.AutoSort = false;
        collection.AddRange([5, 3, 8, 1]);
        collection.AutoSort = true;
        collection.Sort();

        // Assert
        Assert.Equal(new[] { 1, 3, 5, 8 }, collection);

        // 1 notification from AddRange (Reset)
        // 1 notification from Sort (Reset)
        Assert.Equal(2, notifications);
    }

    [Fact]
    public void Constructor_WithCapacity_ShouldWork()
    {
        // Arrange & Act
        var collection = new SortableObservableCollection<int>(100);

        // Assert - Verify collection works with capacity
        Assert.Empty(collection);

        // Verify can add items without exception
        collection.AddRange(Enumerable.Range(0, 100));
        Assert.Equal(100, collection.Count);
    }

    [Fact]
    public void ThreadSafety_ShouldInheritFromBase()
    {
        // Arrange
        var collection = new SortableObservableCollection<int>(x => x);

        // Act - Concurrent adds
        System.Threading.Tasks.Parallel.For(0, 100, i => collection.Add(i));

        // Assert
        Assert.Equal(100, collection.Count);
        Assert.Equal(Enumerable.Range(0, 100).Order(), collection);
    }

    private sealed class Product
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
