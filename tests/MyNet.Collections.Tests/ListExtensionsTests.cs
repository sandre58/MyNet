// -----------------------------------------------------------------------
// <copyright file="ListExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace MyNet.Collections.Tests;

public sealed class ListExtensionsTests
{
    [Fact]
    public void Swap_ValidIndices_ExchangesElements()
    {
        var list = new ArrayList { "a", "b", "c" };

        list.Swap(0, 2);

        Assert.Equal(["c", "b", "a"], list);
    }

    [Fact]
    public void Swap_InvalidIndices_DoesNotChangeList()
    {
        var list = new ArrayList { 1, 2, 3 };

        list.Swap(0, 5);
        list.Swap(-1, 1);
        list.Swap(1, 1);

        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void Sort_WithComparer_SortsNonListImplementation()
    {
        var list = new TestList<int> { 3, 1, 2 };

        list.Sort(x => x, Comparer<int>.Default);

        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void SortDescending_WithComparer_SortsDescending()
    {
        var list = new TestList<int> { 1, 3, 2 };

        list.SortDescending(x => x, Comparer<int>.Default);

        Assert.Equal([3, 2, 1], list);
    }

    [Fact]
    public void SortByDisplay_SortsCaseInsensitiveByDisplayText()
    {
        var list = new List<string> { "zebra", "Alpha", "beta" };

        list.SortByDisplay(x => x, CultureInfo.InvariantCulture);

        Assert.Equal(["Alpha", "beta", "zebra"], list);
    }

    [Fact]
    public void GetByIndex_InRange_ReturnsElement()
    {
        var list = new List<string> { "a", "b" };

        Assert.Equal("b", list.GetByIndex(1));
    }

    [Fact]
    public void GetByIndex_OutOfRange_ReturnsDefault()
    {
        var list = new List<string> { "a" };

        Assert.Null(list.GetByIndex(5, defaultValue: null));
    }

    [Fact]
    public void UpdateFrom_InvokesAddUpdateAndRemoveCallbacks()
    {
        var destination = new List<SyncItem>
        {
            new(1, "keep-old"),
            new(2, "remove-me")
        };

        var source = new[]
        {
            new SyncItem(1, "keep-new"),
            new SyncItem(3, "add-me")
        };

        var removed = new List<int>();
        var updated = new List<(int DestId, int SrcId)>();
        var added = new List<int>();

        destination.UpdateFrom(
            source,
            item => item.Id,
            item => item.Id,
            src => added.Add(src.Id),
            item => removed.Add(item.Id),
            (dest, src) => updated.Add((dest.Id, src.Id)));

        Assert.Equal([2], removed);
        Assert.Equal([(1, 1)], updated);
        Assert.Equal([3], added);
    }

    private sealed record SyncItem(int Id, string Name);

    private sealed class TestList<T> : IList<T>
    {
        private readonly List<T> _inner = [];

        public T this[int index]
        {
            get => _inner[index];
            set => _inner[index] = value;
        }

        public int Count => _inner.Count;

        public bool IsReadOnly => false;

        public void Add(T item) => _inner.Add(item);

        public void Clear() => _inner.Clear();

        public bool Contains(T item) => _inner.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

        public int IndexOf(T item) => _inner.IndexOf(item);

        public void Insert(int index, T item) => _inner.Insert(index, item);

        public bool Remove(T item) => _inner.Remove(item);

        public void RemoveAt(int index) => _inner.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();
    }
}
