// -----------------------------------------------------------------------
// <copyright file="DisplayModeViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using MyNet.UI.ViewModels.Display;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Display;

public class DisplayModeViewModelTests
{
    [Fact]
    public void ListDisplayMode_SetDisplayedColumns_UpdatesColumnVisibility()
    {
        var sut = new ListDisplayModeViewModel();
        sut.ColumnOptions.Add(new("A"));
        sut.ColumnOptions.Add(new("B"));

        sut.SetDisplayedColumns(["B"]);

        sut.ColumnOptions.First(x => x.Identifier == "A").IsVisible.Should().BeFalse();
        sut.ColumnOptions.First(x => x.Identifier == "B").IsVisible.Should().BeTrue();
    }

    [Fact]
    public void CalendarDisplayMode_MoveCommands_UpdateDisplayDate()
    {
        var sut = new DayDisplayModeViewModel();
        var initial = sut.DisplayDate;

        sut.MoveToNextDateCommand.Execute(null);
        sut.DisplayDate.Should().Be(initial.AddDays(1));

        sut.MoveToPreviousDateCommand.Execute(null);
        sut.DisplayDate.Should().Be(initial);

        sut.ResetCommand.Execute(null);
        sut.DisplayDate.Date.Should().Be(DateTime.Now.Date);
    }

    [Fact]
    public void DisplaySelector_AddMode_SetsCurrentMode()
    {
        var grid = new GridDisplayModeViewModel();
        var list = new ListDisplayModeViewModel();
        var sut = new DisplaySelectorViewModel();

        sut.AddMode(grid, isDefault: true);
        sut.AddMode(list);

        sut.CurrentMode.Should().BeSameAs(grid);

        sut.SetModeCommand.Execute(list);

        sut.CurrentMode.Should().BeSameAs(list);
    }
}
