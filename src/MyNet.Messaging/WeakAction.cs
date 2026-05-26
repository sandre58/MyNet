// -----------------------------------------------------------------------
// <copyright file="WeakAction.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyNet.Messaging;

/// <summary>
/// Stores an <see cref="Action" /> without causing a hard reference
/// to be created to the Action's owner. The owner can be garbage collected at any time.
/// </summary>
public class WeakAction
{
    private Action? _staticAction;
    private WeakReference? _targetReference;
    private WeakReference? _actionTargetReference;
    private object? _liveReference;
    private MethodInfo? _method;

    public WeakAction(Action action, bool keepTargetAlive = false)
        : this(action.Target, action, keepTargetAlive)
    {
    }

    public WeakAction(object? target, Action action, bool keepTargetAlive = false)
    {
        if (action.Method.IsStatic)
        {
            _staticAction = action;
            if (target != null)
                _targetReference = new(target);
            return;
        }

        _method = action.Method;
        _actionTargetReference = new(action.Target);
        _liveReference = keepTargetAlive ? action.Target : null;
        _targetReference = new(target);
    }

    /// <summary>
    /// Gets a value indicating whether the WeakAction is static or not.
    /// </summary>
    public bool IsStatic => _staticAction != null;

    /// <summary>
    /// Gets the name of the method that this WeakAction represents.
    /// </summary>
    public virtual string? MethodName => _staticAction?.Method.Name ?? _method?.Name;

    /// <summary>
    /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
    /// by the Garbage Collector already.
    /// </summary>
    public virtual bool IsAlive
    {
        get
        {
            if (_staticAction == null && _targetReference == null && _liveReference == null)
                return false;

            if (_staticAction != null)
                return _targetReference?.IsAlive != false;

            // Non static action
            return _liveReference != null || _targetReference is { IsAlive: true };
        }
    }

    /// <summary>
    /// Gets the Action's owner. This object is stored as a WeakReference.
    /// </summary>
    public object? Target => _targetReference?.Target;

    protected object? ActionTarget => _liveReference ?? _actionTargetReference?.Target;

    /// <summary>
    /// Executes the action. This only happens if the action's owner is still alive.
    /// </summary>
    public void Execute()
    {
        if (!IsAlive)
            return;

        if (_staticAction != null)
        {
            _staticAction();
            return;
        }

        var actionTarget = ActionTarget;
        if (_method != null && actionTarget != null)
        {
            try
            {
                _ = _method.Invoke(actionTarget, null);
            }
            catch
            {
                // Silently ignore invocation errors
            }
        }
    }

    /// <summary>
    /// Sets all references to null, marking this WeakAction for deletion.
    /// </summary>
    public void MarkForDeletion()
    {
        _staticAction = null;
        _targetReference = null;
        _actionTargetReference = null;
        _liveReference = null;
        _method = null;
    }
}

/// <summary>
/// Stores a generic Action&lt;T&gt; without causing a hard reference
/// to be created to the Action's owner. The owner can be garbage collected at any time.
/// </summary>
/// <typeparam name="T">The type of the Action's parameter.</typeparam>
public class WeakAction<T> : IExecuteWithObject
{
    private Action<T?>? _staticAction;
    private WeakReference? _targetReference;
    private WeakReference? _actionTargetReference;
    private object? _liveReference;
    private MethodInfo? _method;

    public WeakAction(Action<T?> action, bool keepTargetAlive = false)
        : this(action.Target, action, keepTargetAlive)
    {
    }

    public WeakAction(object? target, Action<T?> action, bool keepTargetAlive = false)
    {
        if (action.Method.IsStatic)
        {
            _staticAction = action;
            if (target != null)
                _targetReference = new(target);
            return;
        }

        _method = action.Method;
        _actionTargetReference = new(action.Target);
        _liveReference = keepTargetAlive ? action.Target : null;
        _targetReference = new(target);
    }

    /// <summary>
    /// Gets the name of the method that this WeakAction represents.
    /// </summary>
    public string? MethodName => _staticAction?.Method.Name ?? _method?.Name;

    /// <summary>
    /// Gets a value indicating whether the Action's owner is still alive.
    /// </summary>
    public bool IsAlive =>
        (_staticAction != null || _targetReference != null || _liveReference != null) && (_staticAction != null ? _targetReference?.IsAlive != false : _liveReference != null || _targetReference is { IsAlive: true });

    /// <summary>
    /// Gets the Action's owner. This object is stored as a WeakReference.
    /// </summary>
    public object? Target => _targetReference?.Target;

    protected object? ActionTarget => _liveReference ?? _actionTargetReference?.Target;

    /// <summary>
    /// Executes the action. This only happens if the action's owner is still alive.
    /// The action's parameter is set to default(T).
    /// </summary>
    public void Execute() => Execute(default);

    /// <summary>
    /// Executes the action. This only happens if the action's owner is still alive.
    /// </summary>
    /// <param name="parameter">A parameter to be passed to the action.</param>
    public void Execute(T? parameter)
    {
        if (!IsAlive)
            return;

        if (_staticAction != null)
        {
            try
            {
                _staticAction(parameter);
            }
            catch
            {
                // Silently ignore invocation errors
            }

            return;
        }

        var actionTarget = ActionTarget;
        if (_method != null && actionTarget != null)
        {
            try
            {
                _ = _method.Invoke(actionTarget, [parameter]);
            }
            catch
            {
                // Silently ignore invocation errors
            }
        }
    }

    /// <summary>
    /// Executes the action with a parameter of type object. This parameter
    /// will be casted to T. This method implements <see cref="IExecuteWithObject.ExecuteWithObject" />
    /// and can be useful if you store multiple WeakAction{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    /// <param name="parameter">The parameter that will be passed to the action after being casted to T.</param>
    public void ExecuteWithObject(object? parameter)
    {
        var parameterCasted = (T?)parameter;
        Execute(parameterCasted);
    }

    /// <summary>
    /// Sets all references to null, marking this WeakAction for deletion.
    /// </summary>
    public void MarkForDeletion()
    {
        _staticAction = null;
        _targetReference = null;
        _actionTargetReference = null;
        _liveReference = null;
        _method = null;
    }
}
