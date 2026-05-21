// -----------------------------------------------------------------------
// <copyright file="CacheStorage.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MyNet.Utilities.Caching.Policies;

namespace MyNet.Utilities.Caching;

/// <summary>
/// The cache storage.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="CacheStorage{TKey,TValue}" /> class.
/// </remarks>
/// <param name="defaultExpirationPolicyInitCode">The default expiration policy initialization code.</param>
/// <param name="storeNullValues">Allow store null values on the cache.</param>
/// <param name="equalityComparer">The equality comparer.</param>
public sealed class CacheStorage<TKey, TValue>(Func<ExpirationPolicy>? defaultExpirationPolicyInitCode = null, bool storeNullValues = false,
    IEqualityComparer<TKey>? equalityComparer = null) : ICacheStorage<TKey, TValue>
    where TKey : notnull
{
    #region Fields

    /// <summary>
    /// Determines whether the cache storage can store null values.
    /// </summary>
    private readonly bool _storeNullValues = storeNullValues;

    /// <summary>
    /// The dictionary.
    /// </summary>
    private readonly Dictionary<TKey, CacheStorageValueInfo<TValue>> _dictionary = new(equalityComparer ?? EqualityComparer<TKey>.Default);

    /// <summary>
    /// The synchronization object.
    /// </summary>
    private readonly Lock _syncObj = new();

    /// <summary>
    /// The timer that is being executed to invalidate the cache.
    /// </summary>
    private Timer? _expirationTimer;

    /// <summary>
    /// Determines whether the cache storage can check for expired items.
    /// </summary>
    private bool _checkForExpiredItems;

    #endregion

    /// <summary>
    /// Occurs when the item is expiring.
    /// </summary>
    public event EventHandler<ExpiringEventArgs<TKey, TValue>>? Expiring;

    /// <summary>
    /// Occurs when the item has expired.
    /// </summary>
    public event EventHandler<ExpiredEventArgs<TKey, TValue>>? Expired;

    #region ICacheStorage<TKey,TValue> Members

    /// <summary>
    /// Gets or sets a value indicating whether values should be disposed on removal.
    /// </summary>
    /// <value><c>true</c> if values should be disposed on removal; otherwise, <c>false</c>.</value>
    public bool DisposeValuesOnRemoval { get; set; }

    /// <summary>
    /// Gets the keys so it is possible to enumerate the cache.
    /// </summary>
    /// <value>The keys.</value>
    public IEnumerable<TKey> Keys => GetKeysSnapshot();

    /// <summary>
    /// Gets or sets the expiration timer interval.
    /// <para />
    /// The default value is <c>TimeSpan.FromSeconds(1)</c>.
    /// </summary>
    /// <value>The expiration timer interval.</value>
    public TimeSpan ExpirationTimerInterval
    {
        get
        {
            lock (_syncObj)
            {
                return _expirationTimerInterval;
            }
        }

        set
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Expiration timer interval must be greater than zero.");
            }

            lock (_syncObj)
            {
                _expirationTimerInterval = value;
                if (_checkForExpiredItems)
                {
                    UpdateTimerUnsafe();
                }
            }
        }
    }

    private TimeSpan _expirationTimerInterval = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The value associated with the specified key, or default value for the type of the value if the key do not exist.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
    public TValue? this[TKey key] => Get(key);

    /// <summary>
    /// Gets the value associated with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or default value for the type of the value if the key do not exist.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
    public TValue? Get(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_syncObj)
        {
            return !TryGetValueInfoUnsafe(key, out var valueInfo) ? default : valueInfo.Value;
        }
    }

    /// <summary>
    /// Determines whether the cache contains a value associated with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if the cache contains an element with the specified key; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
    public bool Contains(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_syncObj)
        {
            return TryGetValueInfoUnsafe(key, out _);
        }
    }

    /// <summary>
    /// Adds a value to the cache associated with to a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="code">The deferred initialization code of the value.</param>
    /// <param name="expirationPolicy">The expiration policy.</param>
    /// <param name="override">Indicates if the key exists the value will be overridden.</param>
    /// <returns>The instance initialized by the <paramref name="code" />.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
    public TValue GetFromCacheOrFetch(TKey key, Func<TValue> code, ExpirationPolicy? expirationPolicy, bool @override = false)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(code);

        lock (_syncObj)
        {
            if (!@override && TryGetValueInfoUnsafe(key, out var cacheStorageValueInfo))
            {
                return cacheStorageValueInfo.Value;
            }

            var value = code();
            if (value is null && !_storeNullValues)
            {
                return value;
            }

            expirationPolicy ??= defaultExpirationPolicyInitCode?.Invoke();

            _dictionary[key] = new(value, expirationPolicy);
            UpdateExpirationStateUnsafe();

            return value;
        }
    }

    /// <summary>
    /// Adds a value to the cache associated with to a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="code">The deferred initialization code of the value.</param>
    /// <param name="override">Indicates if the key exists the value will be overridden.</param>
    /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
    /// <returns>The instance initialized by the <paramref name="code" />.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
    public TValue GetFromCacheOrFetch(TKey key, Func<TValue> code, bool @override = false, TimeSpan expiration = default) => GetFromCacheOrFetch(key, code, ExpirationPolicy.Duration(expiration), @override);

    /// <summary>
    /// Adds a value to the cache associated with to a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="override">Indicates if the key exists the value will be overridden.</param>
    /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
    public void Add(TKey key, TValue value, bool @override = false, TimeSpan expiration = default) => Add(key, value, ExpirationPolicy.Duration(expiration), @override);

    /// <summary>
    /// Adds a value to the cache associated with to a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="expirationPolicy">The expiration policy.</param>
    /// <param name="override">Indicates if the key exists the value will be overridden.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
    public void Add(TKey key, TValue value, ExpirationPolicy? expirationPolicy, bool @override = false) => GetFromCacheOrFetch(key, () => value, expirationPolicy, @override);

    /// <summary>
    /// Removes an item from the cache.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="action">The action that need to be executed in synchronization with the item cache removal.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
    public void Remove(TKey key, Action? action = null)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_syncObj)
        {
            _ = RemoveItemUnsafe(key, false, action);
        }
    }

    /// <summary>
    /// Clears all the items currently in the cache.
    /// </summary>
    public void Clear()
    {
        lock (_syncObj)
        {
            var keysToRemove = _dictionary.Keys.ToList();
            foreach (var keyToRemove in keysToRemove)
            {
                _ = RemoveItemUnsafe(keyToRemove, false);
            }

            UpdateExpirationStateUnsafe();
        }
    }

    private TKey[] GetKeysSnapshot()
    {
        lock (_syncObj)
        {
            return [.. _dictionary.Keys];
        }
    }

    /// <summary>
    /// Removes the expired items from the cache.
    /// </summary>
    private void RemoveExpiredItems()
    {
        lock (_syncObj)
        {
            if (!_checkForExpiredItems)
            {
                return;
            }

            var keysToRemove = new List<TKey>();
            foreach (var (key, valueInfo) in _dictionary)
            {
                if (valueInfo.IsExpired)
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var keyToRemove in keysToRemove)
            {
                _ = RemoveItemUnsafe(keyToRemove, true);
            }

            UpdateExpirationStateUnsafe();
        }
    }

    /// <summary>
    /// Called when the timer to clean up the cache elapsed.
    /// </summary>
    /// <param name="state">The timer state.</param>
    private void OnTimerElapsed(object? state) => RemoveExpiredItems();

    private bool TryGetValueInfoUnsafe(TKey key, out CacheStorageValueInfo<TValue> valueInfo)
    {
        if (!_dictionary.TryGetValue(key, out var cacheStorageValueInfo))
        {
            valueInfo = null!;
            return false;
        }

        valueInfo = cacheStorageValueInfo;

        if (!valueInfo.IsExpired)
        {
            return true;
        }

        _ = RemoveItemUnsafe(key, true);

        valueInfo = null!;
        return false;
    }

    /// <summary>
    /// Remove item from cache by key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="raiseEvents">Indicates whether events should be raised.</param>
    /// <param name="action">The action that need to be executed in synchronization with the item cache removal.</param>
    /// <returns>The value indicating whether the item was removed.</returns>
    private bool RemoveItemUnsafe(TKey key, bool raiseEvents, Action? action = null)
    {
        // Try to get item, if there is no item by that key then return true to indicate that item was removed.
        if (!_dictionary.TryGetValue(key, out var item))
        {
            return true;
        }

        action?.Invoke();

        var cancel = false;
        var expirationPolicy = item.ExpirationPolicy;
        if (raiseEvents)
        {
            var expiringEventArgs = new ExpiringEventArgs<TKey, TValue>(key, item.Value, expirationPolicy);
            Expiring?.Invoke(this, expiringEventArgs);

            cancel = expiringEventArgs.Cancel;
            expirationPolicy = expiringEventArgs.ExpirationPolicy;
        }

        if (cancel)
        {
            expirationPolicy ??= defaultExpirationPolicyInitCode?.Invoke();

            _dictionary[key] = new(item.Value, expirationPolicy);
            UpdateExpirationStateUnsafe();

            return false;
        }

        _ = _dictionary.Remove(key);

        var dispose = DisposeValuesOnRemoval;
        if (raiseEvents)
        {
            var expiredEventArgs = new ExpiredEventArgs<TKey, TValue>(key, item.Value, dispose);
            Expired?.Invoke(this, expiredEventArgs);

            dispose = expiredEventArgs.Dispose;
        }

        if (dispose)
        {
            item.DisposeValue();
        }

        UpdateExpirationStateUnsafe();

        return true;
    }

    private void UpdateExpirationStateUnsafe()
    {
        var containsItemsThatCanExpire = _dictionary.Values.Any(x => x.CanExpire);
        if (_checkForExpiredItems == containsItemsThatCanExpire)
        {
            return;
        }

        _checkForExpiredItems = containsItemsThatCanExpire;
        UpdateTimerUnsafe();
    }

    private void UpdateTimerUnsafe()
    {
        if (!_checkForExpiredItems)
        {
            _expirationTimer?.Dispose();
            _expirationTimer = null;
            return;
        }

        var timeSpan = _expirationTimerInterval;
        _expirationTimer ??= new(OnTimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
        _ = _expirationTimer.Change(timeSpan, timeSpan);
    }
    #endregion
}
