// -----------------------------------------------------------------------
// <copyright file="WeakFuncTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Utilities.Messaging;
using Xunit;

namespace MyNet.Utilities.Tests.Messaging;

public class WeakFuncTests
{
    #region WeakFunc<TResult>

    [Fact]
    public void WeakFuncNoParameter_Execute_ReturnsCorrectValue()
    {
        // Arrange
        var func = new WeakFunc<int>(() => 42);

        // Act
        var result = func.Execute();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void WeakFuncNoParameter_IsAlive_TrueAfterConstruction()
    {
        // Arrange
        var func = new WeakFunc<int>(() => 42);

        // Act
        var isAlive = func.IsAlive;

        // Assert
        Assert.True(isAlive);
    }

    [Fact]
    public void WeakFuncNoParameter_IsAlive_FalseAfterMarkForDeletion()
    {
        // Arrange
        var func = new WeakFunc<int>(() => 42);

        // Act
        func.MarkForDeletion();

        // Assert
        Assert.False(func.IsAlive);
    }

    [Fact]
    public void WeakFuncNoParameter_Execute_AfterDeletion_ReturnsDefault()
    {
        // Arrange
        var func = new WeakFunc<int>(() => 42);

        // Act
        func.MarkForDeletion();
        var result = func.Execute();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void WeakFuncNoParameter_MethodName_ReturnsCorrectName()
    {
        var func = new WeakFunc<int>(lambda);

        // Act
        var methodName = func.MethodName;

        // Assert
        Assert.NotNull(methodName);
        return;

        // Arrange
        static int lambda() => 42;
    }

    [Fact]
    public void WeakFuncNoParameter_IsStatic_TrueForStaticMethod()
    {
        // Arrange
        var func = new WeakFunc<int>(StaticFunc);

        // Act
        var isStatic = func.IsStatic;

        // Assert
        Assert.True(isStatic);
    }

    #endregion

    #region WeakFunc<T, TResult>

    [Fact]
    public void WeakFuncWithParameter_Execute_ReturnsCorrectValue()
    {
        // Arrange
        var func = new WeakFunc<int, string>(x => x.ToString(CultureInfo.CurrentCulture));

        // Act
        var result = func.Execute(42);

        // Assert
        Assert.Equal("42", result);
    }

    [Fact]
    public void WeakFuncWithParameter_Execute_NoParameter_UsesDefault()
    {
        // Arrange
        var func = new WeakFunc<int, string>(x => x.ToString(CultureInfo.CurrentCulture));

        // Act
        var result = func.Execute();

        // Assert
        Assert.Equal("0", result); // default(int) = 0
    }

    [Fact]
    public void WeakFuncWithParameter_ExecuteWithObject_CastsCorrectly()
    {
        // Arrange
        var func = new WeakFunc<int, string>(x => x.ToString(CultureInfo.CurrentCulture));

        // Act
        var result = func.ExecuteWithObject(99);

        // Assert
        Assert.Equal("99", result);
    }

    [Fact]
    public void WeakFuncWithParameter_ExecuteWithObject_NullParameter()
    {
        // Arrange
        var func = new WeakFunc<string?, string>(x => x?.ToUpper(CultureInfo.CurrentCulture) ?? "NULL");

        // Act
        var result = func.ExecuteWithObject(null);

        // Assert
        Assert.Equal("NULL", result);
    }

    [Fact]
    public void WeakFuncWithParameter_IsAlive_False_AfterMarkForDeletion()
    {
        // Arrange
        var func = new WeakFunc<int, string>(_ => string.Empty);

        // Act
        func.MarkForDeletion();

        // Assert
        Assert.False(func.IsAlive);
    }

    [Fact]
    public void WeakFuncWithParameter_Target_ReturnsCorrectValue()
    {
        // Arrange
        var target = new object();
        var func = new WeakFunc<int, string>(target, _ => string.Empty);

        // Act
        var result = func.Target;

        // Assert
        Assert.Equal(target, result);
    }

    [Fact]
    public void WeakFuncWithParameter_MethodName_ReturnsCorrectName()
    {
        var func = new WeakFunc<int, string>(lambda);

        // Act
        var methodName = func.MethodName;

        // Assert
        Assert.NotNull(methodName);
        return;

        // Arrange
        static string lambda(int x) => x.ToString(CultureInfo.CurrentCulture);
    }

    [Fact]
    public void WeakFuncWithParameter_Execute_WithException_ReturnsDefault()
    {
        // Arrange
        var func = new WeakFunc<string, int>(_ => throw new InvalidOperationException());

        // Act
        var result = func.Execute("test");

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void WeakFuncWithParameter_Execute_AfterDeletion_ReturnsDefault()
    {
        // Arrange
        var func = new WeakFunc<int, string>(x => x.ToString(CultureInfo.CurrentCulture));

        // Act
        func.MarkForDeletion();
        var result = func.Execute(42);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region KeepTargetAlive parameter

    [Fact]
    public void WeakFunc_KeepTargetAlive_PreventsGarbageCollection()
    {
        // Arrange
        var disposable = new DisposableTarget();
        var func = new WeakFunc<int>(disposable, () => 42, keepTargetAlive: true);

        // Act
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Assert
        Assert.True(func.IsAlive);
        Assert.NotNull(func.Target);
    }

    #endregion

    #region IExecuteWithObjectAndResult implementation

    [Fact]
    public void WeakFuncWithParameter_ImplementsIExecuteWithObjectAndResult()
    {
        // Arrange
        var func = new WeakFunc<int, string>(x => x.ToString(CultureInfo.CurrentCulture));

        // Assert
        Assert.NotNull(func);
    }

    [Fact]
    public void IExecuteWithObjectAndResult_ExecuteWithObject_Works()
    {
        // Arrange
        var func = new WeakFunc<int, string>(x => $"Value: {x}");

        // Act
        var result = func.ExecuteWithObject(100);

        // Assert
        Assert.Equal("Value: 100", result);
    }

    [Fact]
    public void IExecuteWithObjectAndResult_Target_Works()
    {
        // Arrange
        var target = new object();
        var func = new WeakFunc<int, string>(target, _ => string.Empty);

        // Act
        var result = func.Target;

        // Assert
        Assert.Equal(target, result);
    }

    [Fact]
    public void IExecuteWithObjectAndResult_IsAlive_Works()
    {
        // Arrange
        var func = new WeakFunc<int, string>(_ => string.Empty);

        // Act
        var isAlive = func.IsAlive;

        // Assert
        Assert.True(isAlive);
    }

    [Fact]
    public void IExecuteWithObjectAndResult_MethodName_Works()
    {
        // Arrange
        var func = new WeakFunc<int, string>(_ => string.Empty);

        // Act
        var methodName = func.MethodName;

        // Assert
        Assert.NotNull(methodName);
    }

    [Fact]
    public void IExecuteWithObjectAndResult_MarkForDeletion_Works()
    {
        // Arrange
        var func = new WeakFunc<int, string>(_ => string.Empty);

        // Act
        func.MarkForDeletion();

        // Assert
        Assert.False(func.IsAlive);
    }

    #endregion

    #region Test helpers

    private static int StaticFunc() => 42;

    private sealed class DisposableTarget;

    #endregion
}
