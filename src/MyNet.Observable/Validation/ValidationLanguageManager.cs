// -----------------------------------------------------------------------
// <copyright file="ValidationLanguageManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using FluentValidation.Resources;
using MyNet.Globalization.Culture;
using MyNet.Observable.Localization;

namespace MyNet.Observable.Validation;

/// <summary>
/// FluentValidation <see cref="LanguageManager"/> backed by <see cref="ValidationResources"/>.
/// </summary>
public sealed class ValidationLanguageManager : LanguageManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationLanguageManager"/> class.
    /// </summary>
    public ValidationLanguageManager()
    {
        RegisterCulture(SupportedCultures.English.TwoLetterISOLanguageName, SupportedCultures.English);
        RegisterCulture(SupportedCultures.French.TwoLetterISOLanguageName, SupportedCultures.French);
    }

    /// <summary>
    /// Gets the localized string for the specified resource name and culture.
    /// </summary>
    /// <param name="resourceName">The name of the resource.</param>
    /// <param name="culture">The culture for which to retrieve the resource.</param>
    /// <returns>The localized string.</returns>
    private static new string GetString(string resourceName, CultureInfo culture) => ValidationResources.ResourceManager.GetString(resourceName, culture) ?? string.Empty;

    /// <summary>
    /// Adapts a resource template by replacing the standard FluentValidation placeholders with the ones expected by the validation framework, and applying any additional placeholder replacements provided.
    /// </summary>
    /// <param name="resourceTemplate">The resource template to adapt.</param>
    /// <param name="placeholderReplacements">An array of placeholder replacements, where each pair of elements represents a placeholder and its replacement.</param>
    /// <returns>The adapted resource template.</returns>
    private static string AdaptTemplate(string resourceTemplate, params string[] placeholderReplacements)
    {
        var message = resourceTemplate.Replace("{0}", "{PropertyName}", StringComparison.Ordinal);

        for (var i = 0; i < placeholderReplacements.Length; i += 2)
        {
            message = message.Replace(placeholderReplacements[i], placeholderReplacements[i + 1], StringComparison.Ordinal);
        }

        return message;
    }

    /// <summary>
    /// Registers the validation messages for a specific language code and resource culture. This method maps FluentValidation validator keys to the corresponding localized messages from the resource, adapting the placeholders as needed.
    /// </summary>
    /// <param name="languageCode">The language code for which to register the validation messages.</param>
    /// <param name="resourceCulture">The culture of the resource from which to retrieve the validation messages.</param>
    private void RegisterCulture(string languageCode, CultureInfo resourceCulture)
    {
        Register(languageCode, "NotNullValidator", GetString(nameof(ValidationResources.FieldIsRequired), resourceCulture));
        Register(languageCode, "NotEmptyValidator", GetString(nameof(ValidationResources.FieldIsRequired), resourceCulture));
        Register(languageCode, "EmailValidator", GetString(nameof(ValidationResources.FieldInvalidEmail), resourceCulture));
        Register(languageCode, "MaximumLengthValidator", GetString(nameof(ValidationResources.FieldExceedsMaxLength), resourceCulture), "{1}", "{MaxLength}");
        Register(languageCode, "MaximumLength_Simple", GetString(nameof(ValidationResources.FieldExceedsMaxLength), resourceCulture), "{1}", "{MaxLength}");
        Register(languageCode, "InclusiveBetweenValidator", GetString(nameof(ValidationResources.FieldMustBeBetween), resourceCulture), "{1}", "{From}", "{2}", "{To}");
        Register(languageCode, "InclusiveBetween_Simple", GetString(nameof(ValidationResources.FieldMustBeBetween), resourceCulture), "{1}", "{From}", "{2}", "{To}");
        Register(languageCode, "GreaterThanOrEqualValidator", GetString(nameof(ValidationResources.FieldMustBeGreaterThanOrEqualTo), resourceCulture));
        Register(languageCode, "LessThanOrEqualValidator", GetString(nameof(ValidationResources.FieldMustBeLessThanOrEqualTo), resourceCulture));
        Register(languageCode, "GreaterThanValidator", GetString(nameof(ValidationResources.FieldMustBeGreaterThan), resourceCulture));
        Register(languageCode, "LessThanValidator", GetString(nameof(ValidationResources.FieldMustBeLessThan), resourceCulture));
    }

    /// <summary>
    /// Registers a validation message for a specific language code and validator key, using the provided resource template and placeholder replacements. This method adapts the resource template to match the expected format of the validation framework and adds the resulting message to the language manager's translations.
    /// </summary>
    /// <param name="languageCode">The language code for which to register the validation message.</param>
    /// <param name="validatorKey">The key of the validator for which to register the message.</param>
    /// <param name="resourceTemplate">The resource template to use for the validation message.</param>
    /// <param name="placeholderReplacements">An array of placeholder replacements, where each pair of elements represents a placeholder and its replacement.</param>
    private void Register(string languageCode, string validatorKey, string resourceTemplate, params string[] placeholderReplacements)
    {
        var message = AdaptTemplate(resourceTemplate, placeholderReplacements);
        AddTranslation(languageCode, validatorKey, message);
    }
}
