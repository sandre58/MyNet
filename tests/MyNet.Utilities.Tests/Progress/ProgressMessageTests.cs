// -----------------------------------------------------------------------
// <copyright file="ProgressMessageTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Utilities.Progress;
using Xunit;

namespace MyNet.Utilities.Tests.Progress;

public class ProgressMessageTests
{
    [Fact]
    public void ToString_WithoutParameters_ReturnsRawMessage()
    {
        var message = new ProgressMessage("Loading");

        Assert.Equal("Loading", message.ToString());
    }

    [Fact]
    public void ToString_WithParameters_ReturnsFormattedMessage()
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            var message = new ProgressMessage("{0:0.00}% complete", 12.345);
            Assert.Equal("12.35% complete", message.ToString());
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
