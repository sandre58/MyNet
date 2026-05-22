// -----------------------------------------------------------------------
// <copyright file="ObservableObjectBehaviorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable.Behaviors;
using Xunit;

namespace MyNet.Observable.Metadata.Generator.Tests;

public sealed class ObservableObjectBehaviorTests
{
    private sealed class TestVm : ObservableObject
    {
        public string Title
        {
            get;
            set
            {
                var before = field;
                OnPropertyChanging(nameof(Title), before, value);
                field = value;
                OnPropertyChanged(nameof(Title), before, value);
            }
        }

            = string.Empty;
    }

    private sealed class TestBehavior : IPropertyChangingBehavior, IPropertyChangedBehavior, IDisposable
    {
        public bool Disposed { get; private set; }

        public int ChangingCalled { get; private set; }

        public int ChangedCalled { get; private set; }

        public PropertyMutationContext? LastContext { get; private set; }

        public void OnPropertyChanging(PropertyMutationContext context)
        {
            ChangingCalled++;
            LastContext = context;
        }

        public void OnPropertyChanged(PropertyMutationContext context)
        {
            ChangedCalled++;
            LastContext = context;
        }

        public void IncrementChanged() => ChangedCalled++;

        public void Dispose() => Disposed = true;
    }

    [Fact]
    public void RegisterBehavior_And_Retrieve_Api_Works()
    {
        var vm = new TestVm();

        var b = new TestBehavior();

        vm.Behaviors.Register(b);

        Assert.True(vm.Behaviors.Has<TestBehavior>());
        Assert.True(vm.Behaviors.TryGet(out TestBehavior? got));
        Assert.Same(b, got);
        Assert.Same(b, vm.Behaviors.Get<TestBehavior>());
        Assert.Same(b, vm.Behaviors.GetOrDefault<TestBehavior>());
    }

    [Fact]
    public void TryExecute_And_Execute_Invoke_Action_On_Behavior()
    {
        var vm = new TestVm();
        var b = new TestBehavior();
        vm.Behaviors.Register(b);

        var executed = false;

        var tryResult = vm.Behaviors.TryExecute<TestBehavior>(x => executed = x == b);
        Assert.True(tryResult);
        Assert.True(executed);

        // Execute should not throw and should invoke action
        vm.Behaviors.Execute<TestBehavior>(x => x.IncrementChanged());
        Assert.Equal(1, b.ChangedCalled);
    }

    [Fact]
    public void TryEvaluate_And_Evaluate_Returns_Expected()
    {
        var vm = new TestVm();
        var b = new TestBehavior();
        vm.Behaviors.Register(b);

        var ok = vm.Behaviors.TryEvaluate<TestBehavior, string>(_ => "ok", out var result);
        Assert.True(ok);
        Assert.Equal("ok", result);

        var eval = vm.Behaviors.Evaluate<TestBehavior, string>(_ => "val", "def");
        Assert.Equal("val", eval);
    }

    [Fact]
    public void Behavior_Is_Disposed_When_Vm_Disposed()
    {
        var vm = new TestVm();
        var b = new TestBehavior();
        vm.Behaviors.Register(b);

        vm.Dispose();

        Assert.True(b.Disposed);
        Assert.True(vm.IsDisposed);
    }

    [Fact]
    public void Behavior_Methods_Are_Called_On_Property_Change()
    {
        var vm = new TestVm();
        var b = new TestBehavior();
        vm.Behaviors.Register(b);

        var changedRaised = false;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TestVm.Title)) changedRaised = true;
        };

        vm.Title = "Hello";

        Assert.Equal(1, b.ChangingCalled);
        Assert.Equal(1, b.ChangedCalled);
        Assert.True(changedRaised);
        Assert.Equal(nameof(TestVm.Title), b.LastContext?.PropertyName);
        Assert.Equal(string.Empty, b.LastContext?.OldValue as string);
    }
}
