// -----------------------------------------------------------------------
// <copyright file="BoundedValueValidator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;
using FluentValidation;

namespace MyNet.Observable.Validation.Validators;

/// <summary>
/// FluentValidation rules for <see cref="BoundedValue{T}"/>.
/// </summary>
/// <typeparam name="T">The numeric type.</typeparam>
public sealed class BoundedValueValidator<T> : AbstractValidator<BoundedValue<T>>
    where T : struct, INumber<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedValueValidator{T}"/> class.
    /// </summary>
    /// <param name="propertyDisplayKey">Translation key used as the field name in messages.</param>
    public BoundedValueValidator(string propertyDisplayKey) => RuleFor(x => x.Value)
        .Must((model, value) => model.Range.Contains(value!.Value))
        .When(x => x.Value.HasValue)
        .WithMessage((model, _) => model.BuildRangeValidationMessage(propertyDisplayKey));
}
