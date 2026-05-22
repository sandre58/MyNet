// -----------------------------------------------------------------------
// <copyright file="SelectionAndWrapperBehaviorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using MyNet.Observable.Behaviors;
using MyNet.Observable.Behaviors.Metadata;
using MyNet.Utilities.Metadata;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class SelectionAndWrapperBehaviorTests
{
    [Fact]
    public void SelectionBehavior_NotifiesOwnerOnSelectionChanges()
    {
        var owner = new DummyOwner();
        owner.Behaviors.Register(new SelectionBehavior(owner));

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        var sel = owner.Behaviors.Get<SelectionBehavior>();

        sel.IsSelected = true;

        Assert.Contains("IsSelected", changed);

        sel.IsSelectable = false; // should clear selection and notify both IsSelected and IsSelectable

        Assert.Contains("IsSelectable", changed);
        Assert.Contains("IsSelected", changed);
    }

    [Fact]
    public void WrapperRelayBehavior_RelaysChildPropertyChanged_ToOwner()
    {
        var owner = new OwnerWithWrapper();
        owner.ForwardProperty(x => x.Wrapper);

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        // Initial relay from current wrapper instance
        owner.Wrapper.Name = "a";
        Assert.Contains($"{nameof(OwnerWithWrapper.Wrapper)}.{nameof(Child.Name)}", changed);

        // Change wrapper instance: ensure detach/attach works
        changed.Clear();
        var nextWrapper = new Child();
        owner.Wrapper = nextWrapper;
        nextWrapper.Name = "b";

        Assert.Contains($"{nameof(OwnerWithWrapper.Wrapper)}.{nameof(Child.Name)}", changed);
    }

    [Fact]
    public void SelectionBehavior_Suspend_PreventsNotifications()
    {
        var owner = new DummyOwner();
        owner.Behaviors.Register(new SelectionBehavior(owner));

        var sel = owner.Behaviors.Get<SelectionBehavior>();

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        using (sel.Suspend())
        {
            sel.IsSelected = true;
            Assert.False(sel.IsSelected);
            Assert.Empty(changed);
        }
    }

    [Fact]
    public void WrapperRelayBehavior_Dispose_StopsRelaying()
    {
        var owner = new OwnerWithWrapper();
        owner.ForwardProperty(x => x.Wrapper);
        var behavior = owner.Behaviors.Get<PropertyChangedForwardingBehavior>(nameof(OwnerWithWrapper.Wrapper), nameof(PropertyChangedForwardingBehavior));

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        // initial relay works
        owner.Wrapper.Name = "a";
        Assert.Contains($"{nameof(OwnerWithWrapper.Wrapper)}.{nameof(Child.Name)}", changed);

        changed.Clear();

        // dispose behavior => no more relaying
        behavior.Dispose();
        owner.Wrapper.Name = "b";
        Assert.DoesNotContain($"{nameof(OwnerWithWrapper.Wrapper)}.{nameof(Child.Name)}", changed);
    }

    [Fact]
    public void UseWrapperRelay_ExpressionOverload_RegistersBehavior()
    {
        var owner = new OwnerWithWrapper();
        owner.ForwardProperty(x => x.Wrapper);

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        owner.Wrapper.Name = "z";

        Assert.Contains($"{nameof(OwnerWithWrapper.Wrapper)}.{nameof(Child.Name)}", changed);
    }

    [Fact]
    public void WrapperRelayBehavior_CanRelayWithoutPropertyPrefix()
    {
        var owner = new OwnerWithWrapper();
        owner.ForwardProperty(x => x.Wrapper, concatenatePropertyName: false);

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        owner.Wrapper.Name = "n";

        Assert.Contains(nameof(Child.Name), changed);
        Assert.DoesNotContain($"{nameof(OwnerWithWrapper.Wrapper)}.{nameof(Child.Name)}", changed);
    }

    [Fact]
    public void GeneratedRegistry_RegistersPropertyBehavior_WithUniquePropertyKey()
    {
        MetadataApplicators.ApplyForwardProperty(
            MetadataRegistry.Get(typeof(OwnerWithGeneratedBehavior)).GetProperty(nameof(OwnerWithGeneratedBehavior.Wrapper)));
        var owner = new OwnerWithGeneratedBehavior();

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        owner.Wrapper.Name = "g";

        Assert.Contains($"{nameof(OwnerWithGeneratedBehavior.Wrapper)}.{nameof(Child.Name)}", changed);
        Assert.True(owner.Behaviors.Has<PropertyChangedForwardingBehavior>(
            nameof(OwnerWithGeneratedBehavior.Wrapper),
            nameof(PropertyChangedForwardingBehavior)));
    }

    [Fact]
    public void GeneratedRegistry_CanDisablePropertyNameConcatenation()
    {
        MetadataApplicators.ApplyForwardProperty(
            MetadataRegistry.Get(typeof(OwnerWithGeneratedBehaviorWithoutPrefix)).GetProperty(nameof(OwnerWithGeneratedBehaviorWithoutPrefix.Wrapper)),
            concatenatePropertyName: false);
        var owner = new OwnerWithGeneratedBehaviorWithoutPrefix();

        var changed = new List<string>();
        owner.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        owner.Wrapper.Name = "g";

        Assert.Contains(nameof(Child.Name), changed);
        Assert.DoesNotContain($"{nameof(OwnerWithGeneratedBehaviorWithoutPrefix.Wrapper)}.{nameof(Child.Name)}", changed);
    }

    private sealed class DummyOwner : ObservableObject;

    private sealed class Child : INotifyPropertyChanged
    {
        public string Name
        {
            get;
            set
            {
                if (field == value)
                    return;
                field = value;
                PropertyChanged?.Invoke(this, new(nameof(Name)));
            }
        }

            = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private sealed class OwnerWithWrapper : ObservableObject
    {
        public Child Wrapper
        {
            get;
            set
            {
                var before = field;
                OnPropertyChanging(nameof(Wrapper), before, value);
                field = value;
                OnPropertyChanged(nameof(Wrapper), before, value);
            }
        }

            = new();
    }

    private sealed class OwnerWithGeneratedBehavior : ObservableObject
    {
        public Child Wrapper
        {
            get;
            init
            {
                var before = field;
                OnPropertyChanging(nameof(Wrapper), before, value);
                field = value;
                OnPropertyChanged(nameof(Wrapper), before, value);
            }
        }

            = new();
    }

    private sealed class OwnerWithGeneratedBehaviorWithoutPrefix : ObservableObject
    {
        public Child Wrapper
        {
            get;
            init
            {
                var before = field;
                OnPropertyChanging(nameof(Wrapper), before, value);
                field = value;
                OnPropertyChanged(nameof(Wrapper), before, value);
            }
        }

            = new();
    }
}
