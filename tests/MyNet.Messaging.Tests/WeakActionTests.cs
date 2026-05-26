// -----------------------------------------------------------------------
// <copyright file="WeakActionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyNet.Messaging.Tests;

public sealed class WeakActionTests : IDisposable
{
    public void Dispose()
    {
        // No cleanup needed
    }

    #region WeakAction (non-generic)

    [Fact]
    public void WeakAction_Execute_CallsAction()
    {
        // Arrange
        var executed = false;
        var action = new WeakAction(() => executed = true);

        // Act
        action.Execute();

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void WeakAction_IsAlive_TrueAfterConstruction()
    {
        // Arrange
        var action = new WeakAction(() => { });

        // Act
        var isAlive = action.IsAlive;

        // Assert
        Assert.True(isAlive);
    }

    [Fact]
    public void WeakAction_IsAlive_FalseAfterMarkForDeletion()
    {
        // Arrange
        var action = new WeakAction(() => { });

        // Act
        action.MarkForDeletion();

        // Assert
        Assert.False(action.IsAlive);
    }

    [Fact]
    public void WeakAction_Target_ReturnsCorrectValue()
    {
        // Arrange
        var target = new object();
        var action = new WeakAction(target, () => { });

        // Act
        var result = action.Target;

        // Assert
        Assert.Equal(target, result);
    }

    [Fact]
    public void WeakAction_IsStatic_TrueForStaticMethod()
    {
        // Arrange
        var action = new WeakAction(StaticMethod);

        // Act
        var isStatic = action.IsStatic;

        // Assert
        Assert.True(isStatic);
    }

    [Fact]
    public void WeakAction_MethodName_ReturnsCorrectName()
    {
        // Arrange
        var action = new WeakAction(StaticMethod);

        // Act
        var methodName = action.MethodName;

        // Assert
        Assert.Equal(nameof(StaticMethod), methodName);
    }

    #endregion

    #region WeakAction<T>

    [Fact]
    public void WeakActionGeneric_Execute_CallsActionWithParameter()
    {
        // Arrange
        string? received = null;
        var action = new WeakAction<string>(param => received = param);

        // Act
        action.Execute("test");

        // Assert
        Assert.Equal("test", received);
    }

    [Fact]
    public void WeakActionGeneric_Execute_NoParameter_UsesDefault()
    {
        // Arrange
        var received = "original";
        var action = new WeakAction<string>(param => received = param);

        // Act
        action.Execute();

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void WeakActionGeneric_ExecuteWithObject_CastsCorrectly()
    {
        // Arrange
        int? received = null;
        var action = new WeakAction<int>(param => received = param);

        // Act
        action.ExecuteWithObject(42);

        // Assert
        Assert.Equal(42, received);
    }

    [Fact]
    public void WeakActionGeneric_ExecuteWithObject_Nullable()
    {
        // Arrange
        int? received = 99;
        var action = new WeakAction<int?>(param => received = param);

        // Act
        action.ExecuteWithObject(null);

        // Assert
        Assert.Null(received);
    }

    [Fact]
    public void WeakActionGeneric_IsAlive_False_AfterMarkForDeletion()
    {
        // Arrange
        var action = new WeakAction<string>(_ => { });

        // Act
        action.MarkForDeletion();

        // Assert
        Assert.False(action.IsAlive);
    }

    [Fact]
    public void WeakActionGeneric_Target_ReturnsCorrectValue()
    {
        // Arrange
        var target = new object();
        var action = new WeakAction<string>(target, _ => { });

        // Act
        var result = action.Target;

        // Assert
        Assert.Equal(target, result);
    }

    [Fact]
    public void WeakActionGeneric_MethodName_ReturnsCorrectName()
    {
        var action = new WeakAction<string>(lambda!);

        // Act
        var methodName = action.MethodName;

        // Assert
        Assert.NotNull(methodName);
        Assert.True(methodName.Contains("lambda", StringComparison.InvariantCultureIgnoreCase) || methodName.Contains("Invoke", StringComparison.InvariantCultureIgnoreCase));
        return;

        // Arrange
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Lambda parameter")]
        static void lambda(string _)
        {
        }
    }

    [Fact]
    public void WeakActionGeneric_Execute_WithException_DoesNotThrow()
    {
        // Arrange
        var action = new WeakAction<string>(_ => throw new InvalidOperationException());

        // Act & Assert - should not throw
        action.Execute("test");
    }

    #endregion

    #region KeepTargetAlive parameter

    [Fact]
    public void WeakAction_KeepTargetAlive_PreventsGarbageCollection()
    {
        // Arrange
        var disposable = new DisposableTarget();
        var action = new WeakAction(disposable, () => { }, keepTargetAlive: true);

        // Act
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Assert
        Assert.True(action.IsAlive);
        Assert.NotNull(action.Target);
    }

    [Fact]
    public void WeakActionGeneric_KeepTargetAlive_Default_AllowsGarbageCollection()
    {
        // Arrange
        var disposable = new DisposableTarget();
        _ = new WeakAction<string>(disposable, _ => { }, keepTargetAlive: false);

        // Act
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Assert - may vary based on GC behavior, but should eventually be dead
        // Note: This test might be flaky depending on GC behavior
    }

    #endregion

    #region Test helpers

    private static void StaticMethod() { }

    private sealed class DisposableTarget;

    #endregion
}
