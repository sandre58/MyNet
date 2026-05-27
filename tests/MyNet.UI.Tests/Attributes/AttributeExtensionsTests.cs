// -----------------------------------------------------------------------
// <copyright file="AttributeExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Attributes;
using Xunit;

namespace MyNet.UI.Tests.Attributes;

public sealed class AttributeExtensionsTests
{
    [Fact]
    public void IsRegisteredAsSingleton_ShouldReturnTrue_WhenAttributeIsPresent()
        => Assert.True(typeof(SingletonService).IsRegisteredAsSingleton());

    [Fact]
    public void IsRegisteredAsTransient_ShouldReturnTrue_WhenAttributeIsPresent()
        => Assert.True(typeof(TransientService).IsRegisteredAsTransient());

    [Fact]
    public void RegistrationChecks_ShouldReturnFalse_WhenNoAttributeIsPresent()
    {
        Assert.False(typeof(PlainService).IsRegisteredAsSingleton());
        Assert.False(typeof(PlainService).IsRegisteredAsTransient());
    }

    [IsSingleton]
    private sealed class SingletonService;

    [IsTransient]
    private sealed class TransientService;

    private sealed class PlainService;
}
