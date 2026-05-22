// -----------------------------------------------------------------------
// <copyright file="ModificationTrackingBehaviorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using MyNet.Observable.Behaviors;
using MyNet.Utilities.Metadata;
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
        sut.RegisterBehavior(behavior);

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
        sut.RegisterBehavior(behavior);

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
                OnPropertyChanging(nameof(Tracked), before, value);
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
                OnPropertyChanging(nameof(Ignored), before, value);
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
                OnPropertyChanging(nameof(Child), before, value);
                field = value;
                OnPropertyChanged(nameof(Child), before, value);
            }
        }
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
