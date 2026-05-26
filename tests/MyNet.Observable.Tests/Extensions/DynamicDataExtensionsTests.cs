// -----------------------------------------------------------------------
// <copyright file="DynamicDataExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Threading;
using DynamicData;
using Xunit;

namespace MyNet.Observable.Tests.Extensions;

public sealed class DynamicDataExtensionsTests
{
    [Fact]
    public void ObserveOnOptional_WhenSchedulerIsNull_ReturnsSameInstance()
    {
        var source = System.Reactive.Linq.Observable.Return(1);

        var result = source.ObserveOnOptional(scheduler: null);

        Assert.True(ReferenceEquals(source, result));
    }

    [Fact]
    public void ObserveOnOptional_WhenSchedulerIsProvided_EmitsValues()
    {
        var source = System.Reactive.Linq.Observable.Return(1);
        var received = new List<int>();

        using var sub = source
            .ObserveOnOptional(ImmediateScheduler.Instance)
            .Subscribe(received.Add);

        Assert.Equal([1], received);
    }

    [Fact]
    public void SubscribeAll_CallsActionOnOuterChanges_AndOnItemPropertyChanges()
    {
        using var source = new SourceList<NotifyItem>();

        var calls = 0;
        using var sub = source.Connect()
            .SubscribeAll(() => calls++);

        Assert.Equal(0, calls);

        var item = new NotifyItem(1);
        source.Add(item);

        Assert.True(SpinWait.SpinUntil(() => calls >= 1, TimeSpan.FromSeconds(2)), $"calls={calls}");

        item.Value = 2;
        Assert.True(SpinWait.SpinUntil(() => calls >= 2, TimeSpan.FromSeconds(2)), $"calls={calls}");
    }

    [Fact]
    public void RemoveMany_RemovesAllMatchingInstances_WhenItemsToRemoveContainDuplicates()
    {
        var list = new List<int>
        {
            1,
            1,
            2,
            3
        };

        list.RemoveMany([1, 1]);

        Assert.Equal([2, 3], list);
    }

    [Fact]
    public void RemoveMany_WhenItemsToRemoveIsNull_Throws()
    {
        ICollection<int> list = [1];
        IEnumerable<int>? itemsToRemove = null;

        Assert.Throws<ArgumentNullException>(() => act(itemsToRemove!));

        void act(IEnumerable<int> items) => list.RemoveMany(items);
    }

    private sealed class NotifyItem(int value) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Value
        {
            get;
            set
            {
                if (field == value)
                    return;

                field = value;
                PropertyChanged?.Invoke(this, new(nameof(Value)));
            }
        }

            = value;
    }
}
