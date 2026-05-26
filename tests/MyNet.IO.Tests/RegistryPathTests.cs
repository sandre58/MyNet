// -----------------------------------------------------------------------
// <copyright file="RegistryPathTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.IO.Registry;
using Xunit;

namespace MyNet.IO.Tests;

public sealed class RegistryPathTests
{
    [Fact]
    public void Combine_CreatesPathWithBackslash()
    {
        var path = RegistryPath.Combine(@"HKEY_CURRENT_USER\Software", "MyApp");

        Assert.Equal(@"HKEY_CURRENT_USER\Software\MyApp", path.ToString());
    }

    [Fact]
    public void RegistryEntry_ExposesPathAndItem()
    {
        var path = new RegistryPath("HKEY_CURRENT_USER", "MyApp");
        var entry = new RegistryEntry<string>(path, "value");

        Assert.Equal(path, entry.Path);
        Assert.Equal("value", entry.Item);
    }
}
