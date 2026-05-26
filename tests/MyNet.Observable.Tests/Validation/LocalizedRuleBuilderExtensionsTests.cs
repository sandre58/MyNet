// -----------------------------------------------------------------------
// <copyright file="LocalizedRuleBuilderExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using FluentValidation;
using MyNet.Observable.Localization;
using MyNet.Primitives;
using Xunit;

namespace MyNet.Observable.Tests.Validation;

public sealed class LocalizedRuleBuilderExtensionsTests
{
    [Fact]
    public void NotEmptyRequired_UsesValidationResources()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

            var validator = new SampleValidator();
            var result = validator.Validate(new SampleModel { Name = string.Empty });

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage == ValidationResources.FieldIsRequired.FormatWith(
                    CultureInfo.CurrentCulture,
                    nameof(SampleModel.Name)));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public void MaximumLength_UsesValidationResources()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

            var validator = new SampleValidator();
            var result = validator.Validate(new SampleModel { Name = new('a', 11) });

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage == ValidationResources.FieldExceedsMaxLength.FormatWith(
                    CultureInfo.CurrentCulture,
                    nameof(SampleModel.Name),
                    10));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public void InclusiveBetween_UsesValidationResources()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

            var validator = new SampleValidator();
            var result = validator.Validate(new SampleModel { Name = "ok", Age = 150 });

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage == ValidationResources.FieldMustBeBetween.FormatWith(
                    CultureInfo.CurrentCulture,
                    nameof(SampleModel.Age),
                    0,
                    120));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    private sealed class SampleModel
    {
        public string Name { get; init; } = string.Empty;

        public int Age { get; init; }
    }

    private sealed class SampleValidator : AbstractValidator<SampleModel>
    {
        public SampleValidator()
        {
            this.RuleForLocalized(x => x.Name).NotEmptyRequired();
            this.RuleForLocalized(x => x.Name).MaximumLength(10);
            this.RuleForLocalized(x => x.Age).InclusiveBetween(0, 120);
        }
    }
}
