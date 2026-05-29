// -----------------------------------------------------------------------
// <copyright file="BusyTaskbarCoordinatorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;
using MyNet.UI.ViewModels.Shell.Taskbar;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class BusyTaskbarCoordinatorTests
{
    [Fact]
    public async Task WhenApplicationBusyStarts_SetsIndeterminateDuringOperation()
    {
        var busy = new BusyService();
        var taskbar = new TaskbarProgressSource();
        using var coordinator = new BusyTaskbarCoordinator(busy, taskbar);

        TaskbarProgressState stateDuringOperation = default;

        await busy.RunAsync<IndeterminateBusy>((_, _) =>
        {
            stateDuringOperation = taskbar.ProgressState;
            return Task.CompletedTask;
        });

        stateDuringOperation.Should().Be(TaskbarProgressState.Indeterminate);
        taskbar.ProgressState.Should().Be(TaskbarProgressState.None);
    }

    [Fact]
    public async Task WhenProgressionBusyReportsValue_SetsNormalProgress()
    {
        var busy = new BusyService();
        var taskbar = new TaskbarProgressSource();
        using var coordinator = new BusyTaskbarCoordinator(busy, taskbar);

        TaskbarProgressState stateDuringOperation = default;
        double valueDuringOperation = default;

        await busy.RunAsync<ProgressionBusy>((progression, _) =>
        {
            progression.Value = 0.42;
            stateDuringOperation = taskbar.ProgressState;
            valueDuringOperation = taskbar.ProgressValue;
            return Task.CompletedTask;
        });

        stateDuringOperation.Should().Be(TaskbarProgressState.Normal);
        valueDuringOperation.Should().Be(0.42);
        taskbar.ProgressState.Should().Be(TaskbarProgressState.None);
    }

    [Fact]
    public void SetError_PreservedUntilNextBusyCycle()
    {
        var busy = new BusyService();
        var taskbar = new TaskbarProgressSource();
        using var coordinator = new BusyTaskbarCoordinator(busy, taskbar);

        taskbar.SetError();

        busy.IsBusy.Should().BeFalse();
        taskbar.ProgressState.Should().Be(TaskbarProgressState.Error);
    }

    [Fact]
    public async Task WhenProgressionBusyCancels_SetsPaused()
    {
        var busy = new BusyService();
        var taskbar = new TaskbarProgressSource();
        using var coordinator = new BusyTaskbarCoordinator(busy, taskbar);

        TaskbarProgressState stateDuringOperation = default;

        await busy.RunAsync<ProgressionBusy>((progression, _) =>
        {
            progression.Value = 0.5;
            progression.Cancel();
            stateDuringOperation = taskbar.ProgressState;
            return Task.CompletedTask;
        });

        stateDuringOperation.Should().Be(TaskbarProgressState.Paused);
        taskbar.ProgressValue.Should().Be(0.5);
    }
}
