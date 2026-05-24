// -----------------------------------------------------------------------
// <copyright file="AbstractValidatorLocalizedExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using FluentValidation;
using MyNet.Observable.Validation;
using MyNet.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Starts a localized validation rule from a property expression.
/// </summary>
public static class AbstractValidatorLocalizedExtensions
{
    /// <summary>
    /// Begins a rule for <paramref name="expression"/> with the property display key inferred from the member name.
    /// </summary>
    /// <param name="validator">The validator instance.</param>
    /// <param name="expression">The property expression (e.g. <c>x => x.Email</c>).</param>
    /// <param name="propertyDisplayKey">Optional translation key when it differs from the member name.</param>
    public static LocalizedRuleBuilder<T, TProperty> RuleForLocalized<T, TProperty>(
        this AbstractValidator<T> validator,
        Expression<Func<T, TProperty>> expression,
        string? propertyDisplayKey = null)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(expression);

        var key = propertyDisplayKey ?? expression.PropertyName();
        return new(validator.RuleFor(expression), key);
    }
}
