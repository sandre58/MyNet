// -----------------------------------------------------------------------
// <copyright file="FileNameTextSanitizerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Text.Sanitization;
using Xunit;

namespace MyNet.Text.Tests;

public sealed class FileNameTextSanitizerTests
{
    private readonly FileNameTextSanitizer _sut = new();

    [Fact]
    public void Apply_RemovesInvalidFileNameCharacters()
    {
        var result = _sut.Apply("repor:t*2026?.txt", CultureInfo.InvariantCulture);

        Assert.Equal("report2026.txt", result);
    }

    [Fact]
    public void Apply_TrimsWhitespace()
    {
        var result = _sut.Apply("  name  ", CultureInfo.InvariantCulture);

        Assert.Equal("name", result);
    }

    [Fact]
    public void Apply_NullInput_Throws() => Assert.Throws<ArgumentNullException>(() => _sut.Apply(null!, CultureInfo.InvariantCulture));
}
