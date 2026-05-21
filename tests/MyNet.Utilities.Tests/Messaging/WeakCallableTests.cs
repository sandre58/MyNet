// -----------------------------------------------------------------------
// <copyright file="WeakCallableTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MyNet.Utilities.Messaging;
using Xunit;

namespace MyNet.Utilities.Tests.Messaging;

public class WeakCallableTests
{
    [Fact]
    public void StaticDelegate_Execute_ReturnsValue()
    {
        var callable = CreateWeakCallable<Func<int>>(StaticValue);

        var result = InvokeExecute(callable);

        Assert.Equal(42, result);
        Assert.True((bool)GetProperty(callable, "IsStatic")!);
        Assert.True((bool)GetProperty(callable, "IsAlive")!);
        Assert.Equal(nameof(StaticValue), GetProperty(callable, "MethodName"));
    }

    [Fact]
    public void InstanceDelegate_Execute_ReturnsValue()
    {
        var target = new InstanceTarget();
        var callable = CreateWeakCallable<Func<int>>(target, target.GetValue);

        var result = InvokeExecute(callable);

        Assert.Equal(7, result);
        Assert.False((bool)GetProperty(callable, "IsStatic")!);
    }

    [Fact]
    public void Execute_WhenDelegateThrows_ReturnsNull()
    {
        var callable = CreateWeakCallable<Func<int>>(ThrowingStatic);

        var result = InvokeExecute(callable);

        Assert.Null(result);
    }

    [Fact]
    public void MarkForDeletion_SetsIsAliveFalse_AndExecuteReturnsNull()
    {
        var callable = CreateWeakCallable<Func<int>>(StaticValue);

        InvokeMethod(callable, "MarkForDeletion", []);

        Assert.False((bool)GetProperty(callable, "IsAlive")!);
        Assert.Null(InvokeExecute(callable));
    }

    [Fact]
    public void GetDelegate_ReturnsStaticDelegateAndNullForInstance()
    {
        var staticCallable = CreateWeakCallable<Func<int>>(StaticValue);
        var target = new InstanceTarget();
        var instanceCallable = CreateWeakCallable<Func<int>>(target, target.GetValue);

        Assert.NotNull(InvokeMethod(staticCallable, "GetDelegate", []));
        Assert.Null(InvokeMethod(instanceCallable, "GetDelegate", []));
    }

    private static object CreateWeakCallable<TDelegate>(TDelegate @delegate)
        where TDelegate : Delegate
    {
        var type = GetWeakCallableGenericType(typeof(TDelegate));
        return Activator.CreateInstance(type, @delegate, false)!;
    }

    private static object CreateWeakCallable<TDelegate>(object target, TDelegate @delegate)
        where TDelegate : Delegate
    {
        var type = GetWeakCallableGenericType(typeof(TDelegate));
        return Activator.CreateInstance(type, target, @delegate, false)!;
    }

    private static Type GetWeakCallableGenericType(Type delegateType)
        => typeof(Messenger).Assembly
            .GetType("MyNet.Utilities.Messaging.WeakCallable`1", throwOnError: true)!
            .MakeGenericType(delegateType);

    private static object? InvokeExecute(object callable)
        => InvokeMethod(callable, "Execute", [Array.Empty<object?>()]);

    private static object? InvokeMethod(object instance, string methodName, object?[] arguments)
    {
        var method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        return method.Invoke(instance, arguments);
    }

    private static object? GetProperty(object instance, string propertyName)
    {
        var property = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        return property.GetValue(instance);
    }

    private static int StaticValue() => 42;

    private static int ThrowingStatic() => throw new InvalidOperationException();

    private sealed class InstanceTarget
    {
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Testing instance method with constant value")]
        private const int Value = 7;

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local", Justification = "Testing instance method")]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Testing instance method")]
        public int GetValue() => Value;
    }
}
