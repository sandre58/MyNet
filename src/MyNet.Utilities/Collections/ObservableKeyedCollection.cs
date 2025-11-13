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
/// A keyed observable collection that maintains an internal dictionary for fast O(1) key lookups.
/// Optimized for performance with automatic dictionary creation and capacity management.
/// </summary>
/// <typeparam name="TKey">The type of the key for items in the collection.</typeparam>
/// <typeparam name="T">The type of the items in the collection.</typeparam>
public abstract class ObservableKeyedCollection<TKey, T> : SortableObservableCollection<T>
    where TKey : notnull
{
    private readonly int _dictionaryCreationThreshold;
    private Dictionary<TKey, T>? _dict;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey, T}"/> class.
    /// </summary>
    protected ObservableKeyedCollection(Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : this([], null, 0, notifyOnUi, useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey, T}"/> class with the specified comparer.
    /// </summary>
    /// <param name="comparer">The equality comparer used to compare keys.</param>
    /// <param name="dictionaryCreationThreshold">Minimum items before creating dictionary. 0 = always create.</param>
    /// <param name="notifyOnUi">Optional action used to marshal notifications on the UI thread.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously to avoid blocking.</param>
    protected ObservableKeyedCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold = 0, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
        : this([], comparer, dictionaryCreationThreshold, notifyOnUi, useAsyncNotifications) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey, T}"/> class with sorting options.
    /// </summary>
    /// <param name="sortSelector">A selector used to order items.</param>
    /// <param name="direction">The sort direction.</param>
    /// <param name="dictionaryCreationThreshold">Minimum items before creating dictionary. 0 = always create.</param>
    /// <param name="notifyOnUi">Optional action used to marshal notifications on the UI thread.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously to avoid blocking.</param>
    protected ObservableKeyedCollection(Func<T, object> sortSelector, ListSortDirection direction = ListSortDirection.Ascending, int dictionaryCreationThreshold = 0, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
     : base(sortSelector, direction, notifyOnUi, useAsyncNotifications)
    {
        Comparer = EqualityComparer<TKey>.Default;
        _dictionaryCreationThreshold = dictionaryCreationThreshold;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey, T}"/> class that contains elements copied from the specified list.
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new collection.</param>
    /// <param name="comparer">The optional comparer used to compare keys.</param>
    /// <param name="dictionaryCreationThreshold">Minimum items before creating dictionary. 0 = always create.</param>
    /// <param name="notifyOnUi">Optional action used to marshal notifications on the UI thread.</param>
    /// <param name="useAsyncNotifications">If true, notifications are sent asynchronously to avoid blocking.</param>
    protected ObservableKeyedCollection(IEnumerable<T> list, IEqualityComparer<TKey>? comparer = null, int dictionaryCreationThreshold = 0, Action<Action>? notifyOnUi = null, bool useAsyncNotifications = true)
   : base(list, notifyOnUi, useAsyncNotifications)
    {
        Comparer = comparer ?? EqualityComparer<TKey>.Default;
        _dictionaryCreationThreshold = dictionaryCreationThreshold;

        // Pre-create dictionary if we have items above threshold OR if threshold is 0
        if (_dictionaryCreationThreshold == 0 || Count > _dictionaryCreationThreshold)
        {
            CreateDictionary();
        }
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
    /// Gets a value indicating whether gets whether the dictionary has been created.
    /// </summary>
    protected bool IsDictionaryCreated => _dict is not null;

    /// <summary>
    /// Gets the item associated with the specified key, or default if the key is not present.
    /// </summary>
    /// <param name="key">The key of the item to get.</param>
    /// <returns>The item associated with the specified key, or default if not found.</returns>
    public T? this[TKey key]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(key);

            // Fast path: Use dictionary if available
            if (_dict is not null)
            {
                return _dict.TryGetValue(key, out var value) ? value : default;
            }

            // Slow path: Linear search
            // Consider creating dictionary if we're searching frequently
            EnsureDictionaryIfNeeded();

            return Items.FirstOrDefault(x => Comparer.Equals(GetKeyForItem(x), key));
        }
    }

    /// <summary>
    /// Determines whether the collection contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the collection.</param>
    /// <returns>True if an element with the key exists; otherwise false.</returns>
    public bool Contains(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (_dict is not null)
        {
            return _dict.ContainsKey(key);
        }

        EnsureDictionaryIfNeeded();
        return Items.Any(x => Comparer.Equals(GetKeyForItem(x), key));
    }

    /// <summary>
    /// Attempts to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <param name="value">The value if found.</param>
    /// <returns>True if the key was found; otherwise false.</returns>
    public bool TryGetValue(TKey key, out T? value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (_dict is not null)
        {
            return _dict.TryGetValue(key, out value);
        }

        EnsureDictionaryIfNeeded();

        value = Items.FirstOrDefault(x => Comparer.Equals(GetKeyForItem(x), key));
        return value is not null;
    }

    /// <summary>
    /// Attempts to add an item to the collection if its key is not already present.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>True if the item was added; false if the key was null or already exists.</returns>
    public bool TryAdd(T item)
    {
        var key = GetKeyForItem(item);
        if (key is null) return false;

        EnsureDictionaryIfNeeded();

        if (_dict?.ContainsKey(key) == true)
            return false;

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
        ArgumentNullException.ThrowIfNull(key);

        // Fast path with dictionary
        if (_dict is not null)
        {
            return _dict.TryGetValue(key, out var item) && Remove(item);
        }

        // Slow path without dictionary
        for (var i = 0; i < Items.Count; i++)
        {
            if (!Comparer.Equals(GetKeyForItem(Items[i]), key)) continue;
            RemoveItem(i);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Changes the key associated with an existing item in the collection.
    /// </summary>
    /// <param name="item">The item whose key is changing.</param>
    /// <param name="newKey">The new key to associate with the item.</param>
    protected void ChangeItemKey(T item, TKey? newKey)
        => ChangeItemKey(item, newKey, default);

    /// <summary>
    /// Changes the key associated with an existing item in the collection.
    /// </summary>
    /// <param name="item">The item whose key is changing.</param>
    /// <param name="newKey">The new key to associate with the item.</param>
    /// <param name="oldKey">The old key to remove. If default, GetKeyForItem is used.</param>
    protected void ChangeItemKey(T item, TKey? newKey, TKey? oldKey)
    {
        if (!ContainsItem(item))
            return;

        // Use provided oldKey or get it from item
        oldKey ??= GetKeyForItem(item);

        if (Comparer.Equals(oldKey, newKey))
            return;

        ExecuteWriteLocked(() =>
        {
            if (newKey is not null)
            {
                AddKeyInternal(newKey, item);
            }

            if (oldKey is not null)
            {
                RemoveKeyInternal(oldKey);
            }
        });
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

        ExecuteWriteLocked(() =>
        {
            // Add to base collection first
            base.InsertItem(index, item);

            // Then handle dictionary
            if (key is not null)
            {
                // Check threshold AFTER item is added
                // Special case: if threshold is 0, always use dictionary
                if (_dict is not null ||
                    (_dictionaryCreationThreshold == 0 && Count > 0) ||
                    Count > _dictionaryCreationThreshold)
                {
                    if (_dict is null)
                    {
                        CreateDictionary();
                    }
                    else
                    {
                        _dict.Add(key, item);
                    }
                }
            }
        });
    }

    /// <summary>
    /// Inserts an item directly into the underlying Items collection without dictionary handling.
    /// Use with caution - this bypasses key tracking.
    /// </summary>
    /// <param name="index">The position at which to insert the item.</param>
    /// <param name="item">The item to insert.</param>
    protected void InsertItemInItems(int index, T item) => base.InsertItem(index, item);

    /// <summary>
    /// Adds the elements of the specified collection to the end of the collection.
    /// Overridden to ensure dictionary is updated.
    /// </summary>
    /// <param name="collection">The collection whose elements should be added.</param>
    public new void AddRange(IEnumerable<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        ExecuteWriteLocked(() =>
        {
            var initialCount = Count;

            // Call parent AddRange which adds to Items directly
            base.AddRange(collection);

            // Now update dictionary with all new items
            if (Count > initialCount)
            {
                // Check if we should create dictionary
                if (_dict is null && (_dictionaryCreationThreshold == 0 || Count > _dictionaryCreationThreshold))
                {
                    // Create dictionary and it will include all items
                    CreateDictionary();
                }
                else if (_dict is not null)
                {
                    // Dictionary exists, add only the new items
                    // Items were added from initialCount to Count-1
                    for (var i = initialCount; i < Count; i++)
                    {
                        var item = Items[i];
                        var key = GetKeyForItem(item);
                        if (key is not null)
                        {
                            _dict.TryAdd(key, item); // Use TryAdd to avoid exceptions on duplicates
                        }
                    }
                }
            }
        });
    }

    /// <summary>
    /// Removes the items with the specified keys from the collection.
    /// </summary>
    /// <param name="keys">The keys of the items to remove.</param>
    /// <returns>The number of items removed.</returns>
    public int RemoveRange(IEnumerable<TKey> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var set = new HashSet<TKey>(keys, Comparer);

        var removedCount = 0;

        ExecuteWriteLocked(() =>
        {
            // First, remove from dictionary
            if (_dict is not null)
            {
                foreach (var key in set)
                {
                    if (_dict.Remove(key))
                    {
                        removedCount++;
                    }
                }
            }

            // Then, remove from base items collection
            for (var i = Items.Count - 1; i >= 0; i--)
            {
                var key = GetKeyForItem(Items[i]);
                if (key is not null && set.Contains(key))
                {
                    RemoveItem(i);
                    removedCount++;
                }
            }
        });

        return removedCount;
    }

    protected override void RemoveItem(int index)
        => ExecuteWriteLocked(() =>
        {
            var key = GetKeyForItem(Items[index]);

            base.RemoveItem(index);

            if (key is not null)
            {
                RemoveKeyInternal(key);
            }
        });

    protected override void SetItem(int index, T item)
        => ExecuteWriteLocked(() =>
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
                    EnsureDictionaryIfNeeded();
                    AddKeyInternal(newKey, item);
                }

                if (oldKey is not null)
                {
                    RemoveKeyInternal(oldKey);
                }
            }

            base.SetItem(index, item);
        });

    /// <summary>
    /// Ensures the dictionary is created if the collection size warrants it.
    /// </summary>
    private void EnsureDictionaryIfNeeded()
    {
        if (_dict is null && Count >= _dictionaryCreationThreshold)
        {
            CreateDictionary();
        }
    }

    /// <summary>
    /// Forces creation of the dictionary regardless of size.
    /// </summary>
    protected void CreateDictionaryNow()
    {
        if (_dict is null)
        {
            CreateDictionary();
        }
    }

    private bool ContainsItem(T item)
    {
        var key = GetKeyForItem(item);

        return _dict is null || key is null
            ? Items.Contains(item)
            : _dict.TryGetValue(key, out var itemInDict) && EqualityComparer<T>.Default.Equals(itemInDict, item);
    }

    /// <summary>
    /// Internal add key without additional locking (assumes already locked).
    /// </summary>
    private void AddKeyInternal(TKey key, T item)
    {
        if (_dict is null)
        {
            CreateDictionary();
        }

        _dict?.Add(key, item);
    }

    /// <summary>
    /// Internal remove key without additional locking (assumes already locked).
    /// </summary>
    private void RemoveKeyInternal(TKey key) => _dict?.Remove(key);

    private void CreateDictionary()
    {
        // Pre-allocate with current count + some headroom
        var capacity = Math.Max(Count, 16);
        _dict = new Dictionary<TKey, T>(capacity, Comparer);

        foreach (var item in Items)
        {
            var key = GetKeyForItem(item);
            if (key is not null)
            {
                // Use TryAdd to avoid exceptions if duplicate keys exist
                _dict.TryAdd(key, item);
            }
        }
    }

    /// <summary>
    /// Gets statistics about the dictionary usage.
    /// </summary>
    protected (bool Created, int Count, int Capacity) GetDictionaryStats()
    {
        if (_dict is null)
            return (false, 0, 0);

        return (true, _dict.Count, _dict.EnsureCapacity(0));
    }
}
