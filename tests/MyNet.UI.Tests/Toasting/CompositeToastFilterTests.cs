// -----------------------------------------------------------------------
// <copyright file="CompositeToastFilterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Moq;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Filters;
using Xunit;

namespace MyNet.UI.Tests.Toasting;

public sealed class CompositeToastFilterTests
{
    [Fact]
    public void ShouldDisplay_ReturnsTrueOnlyWhenAllFiltersPass()
    {
        var notification = new MessageNotification("hello");
        var first = new Mock<IToastFilter>();
        var second = new Mock<IToastFilter>();
        first.Setup(x => x.ShouldDisplay(notification)).Returns(true);
        second.Setup(x => x.ShouldDisplay(notification)).Returns(true);

        new CompositeToastFilter([first.Object, second.Object]).ShouldDisplay(notification).Should().BeTrue();
    }

    [Fact]
    public void ShouldDisplay_ReturnsFalseWhenAnyFilterRejects()
    {
        var notification = new MessageNotification("hello");
        var first = new Mock<IToastFilter>();
        var second = new Mock<IToastFilter>();
        first.Setup(x => x.ShouldDisplay(notification)).Returns(true);
        second.Setup(x => x.ShouldDisplay(notification)).Returns(false);

        new CompositeToastFilter([first.Object, second.Object]).ShouldDisplay(notification).Should().BeFalse();
    }
}
