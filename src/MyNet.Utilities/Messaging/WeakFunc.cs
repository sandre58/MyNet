// -----------------------------------------------------------------------
// <copyright file="WeakFunc.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyNet.Utilities.Messaging;

/// <summary>
/// Stores a Func&lt;TResult&gt; without causing a hard reference
/// to be created to the Function's owner. The owner can be garbage collected at any time.
/// </summary>
/// <typeparam name="TResult">The type of the result of the Func that will be stored by this weak reference.</typeparam>
public class WeakFunc<TResult>
{
    private Func<TResult?>? _staticFunc;
    private WeakReference? _targetReference;
    private WeakReference? _funcTargetReference;
    private object? _liveReference;
    private MethodInfo? _method;

    public WeakFunc(Func<TResult?> func, bool keepTargetAlive = false)
        : this(func.Target, func, keepTargetAlive)
    {
    }

    public WeakFunc(object? target, Func<TResult?> func, bool keepTargetAlive = false)
    {
        if (func.Method.IsStatic)
        {
            _staticFunc = func;
            if (target != null)
                _targetReference = new(target);
            return;
        }

        _method = func.Method;
        _funcTargetReference = new(func.Target);
        _liveReference = keepTargetAlive ? func.Target : null;
        _targetReference = new(target);
    }

    /// <summary>
    /// Gets a value indicating whether the WeakFunc is static or not.
    /// </summary>
    public bool IsStatic => _staticFunc != null;

    /// <summary>
    /// Gets the name of the method that this WeakFunc represents.
    /// </summary>
    public string? MethodName => _staticFunc?.Method.Name ?? _method?.Name;

    /// <summary>
    /// Gets a value indicating whether the Function's owner is still alive.
    /// </summary>
    public bool IsAlive =>
        (_staticFunc != null || _targetReference != null || _liveReference != null) && (_staticFunc != null ? _targetReference?.IsAlive != false : _liveReference != null || _targetReference is { IsAlive: true });

    /// <summary>
    /// Gets the Function's owner. This object is stored as a WeakReference.
    /// </summary>
    public object? Target => _targetReference?.Target;

    protected object? FuncTarget => _liveReference ?? _funcTargetReference?.Target;

    /// <summary>
    /// Executes the Func. This only happens if the Function's owner is still alive.
    /// </summary>
    /// <returns>The result of the Func stored as reference.</returns>
    public TResult? Execute()
    {
        if (!IsAlive)
            return default;

        if (_staticFunc != null)
            return _staticFunc();

        var funcTarget = FuncTarget;
        if (_method != null && funcTarget != null)
        {
            try
            {
                return (TResult?)_method.Invoke(funcTarget, null);
            }
            catch
            {
                return default;
            }
        }

        return default;
    }

    /// <summary>
    /// Sets all references to null, marking this WeakFunc for deletion.
    /// </summary>
    public void MarkForDeletion()
    {
        _targetReference = null;
        _funcTargetReference = null;
        _liveReference = null;
        _method = null;
        _staticFunc = null;
    }
}

/// <summary>
/// Stores a Func without causing a hard reference to be created to the Function's owner.
/// The owner can be garbage collected at any time.
/// </summary>
/// <typeparam name="T">The type of the Function's parameter.</typeparam>
/// <typeparam name="TResult">The type of the Function's return value.</typeparam>
public class WeakFunc<T, TResult> : IExecuteWithObjectAndResult
{
    private Func<T?, TResult?>? _staticFunc;
    private WeakReference? _targetReference;
    private WeakReference? _funcTargetReference;
    private object? _liveReference;
    private MethodInfo? _method;

    public WeakFunc(Func<T?, TResult?> func, bool keepTargetAlive = false)
        : this(func.Target, func, keepTargetAlive)
    {
    }

    public WeakFunc(object? target, Func<T?, TResult?> func, bool keepTargetAlive = false)
    {
        if (func.Method.IsStatic)
        {
            _staticFunc = func;
            if (target != null)
                _targetReference = new(target);
            return;
        }

        _method = func.Method;
        _funcTargetReference = new(func.Target);
        _liveReference = keepTargetAlive ? func.Target : null;
        _targetReference = new(target);
    }

    /// <summary>
    /// Gets the name of the method that this WeakFunc represents.
    /// </summary>
    public string? MethodName => _staticFunc?.Method.Name ?? _method?.Name;

    /// <summary>
    /// Gets a value indicating whether the Function's owner is still alive.
    /// </summary>
    public bool IsAlive => (_staticFunc != null || _targetReference != null || _liveReference != null) && (_staticFunc != null ? _targetReference?.IsAlive != false : _liveReference != null || _targetReference is { IsAlive: true });

    /// <summary>
    /// Gets the Function's owner. This object is stored as a WeakReference.
    /// </summary>
    public object? Target => _targetReference?.Target;

    protected object? FuncTarget => _liveReference ?? _funcTargetReference?.Target;

    /// <summary>
    /// Executes the Func. This only happens if the Function's owner is still alive.
    /// The Function's parameter is set to default(T).
    /// </summary>
    /// <returns>The result of the Func stored as reference.</returns>
    public TResult? Execute() => Execute(default);

    /// <summary>
    /// Executes the Func. This only happens if the Function's owner is still alive.
    /// </summary>
    /// <param name="parameter">A parameter to be passed to the function.</param>
    /// <returns>The result of the Func stored as reference.</returns>
    public TResult? Execute(T? parameter)
    {
        if (!IsAlive)
            return default;

        if (_staticFunc != null)
        {
            try
            {
                return _staticFunc(parameter);
            }
            catch
            {
                return default;
            }
        }

        var funcTarget = FuncTarget;
        if (_method != null && funcTarget != null)
        {
            try
            {
                return (TResult?)_method.Invoke(funcTarget, [parameter]);
            }
            catch
            {
                return default;
            }
        }

        return default;
    }

    /// <summary>
    /// Executes the Func with a parameter of type object. This parameter
    /// will be cast to T. This method implements <see cref="IExecuteWithObjectAndResult.ExecuteWithObject" />
    /// and can be useful if you store multiple WeakFunc{T, TResult} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    /// <param name="parameter">The parameter that will be passed to the Func after being cast to T.</param>
    /// <returns>The result of the execution as object, to be cast to TResult.</returns>
    public object? ExecuteWithObject(object? parameter)
    {
        var parameterCasted = (T?)parameter;
        return Execute(parameterCasted);
    }

    /// <summary>
    /// Sets all references to null, marking this WeakFunc for deletion.
    /// </summary>
    public void MarkForDeletion()
    {
        _staticFunc = null;
        _targetReference = null;
        _funcTargetReference = null;
        _liveReference = null;
        _method = null;
    }
}
