// -----------------------------------------------------------------------
// <copyright file="LocalizedStringRuleBuilderExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentValidation;
using MyNet.Observable.Validation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Localized validation rules for string properties.
/// </summary>
public static class LocalizedStringRuleBuilderExtensions
{
    extension<T>(LocalizedRuleBuilder<T, string> rule)
    {
        /// <summary>
        /// Adds a "not empty" validation rule with a localized message indicating that the field is required.
        /// </summary>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, string> NotEmptyRequired()
            => rule.RuleBuilder.NotEmpty().WithRequiredLocalizedMessage(rule.PropertyDisplayKey);

        /// <summary>
        /// Adds a "maximum length" validation rule with a localized message indicating the maximum allowed length for the field.
        /// </summary>
        /// <param name="maxLength">The maximum allowed length of the field.</param>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, string> MaximumLength(int maxLength)
            => rule.RuleBuilder.MaximumLength(maxLength).WithMaxLengthLocalizedMessage(maxLength, rule.PropertyDisplayKey);

        /// <summary>
        /// Adds an "email address" validation rule with a localized message indicating that the field must be a valid email address.
        /// </summary>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, string> EmailAddress()
            => rule.RuleBuilder.EmailAddress().WithValidEmailLocalizedMessage(rule.PropertyDisplayKey);
    }
}

/// <summary>
/// Localized validation rules for collection properties.
/// </summary>
public static class LocalizedCollectionRuleBuilderExtensions
{
    extension<T, TElement>(LocalizedRuleBuilder<T, IEnumerable<TElement>> rule)
    {
        /// <summary>
        /// Adds a "not empty" validation rule for collections with a localized message indicating that the collection is required and cannot be empty.
        /// </summary>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, IEnumerable<TElement>> NotEmptyRequired()
            => rule.RuleBuilder.NotEmpty().WithNotEmptyCollectionLocalizedMessage(rule.PropertyDisplayKey);
    }
}

/// <summary>
/// Localized validation rules for comparable properties.
/// </summary>
public static class LocalizedComparableRuleBuilderExtensions
{
    extension<T, TComparable>(LocalizedRuleBuilder<T, TComparable> rule)
        where TComparable : IComparable<TComparable>, IComparable
    {
        /// <summary>
        /// Adds an "inclusive between" validation rule with a localized message indicating that the field must be between the specified minimum and maximum values, inclusive.
        /// </summary>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, TComparable> InclusiveBetween(TComparable min, TComparable max)
            => rule.RuleBuilder.InclusiveBetween(min, max).WithBetweenLocalizedMessage(min, max, rule.PropertyDisplayKey);

        /// <summary>
        /// Adds a "less than or equal to" validation rule with a localized message indicating that the field must be less than or equal to the specified maximum value.
        /// </summary>
        /// <param name="maximum">The maximum allowed value.</param>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, TComparable> LessThanOrEqualTo(TComparable maximum)
            => rule.RuleBuilder.LessThanOrEqualTo(maximum).WithLessThanOrEqualLocalizedMessage(maximum, rule.PropertyDisplayKey);

        /// <summary>
        /// Adds a "greater than or equal to" validation rule with a localized message indicating that the field must be greater than or equal to the specified minimum value.
        /// </summary>
        /// <param name="minimum">The minimum allowed value.</param>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, TComparable> GreaterThanOrEqualTo(TComparable minimum)
            => rule.RuleBuilder.GreaterThanOrEqualTo(minimum).WithGreaterThanOrEqualLocalizedMessage(minimum, rule.PropertyDisplayKey);
    }
}

/// <summary>
/// Localized validation rules for date properties.
/// </summary>
public static class LocalizedDateRuleBuilderExtensions
{
    extension<T>(LocalizedRuleBuilder<T, DateTime> rule)
    {
        /// <summary>
        /// Adds a "less than" validation rule with a localized message indicating that the date must be in the past, i.e., less than the current date and time.
        /// </summary>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, DateTime> InPast()
            => rule.RuleBuilder.LessThan(DateTime.Now).WithInPastLocalizedMessage(rule.PropertyDisplayKey);
    }

    extension<T>(LocalizedRuleBuilder<T, DateTimeOffset> rule)
    {
        /// <summary>
        /// Adds a "less than" validation rule with a localized message indicating that the date must be in the past, i.e., less than the current date and time with offset.
        /// </summary>
        /// <returns>The rule builder options.</returns>
        public IRuleBuilderOptions<T, DateTimeOffset> InPast()
            => rule.RuleBuilder.LessThan(DateTimeOffset.Now).WithInPastLocalizedMessage(rule.PropertyDisplayKey);
    }
}
