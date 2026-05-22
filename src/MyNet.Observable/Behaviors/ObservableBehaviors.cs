// -----------------------------------------------------------------------
// <copyright file="ObservableBehaviors.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Facade for registering and accessing behaviors on an <see cref="ObservableObject"/>.
/// </summary>
public sealed class ObservableBehaviors
{
    private readonly BehaviorRegistry _registry;

    internal ObservableBehaviors(BehaviorRegistry registry) => _registry = registry;

    /// <summary>
    /// Registers or replaces a behavior for the computed key.
    /// </summary>
    public void Register<T>(T behavior, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.Register(behavior, propertyName, scope);

    /// <summary>
    /// Removes a behavior for the specified key.
    /// </summary>
    public bool Unregister<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.Unregister<T>(propertyName, scope);

    /// <summary>
    /// Tries to get a single behavior matching the optional key parts.
    /// </summary>
    public bool TryGet<T>([NotNullWhen(true)] out T? behavior, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.TryGet(out behavior, propertyName, scope);

    /// <summary>
    /// Gets all behaviors assignable to <typeparamref name="T"/> matching the optional key parts.
    /// </summary>
    public T[] GetAll<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.GetAll<T>(propertyName, scope);

    /// <summary>
    /// Gets a single behavior or throws when not found or ambiguous.
    /// </summary>
    public T Get<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.Get<T>(propertyName, scope);

    /// <summary>
    /// Returns whether a single behavior matches the optional key parts.
    /// </summary>
    public bool Has<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.Has<T>(propertyName, scope);

    /// <summary>
    /// Gets a behavior or <c>null</c> when not found or ambiguous.
    /// </summary>
    public T? GetOrDefault<T>(string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.GetOrDefault<T>(propertyName, scope);

    /// <summary>
    /// Executes an action when a single matching behavior exists.
    /// </summary>
    public bool TryExecute<T>(Action<T> action, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.TryExecute(action, propertyName, scope);

    /// <summary>
    /// Executes an action on a single matching behavior.
    /// </summary>
    public void Execute<T>(Action<T> action, string? propertyName = null, string? scope = null)
        where T : class, IObservableBehavior
        => _registry.Execute(action, propertyName, scope);

    /// <summary>
    /// Evaluates a selector when a single matching behavior exists.
    /// </summary>
    public bool TryEvaluate<TBehavior, TResult>(Func<TBehavior, TResult> selector, [MaybeNullWhen(false)] out TResult result, string? propertyName = null, string? scope = null)
        where TBehavior : class, IObservableBehavior
        => _registry.TryEvaluate(selector, out result, propertyName, scope);

    /// <summary>
    /// Evaluates a selector on a single matching behavior, or returns <paramref name="defaultValue"/>.
    /// </summary>
    public TResult Evaluate<TBehavior, TResult>(Func<TBehavior, TResult> selector, TResult defaultValue = default!, string? propertyName = null, string? scope = null)
        where TBehavior : class, IObservableBehavior
        => _registry.Evaluate(selector, defaultValue, propertyName, scope);
}
