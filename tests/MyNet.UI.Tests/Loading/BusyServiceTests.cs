// -----------------------------------------------------------------------
// <copyright file="BusyServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;
using Xunit;

namespace MyNet.UI.Tests.Loading;

public class BusyServiceTests
{
    [Fact]
    public async Task RunAsync_SetsIsBusyDuringOperationAsync()
    {
        var service = new BusyService();
        var wasBusy = false;

        await service.RunAsync<IndeterminateBusy>((_, _) =>
        {
            wasBusy = service.IsBusy;
            return Task.CompletedTask;
        });

        wasBusy.Should().BeTrue();
        service.IsBusy.Should().BeFalse();
    }

    [Fact]
    public async Task NestedScopes_KeepIsBusyUntilAllDisposedAsync()
    {
        var service = new BusyService();

        await service.RunAsync<IndeterminateBusy>(async (_, y) =>
        {
            service.IsBusy.Should().BeTrue();

            await service.RunAsync<DeterminateBusy>((_, _) =>
                {
                    service.IsBusy.Should().BeTrue();
                    service.GetCurrent<DeterminateBusy>().Should().NotBeNull();

                    return Task.CompletedTask;
                },
                y).ConfigureAwait(false);

            service.IsBusy.Should().BeTrue();
        }).ConfigureAwait(true);

        service.IsBusy.Should().BeFalse();
    }

    [Fact]
    public void PropertyChanged_RaisedWhenBusyStateChanges()
    {
        var service = new BusyService();
        var isBusyChanges = 0;

        service.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IBusyService.IsBusy))
                isBusyChanges++;
        };

        using (service.Begin<ProgressionBusy>())
        {
            service.IsBusy.Should().BeTrue();
            service.CurrentBusy.Should().BeOfType<ProgressionBusy>();
        }

        isBusyChanges.Should().Be(2);
        service.IsBusy.Should().BeFalse();
        service.CurrentBusy.Should().BeNull();
    }

    [Fact]
    public void ProgressionBusy_UpdatesCurrentBusy()
    {
        var service = new BusyService();

        using (service.Begin<ProgressionBusy>())
        {
            var progression = service.GetCurrent<ProgressionBusy>();
            progression.Should().NotBeNull();
            progression.Report(0.5, "Step 1");

            progression.Value.Should().Be(0.5);
            progression.Title.Should().Be("Step 1");
            service.CurrentBusy.Should().BeSameAs(progression);
        }
    }
}
