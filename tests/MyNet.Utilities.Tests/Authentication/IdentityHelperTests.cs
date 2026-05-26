// -----------------------------------------------------------------------
// <copyright file="IdentityHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Authentication;
using Xunit;

namespace MyNet.Utilities.Tests.Authentication;

public class IdentityHelperTests
{
    [Theory]
    [InlineData(null, "", "")]
    [InlineData("", "", "")]
    [InlineData("UserOnly", "", "UserOnly")]
    [InlineData("DOMAIN\\User", "DOMAIN", "User")]
    [InlineData("DOMAIN\\", "DOMAIN", "")]
    [InlineData("\\User", "", "User")]
    [InlineData(@"DOMAIN\Team\User", "DOMAIN", "Team\\User")]
    public void CanParseIdentityParts(string? identity, string expectedDomain, string expectedName)
    {
        Assert.Equal(expectedDomain, IdentityHelper.GetDomain(identity));
        Assert.Equal(expectedName, IdentityHelper.GetName(identity));
    }
}
