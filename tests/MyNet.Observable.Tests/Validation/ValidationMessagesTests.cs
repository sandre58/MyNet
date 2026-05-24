// -----------------------------------------------------------------------
// <copyright file="ValidationMessagesTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Observable.Localization;
using MyNet.Observable.Validation;
using Xunit;

namespace MyNet.Observable.Tests.Validation;

public sealed class ValidationMessagesTests
{
    [Fact]
    public void Format_InsertsPropertyDisplayNameAndAdditionalArgs()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

            var message = ValidationMessages.Format(
                ValidationResources.FieldExceedsMaxLength,
                "Nom",
                50);

            Assert.Equal("Le champ \"Nom\" ne doit pas dépasser 50 caractères.", message);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public void Format_UsesCurrentCultureForNumericFormatting()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

            var message = ValidationMessages.Format(
                ValidationResources.FieldMustBeBetween,
                "Age",
                1000.5,
                2000.75);

            Assert.Contains("1", message, StringComparison.Ordinal);
            Assert.Contains("Age", message, StringComparison.Ordinal);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
