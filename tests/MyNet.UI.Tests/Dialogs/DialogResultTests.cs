// -----------------------------------------------------------------------
// <copyright file="DialogResultTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Dialogs.MessageBox;
using Xunit;

namespace MyNet.UI.Tests.Dialogs;

public class DialogResultTests
{
    [Fact]
    public void Success_SetsOutcomeAndValue()
    {
        var result = DialogResult<string>.Success("ok");

        result.IsSuccess.Should().BeTrue();
        result.IsCancelled.Should().BeFalse();
        result.IsDismissed.Should().BeFalse();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public void Cancelled_And_Dismissed_SetOutcomeOnly()
    {
        DialogResult<int>.Cancelled().IsCancelled.Should().BeTrue();
        DialogResult<int>.Dismissed().IsDismissed.Should().BeTrue();
    }

    [Fact]
    public void BoolHelpers_MapToExpectedOutcomes()
    {
        DialogResult.Ok().IsSuccess.Should().BeTrue();
        DialogResult.Cancel().IsCancelled.Should().BeTrue();
        DialogResult.Dismiss().IsDismissed.Should().BeTrue();
    }

    [Fact]
    public void MessageBoxService_MapsCancelledOutcome()
    {
        var cancelled = DialogResult<MessageBoxResult>.Cancelled();
        cancelled.IsCancelled.Should().BeTrue();
    }
}
