// -----------------------------------------------------------------------
// <copyright file="ObservableObjectCoreTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using MyNet.Observable.Behaviors;
using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class ObservableObjectCoreTests
{
    [Fact]
    public void SetProperty_RaisesChangingThenChanged()
    {
        var sut = new TestObservable();
        var events = new List<string>();

        sut.PropertyChanging += (_, e) => events.Add("changing:" + e.PropertyName);
        sut.PropertyChanged += (_, e) => events.Add("changed:" + e.PropertyName);

        sut.Value = 42;

        Assert.Equal(["changing:Value", "changed:Value"], events);
    }

    [Fact]
    public void SuspendNotifications_SkipsEventsInsideScope()
    {
        var sut = new TestObservable();
        var changedCount = 0;

        sut.PropertyChanged += (_, _) => changedCount++;

        using (sut.SuspendPublicNotifications())
        {
            sut.Value = 10;
        }

        Assert.Equal(0, changedCount);

        sut.Value = 11;

        Assert.Equal(1, changedCount);
    }

    [Fact]
    public void RegisterBehavior_ExposesBehaviorAndInvokesPipeline()
    {
        var sut = new TestObservable();
        var behavior = new SpyChangedBehavior();

        sut.Behaviors.Register(behavior);

        Assert.True(sut.Behaviors.Has<SpyChangedBehavior>());
        Assert.Same(behavior, sut.Behaviors.Get<SpyChangedBehavior>());

        sut.Value = 7;

        Assert.Equal(1, behavior.OnChangedCalls);
    }

    [Fact]
    public void ObservableObject_Disposables_IsUpdated()
    {
        var sut = new ObservableWithDisposables();

        // Dispose should succeed and dispose the added disposable.
        sut.Dispose();

        Assert.True(sut.IsDisposed);
    }

    private sealed class TestObservable : ObservableObject
    {
        public int Value
        {
            get;
            set
            {
                var before = field;
                OnPropertyChanging(nameof(Value), before, value);
                field = value;
                OnPropertyChanged(nameof(Value), before, value);
            }
        }

        public IDisposable SuspendPublicNotifications() => SuspendNotifications();
    }

    private sealed class SpyChangedBehavior : IPropertyChangedBehavior
    {
        public int OnChangedCalls { get; private set; }

        public void OnPropertyChanged(PropertyMutationContext context) => OnChangedCalls++;
    }

    private sealed class ObservableWithDisposables : ObservableObject
    {
        public ObservableWithDisposables() => Disposables.Add(Disposable.Create(() => { }));
    }
}
