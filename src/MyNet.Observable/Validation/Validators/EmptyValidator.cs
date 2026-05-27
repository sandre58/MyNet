// -----------------------------------------------------------------------
// <copyright file="EmptyValidator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace MyNet.Observable.Validation.Validators;

/// <summary>
/// A validator that performs no validation and always returns a successful result.
/// </summary>
public sealed class EmptyValidator : IValidator
{
    /// <summary>
    /// A singleton instance of the <see cref="EmptyValidator"/> that can be reused wherever a no-op validator is needed.
    /// </summary>
    public static readonly EmptyValidator Instance = new();

    private EmptyValidator()
    {
    }

    /// <inheritdoc/>
    public ValidationResult Validate(IValidationContext context) => new();

    /// <inheritdoc/>
    public Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = default) => Task.FromResult(new ValidationResult());

    /// <inheritdoc/>
    public IValidatorDescriptor CreateDescriptor() => new ValidatorDescriptor<object>([]);

    /// <inheritdoc/>
    public bool CanValidateInstancesOfType(Type type) => true;
}
