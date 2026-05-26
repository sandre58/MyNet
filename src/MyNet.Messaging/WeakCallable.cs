// -----------------------------------------------------------------------
// <copyright file="WeakCallable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyNet.Messaging;

/// <summary>
/// Base class that stores a delegate (Action or Func) without creating a hard reference
/// to the target object. The target can be garbage collected at any time.
/// Consolidates common logic for both WeakAction and WeakFunc.
/// </summary>
internal abstract class WeakCallable
{
    /// <summary>
    /// Gets a value indicating whether the callable is static or not.
    /// </summary>
    public abstract bool IsStatic { get; }

    /// <summary>
    /// Gets the name of the method that this callable represents.
    /// </summary>
    public abstract string? MethodName { get; }

    /// <summary>
    /// Gets a value indicating whether the callable's target is still alive.
    /// </summary>
    public abstract bool IsAlive { get; }

    /// <summary>
    /// Gets the target object. This object is stored as a WeakReference.
    /// </summary>
    public abstract object? Target { get; }

    /// <summary>
    /// Marks this callable for deletion. All references are cleared.
    /// </summary>
    public abstract void MarkForDeletion();
}

/// <summary>
/// Stores an Action or Func without creating a hard reference to the target object.
/// The target can be garbage collected at any time.
/// </summary>
/// <typeparam name="TDelegate">The type of the delegate (Action or Func).</typeparam>
internal sealed class WeakCallable<TDelegate> : WeakCallable
    where TDelegate : Delegate
{
    private TDelegate? _staticDelegate;
    private WeakReference? _targetReference;
    private WeakReference? _delegateTargetReference;
    private object? _liveReference;
    private MethodInfo? _method;
    private bool _isAlive = true;

    public WeakCallable(TDelegate @delegate, bool keepTargetAlive = false)
        : this(@delegate.Target, @delegate, keepTargetAlive)
    {
    }

    public WeakCallable(object? target, TDelegate @delegate, bool keepTargetAlive = false)
    {
        if (@delegate.Method.IsStatic)
        {
            _staticDelegate = @delegate;
            if (target != null)
                _targetReference = new(target);
            return;
        }

        _method = @delegate.Method;
        _delegateTargetReference = new(@delegate.Target);
        _liveReference = keepTargetAlive ? @delegate.Target : null;
        _targetReference = new(target);
    }

    public override bool IsStatic => _staticDelegate != null;

    public override string? MethodName =>
        _staticDelegate?.Method.Name ?? _method?.Name;

    public override bool IsAlive
    {
        get
        {
            if (!_isAlive)
                return false;

            if (_staticDelegate == null && _targetReference == null && _liveReference == null)
                return false;

            if (_staticDelegate != null)
                return _targetReference?.IsAlive != false;

            // Non-static delegate
            return _liveReference != null || _targetReference is { IsAlive: true };
        }
    }

    public override object? Target => _targetReference?.Target;

    private object? DelegateTarget => _liveReference ?? _delegateTargetReference?.Target;

    public override void MarkForDeletion()
    {
        _isAlive = false;
        _targetReference = null;
        _delegateTargetReference = null;
        _liveReference = null;
        _method = null;
        _staticDelegate = null;
    }

    /// <summary>
    /// Executes the delegate with reflection if it's still alive.
    /// </summary>
    public object? Execute(params object?[] parameters)
    {
        if (!IsAlive)
            return null;

        if (_staticDelegate != null)
        {
            try
            {
                return _staticDelegate.DynamicInvoke(parameters);
            }
            catch
            {
                return null;
            }
        }

        var target = DelegateTarget;
        if (_method == null || target == null)
            return null;

        try
        {
            return _method.Invoke(target, parameters);
        }
        catch
        {
            return null;
        }
    }

    public TDelegate? GetDelegate() => !IsAlive || (_staticDelegate == null && (_method == null || DelegateTarget == null)) ? null : _staticDelegate;
}
