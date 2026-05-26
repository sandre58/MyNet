// -----------------------------------------------------------------------
// <copyright file="WindowsAuthenticationServiceTests.cs" company="Stéphane ANDRE">
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
public sealed class WindowsAuthenticationServiceTests : IDisposable
{
    private readonly IPrincipal? _initialPrincipal = Thread.CurrentPrincipal;

    [Fact]
    public void CurrentPrincipalReturnsAnonymousWhenThreadPrincipalTypeDiffers()
    {
        Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("DOMAIN\\existing"), []);
        var service = new TestAuthenticationService();

        Assert.Same(TestAuthenticationService.AnonymousPrincipal, service.CurrentPrincipal);
    }

    [Fact]
    public void AuthenticatedEventIsRaisedOnlyWhenAuthenticationStateChanges()
    {
        var service = new TestAuthenticationService();
        var raised = 0;
        service.Authenticated += (_, _) => raised++;

        service.SignIn("DOMAIN\\first");
        service.SignIn("DOMAIN\\second");
        service.Unauthenticate();
        service.Unauthenticate();

        Assert.Equal(2, raised);
    }

    [Fact]
    public void UnauthenticateResetsCurrentPrincipalToAnonymous()
    {
        var service = new TestAuthenticationService();
        service.SignIn("DOMAIN\\user");

        service.Unauthenticate();

        Assert.Same(TestAuthenticationService.AnonymousPrincipal, service.CurrentPrincipal);
        Assert.False(service.IsAuthenticated);
    }

    public void Dispose() => Thread.CurrentPrincipal = _initialPrincipal;

    private sealed class TestAuthenticationService : WindowsAuthenticationService<TestPrincipal>
    {
        public static readonly TestPrincipal AnonymousPrincipal = new(new GenericIdentity(string.Empty), []);

        public void SignIn(string name) => Authenticate(new(new GenericIdentity(name), []));

        protected override TestPrincipal GetAnonymous() => AnonymousPrincipal;

        protected override TestPrincipal CreatePrincipal(IIdentity identity) => new(identity, []);
    }

    private sealed class TestPrincipal(IIdentity identity, string[] roles) : GenericPrincipal(identity, roles);
}
