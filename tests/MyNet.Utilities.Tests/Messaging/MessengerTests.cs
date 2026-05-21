// -----------------------------------------------------------------------
// <copyright file="MessengerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MyNet.Utilities.Messaging;
using Xunit;

namespace MyNet.Utilities.Tests.Messaging;

[SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Messenger is disposed by calling Reset in Dispose method")]
public sealed class MessengerTests : IDisposable
{
    private readonly Messenger _messenger = new();

    public void Dispose() => Messenger.Reset();

    #region Register tests

    [Fact]
    public void Register_AndSend_ExecutesAction()
    {
        // Arrange
        var executed = false;

        // Act
        _messenger.Register<TestMessage>(this, _ => executed = true);
        _messenger.Send(new TestMessage());

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void Register_MultipleRecipients_AllExecute()
    {
        // Arrange
        var count = 0;

        // Act
        for (var i = 0; i < 5; i++)
        {
            _messenger.Register<TestMessage>(new(), _ => count++);
        }

        _messenger.Send(new TestMessage());

        // Assert
        Assert.Equal(5, count);
    }

    [Fact]
    public void Register_WithToken_OnlyMatchingTokenReceives()
    {
        // Arrange
        var token1 = new object();
        var token2 = new object();
        var executed1 = false;
        var executed2 = false;

        // Act
        _messenger.Register<TestMessage>(this, token1, _ => executed1 = true);
        _messenger.Register<TestMessage>(this, token2, _ => executed2 = true);
        _messenger.Send(new TestMessage(), token1);

        // Assert
        Assert.True(executed1);
        Assert.False(executed2);
    }

    [Fact]
    public void Register_DerivedMessageType_ReceivesIfIncludeDerived()
    {
        // Arrange
        var executedBase = false;
        var executedDerived = false;

        // Act
        _messenger.Register<TestMessageBase>(this, true, _ => executedBase = true);
        _messenger.Register<TestMessageDerived>(this, false, _ => executedDerived = true);
        _messenger.Send(new TestMessageDerived());

        // Assert
        Assert.True(executedBase); // Derived sent to base because includeDerived=true
        Assert.True(executedDerived);
    }

    [Fact]
    public void Register_WithoutDerivedFlag_DoesNotReceiveDerived()
    {
        // Arrange
        var executed = false;

        // Act
        _messenger.Register<TestMessageBase>(this, false, _ => executed = true);
        _messenger.Send(new TestMessageDerived());

        // Assert
        Assert.False(executed);
    }

    #endregion

    #region Send tests

    [Fact]
    public void Send_WithoutParameter_CreatesDefault()
    {
        // Arrange
        TestMessage? receivedMessage = null;

        // Act
        _messenger.Register<TestMessage>(this, msg => receivedMessage = msg);
        _messenger.Send<TestMessage>();

        // Assert
        Assert.NotNull(receivedMessage);
    }

    [Fact]
    public void Send_WithTargetType_OnlyTargetReceives()
    {
        // Arrange
        var targetExecuted = false;
        var otherExecuted = false;
        var target = new TargetClass();
        var other = new object();

        // Act
        _messenger.Register<TestMessage>(target, _ => targetExecuted = true);
        _messenger.Register<TestMessage>(other, _ => otherExecuted = true);
        _messenger.Send<TestMessage, TargetClass>(new());

        // Assert
        Assert.True(targetExecuted);
        Assert.False(otherExecuted);
    }

    [Fact]
    public void Send_MessageContent_TransmittedCorrectly()
    {
        // Arrange
        var message = new TestMessage { Content = "Test" };
        TestMessage? received = null;

        // Act
        _messenger.Register<TestMessage>(this, msg => received = msg);
        _messenger.Send(message);

        // Assert
        Assert.NotNull(received);
        Assert.Equal("Test", received.Content);
    }

    #endregion

    #region Unregister tests

    [Fact]
    public void Unregister_StopsReceivingMessages()
    {
        // Arrange
        var executed = false;

        // Act
        _messenger.Register<TestMessage>(this, _ => executed = true);
        _messenger.Unregister(this);
        _messenger.Send(new TestMessage());

        // Assert
        Assert.False(executed);
    }

    [Fact]
    public void Unregister_SpecificMessage_StopsReceivingThatType()
    {
        // Arrange
        var executedMessage = false;
        var executedOther = false;

        // Act
        _messenger.Register<TestMessage>(this, _ => executedMessage = true);
        _messenger.Register<OtherMessage>(this, _ => executedOther = true);
        _messenger.Unregister<TestMessage>(this);
        _messenger.Send(new TestMessage());
        _messenger.Send(new OtherMessage());

        // Assert
        Assert.False(executedMessage);
        Assert.True(executedOther);
    }

    [Fact]
    public void Unregister_WithToken_OnlyRemovesMatching()
    {
        // Arrange
        var token1 = new object();
        var token2 = new object();
        var executed1 = false;
        var executed2 = false;

        // Act
        _messenger.Register<TestMessage>(this, token1, _ => executed1 = true);
        _messenger.Register<TestMessage>(this, token2, _ => executed2 = true);
        _messenger.Unregister<TestMessage>(this, token1);
        _messenger.Send(new TestMessage(), token1);
        _messenger.Send(new TestMessage(), token2);

        // Assert
        Assert.False(executed1);
        Assert.True(executed2);
    }

    [Fact]
    public void Unregister_WithAction_OnlyRemovesThatAction()
    {
        // Arrange
        var action1Executed = false;
        var action2Executed = false;

        // Act
        _messenger.Register(this, (Action<TestMessage>)action1);
        _messenger.Register(this, (Action<TestMessage>)action2);
        _messenger.Unregister(this, (Action<TestMessage>)action1);
        _messenger.Send(new TestMessage());

        // Assert
        Assert.False(action1Executed);
        Assert.True(action2Executed);
        return;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Test method parameters")]
        void action2(TestMessage _) => action2Executed = true;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Test method parameters")]
        void action1(TestMessage _) => action1Executed = true;
    }

    #endregion

    #region WeakReference tests

    [Fact]
    public void WeakReference_AllowsGarbageCollection()
    {
        // Arrange
        var weakAction = new WeakAction<TestMessage>(_ => { });

        // Act
        var initialAlive = weakAction.IsAlive;
        weakAction.MarkForDeletion();
        var afterMarkAlive = weakAction.IsAlive;

        // Assert
        Assert.True(initialAlive);
        Assert.False(afterMarkAlive);
    }

    [Fact]
    public void WeakActionGeneric_Targets_ReturnsCorrectTarget()
    {
        // Arrange
        var expected = new object();

        // Act
        var weakAction = new WeakAction<TestMessage>(expected, _ => { });
        var received = weakAction.Target;

        // Assert
        Assert.Equal(expected, received);
    }

    [Fact]
    public void Cleanup_RemovesDeadWeakActions()
    {
        // Arrange
        _messenger.Register<TestMessage>(new(), _ => { });

        // Act
        GC.Collect(); // Force garbage collection
        GC.WaitForPendingFinalizers();
        _messenger.Cleanup();

        // Assert
        // Should not throw and dead actions should be cleaned
        _messenger.Send(new TestMessage());
    }

    #endregion

    #region Exception handling tests

    [Fact]
    public void Execute_WithException_DoesNotCrash()
    {
        // Arrange
        var secondExecuted = false;

        // Act
        _messenger.Register<TestMessage>(this, _ => throw new InvalidOperationException("Test"));
        _messenger.Register<TestMessage>(new(), _ => secondExecuted = true);

        // This should not throw
        _messenger.Send(new TestMessage());

        // Assert
        Assert.True(secondExecuted); // Other handlers should still execute
    }

    #endregion

    #region Current messenger tests

    [Fact]
    public void OverrideDefault_UsesCustomInstance()
    {
        // Arrange
        var custom = new Messenger();
        var executed = false;

        // Act
        Messenger.OverrideDefault(custom);
        Messenger.Default?.Register<TestMessage>(this, _ => executed = true);
        Messenger.Default?.Send(new TestMessage());

        // Assert
        Assert.True(executed);
    }

    #endregion

    #region Test messages and classes

    private sealed class TestMessage
    {
        public string? Content { get; init; }
    }

    private class TestMessageBase;

    private sealed class TestMessageDerived : TestMessageBase;

    private sealed class OtherMessage;

    private sealed class TargetClass;

    #endregion
}
