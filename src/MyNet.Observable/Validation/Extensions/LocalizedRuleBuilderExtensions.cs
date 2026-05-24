// -----------------------------------------------------------------------
// <copyright file="LocalizedRuleBuilderExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using FluentValidation;
using MyNet.Observable.Localization;
using MyNet.Observable.Validation;
using MyNet.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// FluentValidation extensions that apply <see cref="ValidationResources"/> messages.
/// </summary>
public static class LocalizedRuleBuilderExtensions
{
    extension<T, TProperty>(IRuleBuilderOptions<T, TProperty> rule)
    {
        /// <summary>
        /// Sets the error message for the rule to a localized "field is required" message, using the provided property display key for translation.
        /// </summary>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithRequiredLocalizedMessage(string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.FieldIsRequired, propertyDisplayKey);

        /// <summary>
        /// Sets the error message for the rule to a localized "field exceeds maximum length" message, using the provided property display key and maximum length for translation.
        /// </summary>
        /// <param name="maxLength">The maximum length allowed for the property.</param>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithMaxLengthLocalizedMessage(int maxLength, string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.FieldExceedsMaxLength, propertyDisplayKey, maxLength);

        /// <summary>
        /// Sets the error message for the rule to a localized "field is not a valid email address" message, using the provided property display key for translation.
        /// </summary>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithValidEmailLocalizedMessage(string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.FieldInvalidEmail, propertyDisplayKey);

        /// <summary>
        /// Sets the error message for the rule to a localized "field is not a valid phone number" message, using the provided property display key for translation.
        /// </summary>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithValidPhoneNumberLocalizedMessage(string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.FieldInvalidPhoneNumber, propertyDisplayKey);

        /// <summary>
        /// Sets the error message for the rule to a localized "field must be between" message, using the provided property display key and range values for translation.
        /// </summary>
        /// <param name="min">The minimum value allowed for the property.</param>
        /// <param name="max">The maximum value allowed for the property.</param>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithBetweenLocalizedMessage<TComparable>(
            TComparable min,
            TComparable max,
            string propertyDisplayKey)
            where TComparable : IComparable<TComparable>, IComparable
            => rule.WithLocalizedMessage(ValidationResources.FieldMustBeBetween, propertyDisplayKey, min, max);

        /// <summary>
        /// Sets the error message for the rule to a localized "field must be less than or equal to" message, using the provided property display key and maximum value for translation.
        /// </summary>
        /// <param name="maximum">The maximum value allowed for the property.</param>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithLessThanOrEqualLocalizedMessage<TComparable>(
            TComparable maximum,
            string propertyDisplayKey)
            where TComparable : IComparable<TComparable>, IComparable
            => rule.WithLocalizedMessage(ValidationResources.FieldMustBeLessThanOrEqualTo, propertyDisplayKey, maximum);

        /// <summary>
        /// Sets the error message for the rule to a localized "field must be greater than or equal to" message, using the provided property display key and minimum value for translation.
        /// </summary>
        /// <param name="minimum">The minimum value allowed for the property.</param>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithGreaterThanOrEqualLocalizedMessage<TComparable>(
            TComparable minimum,
            string propertyDisplayKey)
            where TComparable : IComparable<TComparable>, IComparable
            => rule.WithLocalizedMessage(ValidationResources.FieldMustBeGreaterThanOrEqualTo, propertyDisplayKey, minimum);

        /// <summary>
        /// Sets the error message for the rule to a localized "field must be less than" message, using the provided property display key and maximum value for translation.
        /// </summary>
        /// <param name="maximum">The maximum value allowed for the property.</param>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithLessThanLocalizedMessage<TComparable>(
            TComparable maximum,
            string propertyDisplayKey)
            where TComparable : IComparable<TComparable>, IComparable
            => rule.WithLocalizedMessage(ValidationResources.FieldMustBeLessThan, propertyDisplayKey, maximum);

        /// <summary>
        /// Sets the error message for the rule to a localized "field must be greater than" message, using the provided property display key and minimum value for translation.
        /// </summary>
        /// <param name="minimum">The minimum value allowed for the property.</param>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithGreaterThanLocalizedMessage<TComparable>(
            TComparable minimum,
            string propertyDisplayKey)
            where TComparable : IComparable<TComparable>, IComparable
            => rule.WithLocalizedMessage(ValidationResources.FieldMustBeGreaterThan, propertyDisplayKey, minimum);

        /// <summary>
        /// Sets the error message for the rule to a localized "field must be in the past" message, using the provided property display key for translation.
        /// </summary>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithInPastLocalizedMessage(string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.FieldMustBeInPast, propertyDisplayKey);

        /// <summary>
        /// Sets the error message for the rule to a localized "collection requires at least one item" message, using the provided property display key for translation.
        /// </summary>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithNotEmptyCollectionLocalizedMessage(string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.CollectionRequiresAtLeastOneItem, propertyDisplayKey);

        /// <summary>
        /// Sets the error message for the rule to a localized "collection contains duplicate items" message, using the provided property display key for translation.
        /// </summary>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithUniqueItemsLocalizedMessage(string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.CollectionDuplicateItems, propertyDisplayKey);

        /// <summary>
        /// Sets the error message for the rule to a localized "invalid file path" message, using the provided property display key for translation.
        /// </summary>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithValidFilePathLocalizedMessage(string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.FieldInvalidFilePath, propertyDisplayKey);

        /// <summary>
        /// Sets the error message for the rule to a localized "file not found" message, using the provided property display key for translation.
        /// </summary>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithExistingFileLocalizedMessage(string propertyDisplayKey)
            => rule.WithLocalizedMessage(ValidationResources.FieldFileNotFound, propertyDisplayKey);

        /// <summary>
        /// Sets the error message for the rule to a localized "folder not found" message, using the provided path selector for translation.
        /// </summary>
        /// <param name="pathSelector">A function that selects the path to be validated.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithExistingFolderLocalizedMessage(Func<T, string?> pathSelector)
            => rule.WithMessage(instance =>
                ValidationResources.PathPartNotFound.FormatWith(
                    CultureInfo.CurrentCulture,
                    pathSelector(instance) ?? string.Empty));

        /// <summary>
        /// Sets the error message for the rule to a localized message, using the provided property display key and additional arguments for translation.
        /// </summary>
        /// <param name="template">The template for the localized message.</param>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <param name="additionalArgs">Additional arguments for the message template.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithLocalizedMessage(
            string template,
            string propertyDisplayKey,
            params object?[] additionalArgs)
            => rule.WithMessage(_ => ValidationMessages.FormatForPropertyKey(template, propertyDisplayKey, additionalArgs));

        /// <summary>
        /// Sets the error message for the rule to a localized message, using the provided property display key and a function to generate additional arguments for translation.
        /// </summary>
        /// <param name="template">The template for the localized message.</param>
        /// <param name="propertyDisplayKey">The key used to look up the localized display name of the property.</param>
        /// <param name="additionalArgs">A function that generates additional arguments for the message template.</param>
        /// <returns>The rule builder with the localized error message applied.</returns>
        public IRuleBuilderOptions<T, TProperty> WithLocalizedMessage(
            string template,
            string propertyDisplayKey,
            Func<T, object?[]> additionalArgs)
            => rule.WithMessage(instance =>
                ValidationMessages.FormatForPropertyKey(template, propertyDisplayKey, additionalArgs(instance)));
    }
}
