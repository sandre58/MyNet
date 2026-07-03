// -----------------------------------------------------------------------
// <copyright file="TranslationExtensionsFacadeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Facade;
using Xunit;

namespace MyNet.Globalization.Tests.Facade;

public class TranslationExtensionsFacadeTests
{
    [Fact]
    public void TranslateOr_UsesPlainTextWhenNoResourceKey() =>
        Assert.Equal("(none)", (((string?)null)!).TranslateOr("(none)"));

    [Fact]
    public void TranslateOr_ReturnsEmptyWhenNoKeyOrFallback() =>
        Assert.Equal(string.Empty, (((string?)null)!).TranslateOr());
}
