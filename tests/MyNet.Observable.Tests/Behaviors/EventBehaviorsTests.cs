// -----------------------------------------------------------------------
// <copyright file="EventBehaviorsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;
using MyNet.Observable.Behaviors;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Metadata;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class EventBehaviorsTests
{
    [Fact]
    public async Task EventBehavior_Invokes_IAsyncEventAware_OnEventAsync()
    {
        var owner = new AsyncEventAwareOwner();
        var behavior = new TestAsyncEventBehavior(owner);

        behavior.Raise(new() { Value = 123 });

        // The behavior triggers the async handler without awaiting it. The handler used here
        // completes synchronously, but give a small delay to avoid timing issues.
        await Task.Delay(20);

        Assert.Equal(1, owner.Calls);
        Assert.Equal(123, owner.Value);
    }

    [Fact]
    public void CultureAndTimeZoneBehaviors_NotifyConfiguredProperties_AndDispatchEventAwareCallbacks()
    {
        MetadataRegistry.For<EventAwareOwner>()
            .Property(x => x.CultureLabel)
            .UpdateOnCultureChanged();

        MetadataRegistry.For<EventAwareOwner>()
            .Property(x => x.TimeZoneLabel)
            .UpdateOnTimeZoneChanged();

        var cultureService = new FakeCultureService();
        var timeZoneService = new FakeTimeZoneService();
        var sut = new EventAwareOwner();

        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        sut.Behaviors.Register(new CultureChangedBehavior(sut, cultureService));
        sut.Behaviors.Register(new TimeZoneChangedBehavior(sut, timeZoneService));

        cultureService.Raise(CultureInfo.InvariantCulture, new("fr-FR"));
        timeZoneService.Raise(TimeZoneInfo.Utc, TimeZoneInfo.Local);

        Assert.Contains(nameof(EventAwareOwner.CultureLabel), changedProperties);
        Assert.Contains(nameof(EventAwareOwner.TimeZoneLabel), changedProperties);
        Assert.NotNull(sut.LastCultureEvent);
        Assert.NotNull(sut.LastTimeZoneEvent);
    }

    private sealed class EventAwareOwner : ObservableObject, IEventAware<CultureChangedEvent>, IEventAware<TimeZoneChangedEvent>
    {
        public string CultureLabel { get; private set; } = string.Empty;

        public string TimeZoneLabel { get; private set; } = string.Empty;

        public CultureChangedEvent? LastCultureEvent { get; private set; }

        public TimeZoneChangedEvent? LastTimeZoneEvent { get; private set; }

        void IEventAware<CultureChangedEvent>.OnEvent(CultureChangedEvent e)
        {
            CultureLabel = e.Culture.Name;
            LastCultureEvent = e;
        }

        void IEventAware<TimeZoneChangedEvent>.OnEvent(TimeZoneChangedEvent e)
        {
            TimeZoneLabel = e.TimeZone.DisplayName;
            LastTimeZoneEvent = e;
        }
    }

    private sealed class FakeCultureService : ICultureService
    {
        public event EventHandler<CultureChangedEventArgs>? CultureChanged;

        public CultureInfo CurrentCulture { get; private set; } = CultureInfo.InvariantCulture;

        public void SetCulture(CultureInfo culture) => Raise(CurrentCulture, culture);

        public void SetCulture(string cultureCode) => SetCulture(new CultureInfo(cultureCode));

        public void Raise(CultureInfo oldCulture, CultureInfo newCulture)
        {
            CurrentCulture = newCulture;
            CultureChanged?.Invoke(this, new(oldCulture, newCulture));
        }
    }

    private sealed class FakeTimeZoneService : ITimeZoneService
    {
        public event EventHandler<TimeZoneChangedEventArgs>? TimeZoneChanged;

        public DateTime Now => DateTime.Now;

        public TimeZoneInfo CurrentTimeZone { get; private set; } = TimeZoneInfo.Utc;

        public void SetTimeZone(TimeZoneInfo timeZone) => Raise(CurrentTimeZone, timeZone);

        public void SetTimeZone(string timeZoneId) => SetTimeZone(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));

        public DateTimeOffset FromUtc(DateTimeOffset utcDateTime) => TimeZoneInfo.ConvertTime(utcDateTime, CurrentTimeZone);

        public DateTimeOffset ToUtc(DateTimeOffset localDateTime) => TimeZoneInfo.ConvertTime(localDateTime, TimeZoneInfo.Utc);

        public DateTimeOffset Convert(DateTimeOffset dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone) => TimeZoneInfo.ConvertTime(dateTime, destinationTimeZone);

        public void Raise(TimeZoneInfo oldTimeZone, TimeZoneInfo newTimeZone)
        {
            CurrentTimeZone = newTimeZone;
            TimeZoneChanged?.Invoke(this, new(oldTimeZone, newTimeZone));
        }
    }

    private sealed class DummyEvent
    {
        public int Value { get; init; }
    }

    private sealed class AsyncEventAwareOwner : ObservableObject, IAsyncEventAware<DummyEvent>
    {
        public int Value { get; private set; }

        public int Calls { get; private set; }

        public Task OnEventAsync(DummyEvent e, CancellationToken cancellationToken = default)
        {
            Value = e.Value;
            Calls++;
            return Task.CompletedTask;
        }
    }

    private sealed class TestAsyncEventBehavior(ObservableObject owner) : EventBehavior<ObservableObject, DummyEvent>(owner)
    {
        public void Raise(DummyEvent evt) => RaiseEvent(evt);
    }
}
