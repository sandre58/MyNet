// -----------------------------------------------------------------------
// <copyright file="WindowsAuthenticationConcreteTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading;
using MyNet.Platform.Windows.Authentication;
using Xunit;

namespace MyNet.Platform.Windows.Tests;

[SupportedOSPlatform("windows")]
public sealed class WindowsAuthenticationConcreteTests : IDisposable
{
    private readonly IPrincipal? _initialPrincipal = Thread.CurrentPrincipal;

    [Fact]
    public void Authenticate_SetsCurrentWindowsPrincipal()
    {
        var service = new WindowsAuthenticationService();

        service.Authenticate();

        Assert.True(service.IsAuthenticated);
        Assert.IsType<WindowsUserPrincipal>(service.CurrentPrincipal);
    }

    [Fact]
    public void Unauthenticate_ResetsToSharedAnonymousPrincipal()
    {
        var service = new WindowsAuthenticationService();
        service.Authenticate();

        service.Unauthenticate();

        Assert.Same(WindowsAuthenticationService.Anonymous, service.CurrentPrincipal);
        Assert.False(service.IsAuthenticated);
    }

    [Fact]
    public void WindowsUserPrincipal_ExposesNameAndDomain()
    {
        var principal = new WindowsUserPrincipal(new GenericIdentity("DOMAIN\\user"), []);

        Assert.Equal("user", principal.Name, StringComparer.Ordinal);
        Assert.Equal("DOMAIN", principal.Domain, StringComparer.Ordinal);
    }

    [Fact]
    public void WindowsUserPrincipal_Constructor_WithNullRoles_Throws()
        => Assert.Throws<ArgumentNullException>(() => new WindowsUserPrincipal(new GenericIdentity("user"), null!));

    public void Dispose() => Thread.CurrentPrincipal = _initialPrincipal;
}
