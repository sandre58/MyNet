// -----------------------------------------------------------------------
// <copyright file="SupportedCulturesTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Culture;
using Xunit;

namespace MyNet.Globalization.Tests.Culture;

public sealed class SupportedCulturesTests
{
    [Fact]
    public void English_HasEnCultureName() => Assert.Equal("en", SupportedCultures.English.TwoLetterISOLanguageName);

    [Fact]
    public void French_HasFrCultureName() => Assert.Equal("fr", SupportedCultures.French.TwoLetterISOLanguageName);
}
