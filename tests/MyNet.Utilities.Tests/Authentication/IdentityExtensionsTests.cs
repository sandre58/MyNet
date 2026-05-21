// -----------------------------------------------------------------------
// <copyright file="IdentityExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Principal;
using Xunit;

namespace MyNet.Utilities.Tests.Authentication;

public class IdentityExtensionsTests
{
    [Theory]
    [InlineData("DOMAIN\\Alice", "DOMAIN", "Alice")]
    [InlineData("Alice", "", "Alice")]
    [InlineData("", "", "")]
    public void GetDomainAndGetName_DelegatesToIdentityHelper(string identityName, string expectedDomain, string expectedName)
    {
        IIdentity identity = new GenericIdentity(identityName);

        Assert.Equal(expectedDomain, identity.GetDomain());
        Assert.Equal(expectedName, identity.GetName());
    }
}
