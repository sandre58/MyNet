// -----------------------------------------------------------------------
// <copyright file="MessengerExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MyNet.Utilities.Messaging;
using Xunit;

namespace MyNet.Utilities.Tests.Messaging;

[SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Messenger is disposed by calling Reset in Dispose method")]
public sealed class MessengerExtensionsTests : IDisposable
{
    private readonly Messenger _messenger = new();

    public void Dispose() => Messenger.Reset();

    #region RegisterOnce tests

    [Fact]
    public void RegisterOnce_ExecutesOnce()
    {
        // Arrange
        var count = 0;

        // Act
        _messenger.RegisterOnce<TestMessage>(this, _ => count++);
        _messenger.Send(new TestMessage());
        _messenger.Send(new TestMessage());

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void RegisterOnce_UnregistersAutomatically()
    {
        // Arrange
        var executed = false;

        // Act
        _messenger.RegisterOnce<TestMessage>(this, _ => executed = true);
        _messenger.Send(new TestMessage());
        executed = false;
        _messenger.Send(new TestMessage());

        // Assert
        Assert.False(executed);
    }

    #endregion

    #region RegisterForDerivedMessages tests

    [Fact]
    public void RegisterForDerivedMessages_ReceivesDerived()
    {
        // Arrange
        var executed = false;

        // Act
        _messenger.RegisterForDerivedMessages<TestMessageBase>(this, _ => executed = true);
        _messenger.Send(new TestMessageDerived());

        // Assert
        Assert.True(executed);
    }

    #endregion

    #region Channel tests

    [Fact]
    public void RegisterOnChannel_OnlySendsOnMatchingChannel()
    {
        // Arrange
        var channel1Executed = false;
        var channel2Executed = false;

        // Act
        _messenger.RegisterOnChannel<TestMessage>(this, "channel1", _ => channel1Executed = true);
        _messenger.RegisterOnChannel<TestMessage>(this, "channel2", _ => channel2Executed = true);
        _messenger.SendOnChannel(new TestMessage(), "channel1");

        // Assert
        Assert.True(channel1Executed);
        Assert.False(channel2Executed);
    }

    [Fact]
    public void SendOnChannel_CorrectChannelReceives()
    {
        // Arrange
        var executed = false;
        const string channel = "test-channel";

        // Act
        _messenger.RegisterOnChannel<TestMessage>(this, channel, _ => executed = true);
        _messenger.SendOnChannel(new TestMessage(), channel);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void UnregisterFromChannel_RemovesChannelRegistration()
    {
        // Arrange
        var executed = false;
        const string channel = "test-channel";

        // Act
        _messenger.RegisterOnChannel<TestMessage>(this, channel, _ => executed = true);
        _messenger.UnregisterFromChannel<TestMessage>(this, channel);
        _messenger.SendOnChannel(new TestMessage(), channel);

        // Assert
        Assert.False(executed);
    }

    #endregion

    #region SendTo tests

    [Fact]
    public void SendTo_OnlyTargetTypeReceives()
    {
        // Arrange
        var targetExecuted = false;
        var otherExecuted = false;

        // Act
        _messenger.Register<TestMessage>(new TargetClass(), _ => targetExecuted = true);
        _messenger.Register<TestMessage>(new(), _ => otherExecuted = true);
        _messenger.SendTo<TestMessage, TargetClass>(new());

        // Assert
        Assert.True(targetExecuted);
        Assert.False(otherExecuted);
    }

    #endregion

    #region UnregisterAll tests

    [Fact]
    public void UnregisterAll_RemovesAllRegistrations()
    {
        // Arrange
        var message1Executed = false;
        var message2Executed = false;

        // Act
        _messenger.Register<TestMessage>(this, _ => message1Executed = true);
        _messenger.Register<OtherMessage>(this, _ => message2Executed = true);
        _messenger.UnregisterAll(this);
        _messenger.Send(new TestMessage());
        _messenger.Send(new OtherMessage());

        // Assert
        Assert.False(message1Executed);
        Assert.False(message2Executed);
    }

    #endregion

    #region RegisterWithFilter tests

    [Fact]
    public void RegisterWithFilter_OnlyExecutesWhenPredicateMatches()
    {
        // Arrange
        var count = 0;

        // Act
        _messenger.RegisterWithFilter<TestMessage>(
            this,
            msg => msg.Content == "match",
            _ => count++);

        _messenger.Send(new TestMessage { Content = "no-match" });
        _messenger.Send(new TestMessage { Content = "match" });
        _messenger.Send(new TestMessage { Content = "no-match" });

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void RegisterWithFilter_PredicateFalse_DoesNotExecute()
    {
        // Arrange
        var executed = false;

        // Act
        _messenger.RegisterWithFilter<TestMessage>(
            this,
            _ => false,
            _ => executed = true);

        _messenger.Send(new TestMessage());

        // Assert
        Assert.False(executed);
    }

    #endregion

    #region SendFromFactory tests

    [Fact]
    public void SendFromFactory_CreatesAndSends()
    {
        // Arrange
        TestMessage? received = null;

        // Act
        _messenger.Register<TestMessage>(this, msg => received = msg);
        _messenger.SendFromFactory(() => new TestMessage { Content = "factory" });

        // Assert
        Assert.NotNull(received);
        Assert.Equal("factory", received.Content);
    }

    #endregion

    #region Test messages

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
