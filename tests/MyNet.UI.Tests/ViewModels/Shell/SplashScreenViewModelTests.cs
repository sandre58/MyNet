// -----------------------------------------------------------------------
// <copyright file="SplashScreenViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Shell.Startup;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Shell;

public class SplashScreenViewModelTests
{
    [Fact]
    public async Task ExecuteAsync_RunsTasksInOrder_AndInvokesCompletedCallbackAsync()
    {
        var vm = new SplashScreenViewModel(new ApplicationInfo());
        var order = 0;
        var first = 0;
        var second = 0;

        vm.AddTask("Step 1", () =>
        {
            first = ++order;
            return Task.CompletedTask;
        });
        vm.AddTask("Step 2", () =>
        {
            second = ++order;
            return Task.CompletedTask;
        });

        var completed = false;
        await vm.ExecuteAsync(completedCallBack: () => completed = true);

        first.Should().Be(1);
        second.Should().Be(2);
        completed.Should().BeTrue();
        vm.IsBusy.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_SkipsTask_WhenCanExecuteReturnsFalseAsync()
    {
        var vm = new SplashScreenViewModel(new ApplicationInfo());
        var executed = false;

        vm.AddTask("Skipped",
            () =>
            {
                executed = true;
                return Task.CompletedTask;
            },
            () => false);

        await vm.ExecuteAsync();

        executed.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_OnFailure_InvokesFailedCallbackAndClearsBusyAsync()
    {
        var vm = new SplashScreenViewModel(new ApplicationInfo());
        var expected = new InvalidOperationException("startup failed");

        vm.AddTask("Fail", () => throw expected);

        Exception? caught = null;
        await vm.ExecuteAsync(failedCallback: e => caught = e);

        caught.Should().BeSameAs(expected);
        vm.IsBusy.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesMessageWithEllipsisAsync()
    {
        var vm = new SplashScreenViewModel(new ApplicationInfo());
        vm.AddTask("Loading", () => Task.CompletedTask);

        await vm.ExecuteAsync();

        vm.Message.Should().Be("Loading...");
    }
}
