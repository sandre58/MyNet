// -----------------------------------------------------------------------
// <copyright file="TextRedactionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using MyNet.Text.Redaction;
using Xunit;

namespace MyNet.Text.Tests;

public partial class TextRedactionExtensionsTests
{
    [Fact]
    public void Redact_MasksMiddleSection()
    {
        var result = "1234567890".Redact(showStart: 2, showEnd: 2);

        Assert.Equal("12******90", result);
    }

    [Fact]
    public void Redact_WithRegex_RedactsMatches()
    {
        var result = "token=abcd1234".Redact(CustomRegex(), "[REDACTED]");

        Assert.Equal("token=[REDACTED]", result);
    }

    [Fact]
    public void Redact_EmailPreset_Works()
    {
        var result = "alice@example.com".Redact(Redactor.Email);

        Assert.StartsWith("a", result, StringComparison.Ordinal);
        Assert.Contains("@", result, StringComparison.Ordinal);
        Assert.EndsWith(".com", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Redact_PhonePreset_MasksAllDigitsExceptLastTwo()
    {
        var result = "+33 6 12 34 56 78".Redact(Redactor.Phone);

        Assert.Equal("+** * ** ** ** 78", result);
    }

    [Fact]
    public void Redact_CardNumberPreset_MasksAllDigitsExceptLastFour()
    {
        var result = "4111 1111 1111 1234".Redact(Redactor.CardNumber);

        Assert.Equal("**** **** **** 1234", result);
    }

    [GeneratedRegex("[a-z0-9]{8}", RegexOptions.Compiled)]
    private static partial Regex CustomRegex();
}
