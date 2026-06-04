// -----------------------------------------------------------------------
// <copyright file="CommandFactoryExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.Commands;
using Xunit;

namespace MyNet.UI.Tests.Commands;

public sealed class CommandFactoryExtensionsTests
{
    [Fact]
    public void GetOrDefault_WhenNull_ReturnsDefaultFactory()
    {
        ICommandFactory? factory = null;

        factory.GetOrDefault().Should().BeSameAs(RelayCommandFactory.Default);
    }

    [Fact]
    public void GetOrDefault_WhenProvided_ReturnsSameInstance()
    {
        var factory = new RelayCommandFactory();

        factory.GetOrDefault().Should().BeSameAs(factory);
    }
}
