// -----------------------------------------------------------------------
// <copyright file="TranslationKeyProviderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Localization.Translation.KeyGeneration;
using Xunit;

namespace MyNet.Globalization.Tests.Translation;

public sealed class TranslationKeyProviderTests
{
    private readonly TranslationKeyProvider _sut = new();

    [Fact]
    public void GetKey_ForEnum_ConcatenatesTypeNameAndValue()
    {
        var key = _sut.GetKey(DayOfWeek.Monday);

        Assert.Equal($"{nameof(DayOfWeek)}{DayOfWeek.Monday}", key);
    }

    [Fact]
    public void GetKey_ForPlainObject_ConcatenatesTypeNameAndToString()
    {
        var key = _sut.GetKey(42);

        Assert.Equal("Int3242", key);
    }

    [Fact]
    public void GetKey_NullValue_Throws() => Assert.Throws<ArgumentNullException>(() => _sut.GetKey(null!));
}
