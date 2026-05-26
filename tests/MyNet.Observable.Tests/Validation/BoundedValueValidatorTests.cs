// -----------------------------------------------------------------------
// <copyright file="BoundedValueValidatorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.Observable.Validation.Validators;
using Xunit;

namespace MyNet.Observable.Tests.Validation;

public sealed class BoundedValueValidatorTests
{
    [Fact]
    public void Validate_ValueInsideRange_Succeeds()
    {
        var model = new BoundedValue<int>(0, 10) { Value = 5 };
        var sut = new BoundedValueValidator<int>("Value");

        var result = sut.Validate(model);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ValueOutsideRange_Fails()
    {
        var model = new BoundedValue<int>(0, 10) { Value = 20 };
        var sut = new BoundedValueValidator<int>("Value");

        var result = sut.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void Validate_NullValue_Succeeds()
    {
        var model = new BoundedValue<int>(0, 10) { Value = null };
        var sut = new BoundedValueValidator<int>("Value");

        var result = sut.Validate(model);

        result.IsValid.Should().BeTrue();
    }
}
