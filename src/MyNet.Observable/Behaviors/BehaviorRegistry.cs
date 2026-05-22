// -----------------------------------------------------------------------
// <copyright file="BehaviorRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Registry for behaviors attached to an <see cref="ObservableObject"/>.
/// Each entry is keyed by <see cref="BehaviorKey"/>; registration order defines pipeline execution order.
/// </summary>
internal sealed class BehaviorRegistry : IDisposable
{
    private readonly Dictionary<BehaviorKey, IObservableBehavior> _behaviors = [];
    private readonly List<BehaviorKey> _registrationOrder = [];
    private readonly Lock _gate = new();
    private bool _disposed;

    private static void DisposeBehavior(IObservableBehavior behavior)
    {
        if (behavior is IDisposable disposable)
            disposable.Dispose();
    }

    /// <summary>
    /// Gets property-changing behaviors in registration order.
    /// </summary>
    public IPropertyChangingBehavior[] Changing { get; private set; } = [];

    /// <summary>
    /// Gets property-changed behaviors in registration order.
    /// </summary>
    public IPropertyChangedBehavior[] Changed { get; private set; } = [];

    /// <summary>
    /// Gets all registered behavior instances in registration order.
    /// </summary>
    public IObservableBehavior[] All
    {
        get
        {
            lock (_gate)
            {
                return [.. _registrationOrder.Select(k => _behaviors[k])];
            }
        }
    }

    #region Register / unregister

    /// <summary>
    /// Registers or replaces a behavior for the computed key. Previous instances are disposed when <see cref="IDisposable"/>; registration order is preserved on replace.
    /// </summary>
    public void Register(IObservableBehavior behavior, string? propertyName = null, string? scope = null)
    {
        ArgumentNullException.ThrowIfNull(behavior);
        ThrowIfDisposed();

        lock (_gate)
        {
            var key = BehaviorKey.Create(behavior.GetType(), scope, propertyName);

            if (_behaviors.TryGetValue(key, out var existing) && !ReferenceEquals(existing, behavior))
                DisposeBehavior(existing);

            if (!_behaviors.ContainsKey(key))
                _registrationOrder.Add(key);

            _behaviors[key] = behavior;
            RebuildPipeline();
        }
    }

    /// <summary>
    /// Removes a behavior for the specified key and disposes it when <see cref="IDisposable"/>.
    /// </summary>
    public bool Unregister<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        ThrowIfDisposed();

        lock (_gate)
        {
            var key = BehaviorKey.Create(typeof(T), scope, propertyName);

            if (!_behaviors.Remove(key, out var existing))
                return false;

            _registrationOrder.Remove(key);
            DisposeBehavior(existing);
            RebuildPipeline();
            return true;
        }
    }

    #endregion

    #region Lookup

    /// <summary>
    /// Tries to get a behavior using an exact or filtered key match. Returns false when zero or more than one instance matches.
    /// </summary>
    public bool TryGet<T>([NotNullWhen(true)] out T? behavior, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        lock (_gate)
        {
            var key = BehaviorKey.Create(typeof(T), scope, propertyName);

            if (_behaviors.TryGetValue(key, out var exact) && exact is T typed)
            {
                behavior = typed;
                return true;
            }

            var matches = CollectMatches<T>(propertyName, scope);

            if (matches.Count == 1)
            {
                behavior = matches[0];
                return true;
            }

            behavior = null;
            return false;
        }
    }

    /// <summary>
    /// Gets all registered behaviors assignable to <typeparamref name="T"/> that match the optional scope and property name filter.
    /// </summary>
    public T[] GetAll<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        lock (_gate)
        {
            return [.. CollectMatches<T>(propertyName, scope)];
        }
    }

    public T Get<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => TryGet<T>(out var b, propertyName, scope)
            ? b
            : throw new InvalidOperationException(
                $"Behavior {typeof(T).Name} not found for propertyName '{propertyName ?? "<null>"}' and scope '{scope ?? "<null>"}'.");

    public bool Has<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => TryGet<T>(out _, propertyName, scope);

    public T? GetOrDefault<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        TryGet<T>(out var behavior, propertyName, scope);
        return behavior;
    }

    #endregion

    #region Api

    public bool TryExecute<T>(Action<T> action, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!TryGet<T>(out var behavior, propertyName, scope))
            return false;

        action(behavior);
        return true;
    }

    public void Execute<T>(Action<T> action, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
    {
        ArgumentNullException.ThrowIfNull(action);
        action(Get<T>(propertyName, scope));
    }

    public bool TryEvaluate<TBehavior, TResult>(Func<TBehavior, TResult> selector, [MaybeNullWhen(false)] out TResult result, string? propertyName = null, string? scope = null)
        where TBehavior : class, IObservableBehavior
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!TryGet<TBehavior>(out var behavior, propertyName, scope))
        {
            result = default;
            return false;
        }

        result = selector(behavior);
        return true;
    }

    public TResult Evaluate<TBehavior, TResult>(Func<TBehavior, TResult> selector, TResult defaultValue = default!, string? propertyName = null, string? scope = null)
        where TBehavior : class, IObservableBehavior
    {
        ArgumentNullException.ThrowIfNull(selector);
        return TryGet<TBehavior>(out var behavior, propertyName, scope) ? selector(behavior) : defaultValue;
    }

    #endregion

    #region Pipeline

    private void RebuildPipeline()
    {
        var changing = new List<IPropertyChangingBehavior>();
        var changed = new List<IPropertyChangedBehavior>();

        foreach (var key in _registrationOrder)
        {
            var behavior = _behaviors[key];

            if (behavior is IPropertyChangingBehavior changingBehavior)
                changing.Add(changingBehavior);

            if (behavior is IPropertyChangedBehavior changedBehavior)
                changed.Add(changedBehavior);
        }

        Changing = [.. changing];
        Changed = [.. changed];
    }

    private List<T> CollectMatches<T>(string? propertyName, string? scope)
        where T : class, IObservableBehavior
    {
        var matches = new List<T>();

        foreach (var entry in _behaviors)
        {
            if (entry.Value is not T typed)
                continue;

            if (!entry.Key.Matches(scope, propertyName))
                continue;

            matches.Add(typed);
        }

        return matches;
    }

    #endregion

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        lock (_gate)
        {
            foreach (var behavior in _behaviors.Values.OfType<IDisposable>())
                behavior.Dispose();

            _behaviors.Clear();
            _registrationOrder.Clear();
            RebuildPipeline();
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, nameof(BehaviorRegistry));
}

/// <summary>
/// Identifies a behavior instance in <see cref="BehaviorRegistry"/>.
/// </summary>
/// <param name="BehaviorType">Concrete behavior type used at registration.</param>
/// <param name="Scope">Optional scope discriminator (for example behavior kind name).</param>
/// <param name="PropertyName">Optional property name the behavior is bound to.</param>
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Properties are used for equality and hashing.")]
public readonly record struct BehaviorKey(Type BehaviorType, string? Scope = null, string? PropertyName = null)
{
    private static string? Normalize(string? value) => string.IsNullOrEmpty(value) ? null : value;

    /// <summary>
    /// Creates a normalized key for the given behavior type and optional scope/property identifiers.
    /// </summary>
    public static BehaviorKey Create(Type behaviorType, string? scope = null, string? propertyName = null)
        => new(behaviorType, Normalize(scope), Normalize(propertyName));

    /// <summary>
    /// Returns whether this key matches the optional scope and property filters.
    /// When both filters are null, only keys without scope and property name match.
    /// </summary>
    public bool Matches(string? scope, string? propertyName)
    {
        var normalizedScope = Normalize(scope);
        var normalizedProperty = Normalize(propertyName);

        if (normalizedScope is null && normalizedProperty is null)
            return Scope is null && PropertyName is null;

        if (normalizedScope is not null && Scope != normalizedScope)
            return false;

        if (normalizedProperty is not null && PropertyName != normalizedProperty)
            return false;

        return true;
    }
}
