// -----------------------------------------------------------------------
// <copyright file="RegistryValueConverterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.Versioning;
using Microsoft.Win32;
using MyNet.Platform.Windows.Registry;
using Xunit;

namespace MyNet.Platform.Windows.Tests;

[SupportedOSPlatform("windows")]
public sealed class RegistryValueConverterTests
{
    private readonly RegistryValueConverter _converter = new();

    [Fact]
    public void GetKind_ReturnsExpectedKindsForCommonTypes()
    {
        Assert.Equal(RegistryValueKind.DWord, _converter.GetKind(typeof(int)));
        Assert.Equal(RegistryValueKind.QWord, _converter.GetKind(typeof(long)));
        Assert.Equal(RegistryValueKind.String, _converter.GetKind(typeof(string)));
        Assert.Equal(RegistryValueKind.MultiString, _converter.GetKind(typeof(string[])));
        Assert.Equal(RegistryValueKind.DWord, _converter.GetKind(typeof(int?)));
    }

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ConvertToAndFrom_Bool_RoundTrips(bool value, int stored)
    {
        Assert.Equal(stored, _converter.ConvertTo(value));
        Assert.Equal(value, _converter.ConvertFrom(stored, typeof(bool)));
    }

    [Fact]
    public void ConvertToAndFrom_DateTime_RoundTrips()
    {
        var value = new DateTime(2024, 6, 15, 12, 30, 0, DateTimeKind.Utc);

        var stored = _converter.ConvertTo(value);
        var roundTripped = _converter.ConvertFrom(stored, typeof(DateTime));

        Assert.Equal(value, roundTripped);
    }

    [Fact]
    public void ConvertToAndFrom_Int_RoundTrips()
    {
        const int value = 42;

        var stored = _converter.ConvertTo(value);
        var roundTripped = _converter.ConvertFrom(stored, typeof(int));

        Assert.Equal(value, roundTripped);
    }

    [Fact]
    public void ConvertTo_WithNull_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => _converter.ConvertTo(null!));

    [Fact]
    public void ConvertFrom_WithNull_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => _converter.ConvertFrom(null!, typeof(string)));
}
