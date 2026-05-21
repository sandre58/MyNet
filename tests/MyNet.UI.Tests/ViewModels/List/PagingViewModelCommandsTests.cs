// -----------------------------------------------------------------------
// <copyright file="PagingViewModelCommandsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.ViewModels.List.Paging;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.List;

/// <summary>
/// P0 Test 5 – PagingViewModel commands.
/// Validates that navigation commands correctly change the current page
/// and respect their CanExecute guards.
/// </summary>
public sealed class PagingViewModelCommandsTests
{
    private static PagingViewModel CreatePager(int totalItems = 100, int pageSize = 10, int startPage = 1)
    {
        var vm = new PagingViewModel(pageSize);
        vm.Update(totalItems, startPage);
        return vm;
    }

    // ── MoveNext ─────────────────────────────────────────────────────────────
    [Fact]
    public void MoveNextCommand_Should_IncrementPage()
    {
        var vm = CreatePager(startPage: 1);

        vm.MoveNextCommand.Execute(null);

        vm.CurrentPage.Should().Be(2);
    }

    [Fact]
    public void MoveNextCommand_CanExecute_Should_Be_False_On_LastPage()
    {
        var vm = CreatePager(totalItems: 20, pageSize: 10, startPage: 2);

        vm.MoveNextCommand.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void MoveNextCommand_CanExecute_Should_Be_True_Before_LastPage()
    {
        var vm = CreatePager(startPage: 1);

        vm.MoveNextCommand.CanExecute(null).Should().BeTrue();
    }

    // ── MovePrevious ──────────────────────────────────────────────────────────
    [Fact]
    public void MovePreviousCommand_Should_DecrementPage()
    {
        var vm = CreatePager(startPage: 3);

        vm.MovePreviousCommand.Execute(null);

        vm.CurrentPage.Should().Be(2);
    }

    [Fact]
    public void MovePreviousCommand_CanExecute_Should_Be_False_On_FirstPage()
    {
        var vm = CreatePager(startPage: 1);

        vm.MovePreviousCommand.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void MovePreviousCommand_CanExecute_Should_Be_True_After_FirstPage()
    {
        var vm = CreatePager(startPage: 3);

        vm.MovePreviousCommand.CanExecute(null).Should().BeTrue();
    }

    // ── MoveFirst ────────────────────────────────────────────────────────────
    [Fact]
    public void MoveFirstCommand_Should_NavigateTo_FirstPage()
    {
        var vm = CreatePager(startPage: 5);

        vm.MoveFirstCommand.Execute(null);

        vm.CurrentPage.Should().Be(1);
    }

    [Fact]
    public void MoveFirstCommand_CanExecute_Should_Be_False_On_FirstPage()
    {
        var vm = CreatePager(startPage: 1);

        vm.MoveFirstCommand.CanExecute(null).Should().BeFalse();
    }

    // ── MoveLast ─────────────────────────────────────────────────────────────
    [Fact]
    public void MoveLastCommand_Should_NavigateTo_LastPage()
    {
        var vm = CreatePager(totalItems: 100, pageSize: 10, startPage: 1);

        vm.MoveLastCommand.Execute(null);

        vm.CurrentPage.Should().Be(10);
    }

    [Fact]
    public void MoveLastCommand_CanExecute_Should_Be_False_On_LastPage()
    {
        var vm = CreatePager(totalItems: 20, pageSize: 10, startPage: 2);

        vm.MoveLastCommand.CanExecute(null).Should().BeFalse();
    }

    // ── PagingChanged event ──────────────────────────────────────────────────
    [Fact]
    public void MoveNextCommand_Should_RaisePagingChanged()
    {
        var vm = CreatePager(startPage: 1);
        var eventRaised = false;
        vm.PagingChanged += (_, _) => eventRaised = true;

        vm.MoveNextCommand.Execute(null);

        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void PagingChanged_EventArgs_Should_ContainNewPage()
    {
        var vm = CreatePager(startPage: 1);
        int? reportedPage = null;
        vm.PagingChanged += (_, e) => reportedPage = e.Page;

        vm.MoveNextCommand.Execute(null);

        reportedPage.Should().Be(2);
    }

    // ── Properties consistency ────────────────────────────────────────────────
    [Fact]
    public void HasNextPage_Should_Be_True_When_Not_On_LastPage()
    {
        var vm = CreatePager(startPage: 1);
        vm.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasPreviousPage_Should_Be_False_On_FirstPage()
    {
        var vm = CreatePager(startPage: 1);
        vm.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void TotalPages_Should_Be_Correct()
    {
        var vm = CreatePager(totalItems: 25, pageSize: 10);
        vm.TotalPages.Should().Be(3); // ceil(25/10)
    }

    [Fact]
    public void Update_WithZeroItems_Should_SetCurrentPageTo1()
    {
        var vm = new PagingViewModel(10);
        vm.Update(0, 5);

        vm.CurrentPage.Should().Be(1);
        vm.TotalPages.Should().Be(0);
        vm.HasNextPage.Should().BeFalse();
        vm.HasPreviousPage.Should().BeFalse();
    }
}
