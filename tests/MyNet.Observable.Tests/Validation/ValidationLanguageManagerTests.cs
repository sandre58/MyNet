// -----------------------------------------------------------------------
// <copyright file="ValidationLanguageManagerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using FluentValidation;
using MyNet.Observable.Localization;
using MyNet.Observable.Validation;
using Xunit;

namespace MyNet.Observable.Tests.Validation;

public sealed class ValidationLanguageManagerTests
{
    [Fact]
    public void NotEmpty_UsesValidationResources_WhenLanguageManagerIsConfigured()
    {
        var previousManager = ValidatorOptions.Global.LanguageManager;
        var previousCulture = CultureInfo.CurrentUICulture;

        try
        {
            ValidatorOptions.Global.LanguageManager = new ValidationLanguageManager();
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

            var validator = new NameValidator();
            var result = validator.Validate(new Person { Name = string.Empty });

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage.Contains("obligatoire", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            ValidatorOptions.Global.LanguageManager = previousManager;
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    [Fact]
    public void MaximumLength_UsesValidationResources_WhenLanguageManagerIsConfigured()
    {
        var previousManager = ValidatorOptions.Global.LanguageManager;
        var previousCulture = CultureInfo.CurrentUICulture;

        try
        {
            ValidatorOptions.Global.LanguageManager = new ValidationLanguageManager();
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

            var validator = new NameValidator();
            var result = validator.Validate(new Person { Name = new('a', 11) });

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage == ValidationResources.FieldExceedsMaxLength
                    .Replace("{0}", "{PropertyName}", StringComparison.Ordinal)
                    .Replace("{1}", "{MaxLength}", StringComparison.Ordinal)
                    .Replace("{PropertyName}", nameof(Person.Name), StringComparison.Ordinal)
                    .Replace("{MaxLength}", "10", StringComparison.Ordinal));
        }
        finally
        {
            ValidatorOptions.Global.LanguageManager = previousManager;
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    [Fact]
    public void Configure_IsIdempotent()
    {
        var previousManager = ValidatorOptions.Global.LanguageManager;

        try
        {
            ValidationLocalization.Configure();
            var first = ValidatorOptions.Global.LanguageManager;
            ValidationLocalization.Configure();
            var second = ValidatorOptions.Global.LanguageManager;

            Assert.Same(first, second);
            Assert.IsType<ValidationLanguageManager>(first);
        }
        finally
        {
            ValidatorOptions.Global.LanguageManager = previousManager;
        }
    }

    private sealed class Person
    {
        public string Name { get; init; } = string.Empty;
    }

    private sealed class NameValidator : AbstractValidator<Person>
    {
        public NameValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).MaximumLength(10);
        }
    }
}
