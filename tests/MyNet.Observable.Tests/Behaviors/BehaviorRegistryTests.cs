// -----------------------------------------------------------------------
// <copyright file="BehaviorRegistryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable.Behaviors;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class BehaviorRegistryTests
{
    [Fact]
    public void Register_SameKey_ReplacesWithoutDuplicatingPipeline()
    {
        var owner = new PipelineOwner();
        var first = new CountingChangedBehavior();
        var second = new CountingChangedBehavior();

        owner.Behaviors.Register(first);
        owner.Behaviors.Register(second);

        Assert.True(first.Disposed);
        owner.RaiseValue();

        Assert.Equal(1, second.ChangedCalls);
    }

    [Fact]
    public void TryGet_WithScopeAndPropertyName_ReturnsExactInstance()
    {
        var owner = new PipelineOwner();
        var wrapperA = new CountingChangedBehavior();
        var wrapperB = new CountingChangedBehavior();

        owner.Behaviors.Register(wrapperA, "WrapperA", nameof(CountingChangedBehavior));
        owner.Behaviors.Register(wrapperB, "WrapperB", nameof(CountingChangedBehavior));

        Assert.False(owner.Behaviors.TryGet<CountingChangedBehavior>(out _));

        Assert.True(owner.Behaviors.TryGet(out CountingChangedBehavior? gotA, "WrapperA", nameof(CountingChangedBehavior)));
        Assert.Same(wrapperA, gotA);

        Assert.True(owner.Behaviors.TryGet(out CountingChangedBehavior? gotB, "WrapperB", nameof(CountingChangedBehavior)));
        Assert.Same(wrapperB, gotB);
    }

    [Fact]
    public void GetAll_ReturnsAllMatchingScopedBehaviors()
    {
        var owner = new PipelineOwner();
        owner.Behaviors.Register(new CountingChangedBehavior(), "A", nameof(CountingChangedBehavior));
        owner.Behaviors.Register(new CountingChangedBehavior(), "B", nameof(CountingChangedBehavior));

        var all = owner.Behaviors.GetAll<CountingChangedBehavior>(scope: nameof(CountingChangedBehavior));

        Assert.Equal(2, all.Length);
    }

    [Fact]
    public void Unregister_RemovesFromPipeline()
    {
        var owner = new PipelineOwner();
        var behavior = new CountingChangedBehavior();
        owner.Behaviors.Register(behavior);

        Assert.True(owner.Behaviors.Unregister<CountingChangedBehavior>());

        owner.RaiseValue();

        Assert.Equal(0, behavior.ChangedCalls);
        Assert.True(behavior.Disposed);
    }

    [Fact]
    public void RegisterForwarding_ReplacesPreviousInstance()
    {
        const string wrapperProperty = "Wrapper";
        var owner = new PipelineOwner();
        owner.Behaviors.Register(new CountingChangedBehavior(), wrapperProperty, nameof(PropertyChangedForwardingBehavior));

        var first = owner.Behaviors.Get<CountingChangedBehavior>(wrapperProperty, nameof(PropertyChangedForwardingBehavior));
        owner.Behaviors.Register(new CountingChangedBehavior(), wrapperProperty, nameof(PropertyChangedForwardingBehavior));

        Assert.True(first.Disposed);
        Assert.NotSame(first, owner.Behaviors.Get<CountingChangedBehavior>(wrapperProperty, nameof(PropertyChangedForwardingBehavior)));
    }

    private sealed class PipelineOwner : ObservableObject
    {
        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                var before = _value;
                OnPropertyChanged(nameof(Value), before, value);
                _value = value;
            }
        }

        public void RaiseValue() => OnPropertyChanged(nameof(Value), _value, _value);
    }

    private sealed class CountingChangedBehavior : IPropertyChangedBehavior, IDisposable
    {
        public int ChangedCalls { get; private set; }

        public bool Disposed { get; private set; }

        public void OnPropertyChanged(PropertyMutationContext context) => ChangedCalls++;

        public void Dispose() => Disposed = true;
    }
}
