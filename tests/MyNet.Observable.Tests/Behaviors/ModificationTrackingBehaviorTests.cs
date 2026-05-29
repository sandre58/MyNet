// -----------------------------------------------------------------------
// <copyright file="ModificationTrackingBehaviorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using MyNet.Metadata;
using MyNet.Observable.Behaviors;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class ModificationTrackingBehaviorTests
{
    [Fact]
    public void IgnoresConfiguredProperty_AndTracksRegularProperty()
    {
        MetadataRegistry.For<TrackableOwner>()
            .Property(x => x.Ignored)
            .IgnoreModificationTracking();

        var sut = new TrackableOwner();
        var behavior = new ModificationTrackingBehavior(sut);
        sut.Behaviors.Register(behavior);

        sut.Ignored = 1;

        Assert.False(behavior.IsModified);

        sut.Tracked = 1;

        Assert.True(behavior.IsModified);

        behavior.ResetModified();

        Assert.False(behavior.IsModified);
    }

    [Fact]
    public void ChildModification_PropagatesToOwner()
    {
        MetadataRegistry.For<TrackableOwner>()
            .Property(x => x.Ignored)
            .IgnoreModificationTracking();

        var sut = new TrackableOwner();
        var behavior = new ModificationTrackingBehavior(sut);
        sut.Behaviors.Register(behavior);

        var child = new TrackableChild();
        sut.Child = child;
        behavior.ResetModified();

        child.MarkModified();

        Assert.True(behavior.IsModified);
    }

    [Fact]
    public void ObservableObject_CanImplement_IModificationAware()
    {
        var sut = new ObservableModificationAware();

        // Exercise the extension that checks for IModificationAware on an ObservableObject
        Assert.False(sut.IsModified());
    }

    [Fact]
    public void Constructor_WhenGetterDependsOnDerivedField_DoesNotThrow()
    {
        var exception = Record.Exception(static () => _ = new DeferredChildOwner());

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureInitialStateAttached_WhenGetterDependsOnDerivedField_PropagatesChildModification()
    {
        var sut = new DeferredChildOwner();
        var behavior = sut.Behaviors.Get<ModificationTrackingBehavior>();

        Assert.False(behavior.IsModified);

        sut.Child!.MarkModified();

        Assert.True(behavior.IsModified);
    }

    private sealed class ObservableModificationAware : ObservableObject, IModificationAware
    {
        public bool IsModified { get; private set; }

        public void ResetModified() => IsModified = false;
    }

    private sealed class TrackableOwner : ObservableObject
    {
        public int Tracked
        {
            get;
            set
            {
                var before = field;
                if (!OnPropertyChanging(nameof(Tracked), before, value))
                    return;
                field = value;
                OnPropertyChanged(nameof(Tracked), before, value);
            }
        }

        public int Ignored
        {
            get;
            set
            {
                var before = field;
                if (!OnPropertyChanging(nameof(Ignored), before, value))
                    return;
                field = value;
                OnPropertyChanged(nameof(Ignored), before, value);
            }
        }

        public TrackableChild? Child
        {
            get;
            set
            {
                var before = field;
                if (!OnPropertyChanging(nameof(Child), before, value))
                    return;
                field = value;
                OnPropertyChanged(nameof(Child), before, value);
            }
        }
    }

    private sealed class DeferredChildOwner : ObservableObject
    {
        private readonly ChildHost _host;

        public DeferredChildOwner()
        {
            Behaviors.Register(new ModificationTrackingBehavior(this));
            _host = new ChildHost();
        }

        public TrackableChild? Child => _host.Child;
    }

    private sealed class ChildHost
    {
        public TrackableChild Child { get; } = new();
    }

    private sealed class TrackableChild : IModificationAware, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsModified { get; private set; }

        public void MarkModified()
        {
            IsModified = true;
            PropertyChanged?.Invoke(this, new(nameof(IsModified)));
        }

        public void ResetModified() => IsModified = false;
    }
}
