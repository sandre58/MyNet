// -----------------------------------------------------------------------
// <copyright file="LocalizedRuleBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable.Validation;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Wraps a FluentValidation rule with a resolved property display key for localized messages.
/// </summary>
public sealed class LocalizedRuleBuilder<T, TProperty>(IRuleBuilder<T, TProperty> ruleBuilder, string propertyDisplayKey)
{
    /// <summary>
    /// Gets the underlying FluentValidation rule builder.
    /// </summary>
    public IRuleBuilder<T, TProperty> RuleBuilder { get; } = ruleBuilder;

    /// <summary>
    /// Gets the translation key used as {0} in validation message templates.
    /// </summary>
    public string PropertyDisplayKey { get; } = propertyDisplayKey;
}
