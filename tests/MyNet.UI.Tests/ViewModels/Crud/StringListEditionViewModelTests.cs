// -----------------------------------------------------------------------
// <copyright file="StringListEditionViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using MyNet.UI.ViewModels.Crud;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Crud;

public class StringListEditionViewModelTests
{
    [Fact]
    public void Load_PopulatesRows_AndMarksClean()
    {
        var sut = new StringListEditionViewModel();
        sut.Load(["alpha", "beta"]);

        sut.Items.Should().HaveCountGreaterThanOrEqualTo(2);
        sut.Count.Should().Be(2);
        sut.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void AddCommand_AddsRow_WhenLastRowHasValue()
    {
        var sut = new StringListEditionViewModel();
        sut.Load(["first"]);
        var lastRow = sut.Items[^1];
        lastRow.Value = "second";

        sut.AddCommand.Execute(lastRow);

        sut.Items.Should().HaveCount(2);
        sut.IsDirty.Should().BeTrue();
    }

    [Fact]
    public void RemoveCommand_RemovesRow_WhenMoreThanOneRowExists()
    {
        var sut = new StringListEditionViewModel();
        sut.Load(["keep", "remove"]);
        var toRemove = sut.Items[1];

        sut.RemoveCommand.Execute(toRemove);

        sut.Items.Should().NotContain(toRemove);
        sut.GetValues().Should().BeEquivalentTo(["keep"]);
    }

    [Fact]
    public void ApplyTo_ReplacesTargetCollection()
    {
        var sut = new StringListEditionViewModel();
        sut.Load(["one", "two"]);
        var target = new List<string> { "old" };

        sut.ApplyTo(target);

        target.Should().Equal("one", "two");
    }
}
