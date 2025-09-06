// -----------------------------------------------------------------------
// <copyright file="ObservableKeyedCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MyNet.Utilities.Collections;

/// <summary>
/// A keyed observable collection that maintains an internal dictionary for fast key lookups.
/// </summary>
/// <typeparam name="TKey">The type of the key for items in the collection.</typeparam>
/// <typeparam name="T">The type of the items in the collection.</typeparam>
public abstract class ObservableKeyedCollection<TKey, T> : SortableObservableCollection<T>
    where TKey : notnull
{
    private Dictionary<TKey, T>? _dict;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey, T}"/> class.
    /// </summary>
    protected ObservableKeyedCollection()
        : this([]) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey, T}"/> class with the specified comparer.
    /// </summary>
    /// <param name="comparer">The equality comparer used to compare keys.</param>
    protected ObservableKeyedCollection(IEqualityComparer<TKey> comparer)
        : this([], comparer) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey, T}"/> class with sorting options.
    /// </summary>
    /// <param name="sortSelector">A selector used to order items.</param>
    /// <param name="direction">The sort direction.</param>
    protected ObservableKeyedCollection(Func<T, object> sortSelector, ListSortDirection direction = ListSortDirection.Ascending)
        : base(sortSelector, direction) => Comparer = EqualityComparer<TKey>.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey, T}"/> class that contains elements copied from the specified list.
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new collection.</param>
    /// <param name="comparer">The optional comparer used to compare keys. If null the default comparer is used.</param>
    protected ObservableKeyedCollection(IEnumerable<T> list, IEqualityComparer<TKey>? comparer = null)
        : base(list)
    {
        comparer ??= EqualityComparer<TKey>.Default;

        Comparer = comparer;
    }

    /// <summary>
    /// Gets the comparer used to compare keys.
    /// </summary>
    public IEqualityComparer<TKey> Comparer { get; }

    /// <summary>
    /// Gets the internal dictionary used for fast key lookups, if it has been created.
    /// </summary>
    protected IDictionary<TKey, T>? Dictionary => _dict;

    /// <summary>
    /// Gets the item associated with the specified key, or null if the key is not present.
    /// </summary>
    /// <param name="key">The key of the item to get.</param>
    /// <returns>The item associated with the specified key, or null if not found.</returns>
    public T? this[TKey key]
        => key switch
        {
            null => throw new ArgumentNullException(nameof(key)),
            _ => _dict is not null && _dict.TryGetValue(key, out var value) ? value : Items.FirstOrDefault(x => Comparer.Equals(GetKeyForItem(x), key))
        };

    /// <summary>
    /// Determines whether the collection contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the collection.</param>
    /// <returns>True if an element with the key exists; otherwise false.</returns>
    public bool Contains(TKey key)
        => key switch
        {
            null => throw new ArgumentNullException(nameof(key)),
            _ => _dict?.ContainsKey(key) ?? Items.Any(x => Comparer.Equals(GetKeyForItem(x), key))
        };

    /// <summary>
    /// Attempts to add an item to the collection if its key is not already present.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>True if the item was added; false if the key was null or already exists.</returns>
    public bool TryAdd(T item)
    {
        var key = GetKeyForItem(item);
        if (key is null || _dict?.ContainsKey(key) != false) return false;

        Add(item);

        return true;
    }

    /// <summary>
    /// Removes the item with the specified key from the collection.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>True if the item was found and removed; otherwise false.</returns>
    public bool Remove(TKey key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (_dict is not null)
        {
            return _dict.ContainsKey(key) && Remove(_dict[key]);
        }

        for (var i = 0; i < Items.Count; i++)
        {
            if (!Comparer.Equals(GetKeyForItem(Items[i]), key)) continue;
            RemoveItem(i);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Change the key associated with an existing item in the collection.
    /// </summary>
    /// <param name="item">The item whose key is changing.</param>
    /// <param name="newKey">The new key to associate with the item.</param>
    protected void ChangeItemKey(T item, TKey? newKey)
    {
        // check if the item exists in the collection
        if (!ContainsItem(item))
        {
            return;
        }

        var oldKey = GetKeyForItem(item);

        if (Comparer.Equals(oldKey, newKey)) return;

        if (newKey is not null)
        {
            AddKey(newKey, item);
        }

        if (oldKey is not null)
        {
            RemoveKey(oldKey);
        }
    }

    /// <inheritdoc />
    protected override void ClearItems()
    {
        _dict?.Clear();

        base.ClearItems();
    }

    /// <summary>
    /// When implemented in a derived class, returns the key for the specified item.
    /// </summary>
    /// <param name="item">The item to extract the key from.</param>
    /// <returns>The key for the specified item, or null if no key is associated.</returns>
    protected abstract TKey? GetKeyForItem(T item);

    protected override void InsertItem(int index, T item)
    {
        var key = GetKeyForItem(item);
        if (key is not null)
        {
            AddKey(key, item);
        }

        base.InsertItem(index, item);
    }

    /// <summary>
    /// Inserts an item directly into the underlying Items collection without dictionary handling.
    /// </summary>
    /// <param name="index">The position at which to insert the item.</param>
    /// <param name="item">The item to insert.</param>
    protected void InsertItemInItems(int index, T item) => base.InsertItem(index, item);

    protected override void RemoveItem(int index)
    {
        var key = GetKeyForItem(Items[index]);
        if (key is not null)
        {
            RemoveKey(key);
        }

        base.RemoveItem(index);
    }

    protected override void SetItem(int index, T item)
        => ExecuteThreadSafe(() =>
        {
            var newKey = GetKeyForItem(item);
            var oldKey = GetKeyForItem(Items[index]);

            if (Comparer.Equals(oldKey, newKey))
            {
                if (newKey is not null && _dict is not null)
                {
                    _dict[newKey] = item;
                }
            }
            else
            {
                if (newKey is not null)
                {
                    AddKey(newKey, item);
                }

                if (oldKey is not null)
                {
                    RemoveKey(oldKey);
                }
            }

            base.SetItem(index, item);
        });

    private bool ContainsItem(T item)
    {
        TKey? key;
        if (_dict is null || (key = GetKeyForItem(item)) is null)
        {
            return Items.Contains(item);
        }

        var exist = _dict.TryGetValue(key, out var itemInDict);
        return exist && EqualityComparer<T>.Default.Equals(itemInDict, item);
    }

    private void AddKey(TKey key, T item)
        => ExecuteThreadSafe(() =>
        {
            if (_dict is null)
            {
                CreateDictionary();
            }

            _dict?.Add(key, item);
        });

    private void CreateDictionary()
    {
        _dict = new Dictionary<TKey, T>(Comparer);
        foreach (var item in Items)
        {
            var key = GetKeyForItem(item);
            if (key is not null)
            {
                _dict.Add(key, item);
            }
        }
    }

    private void RemoveKey(TKey key)
        => ExecuteThreadSafe(() =>
        {
            if (_dict is not null)
            {
                _ = _dict.Remove(key);
            }
        });
}
