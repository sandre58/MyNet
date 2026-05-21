// -----------------------------------------------------------------------
// <copyright file="TranslationKeyProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities;

namespace MyNet.Globalization.Localization.Translation.KeyGeneration;

/// <summary>
/// Provides a default implementation of the <see cref="ITranslationKeyProvider"/> interface for generating resource keys based on the type and value of the input object.
/// </summary>
public sealed class TranslationKeyProvider : ITranslationKeyProvider
{
    /// <inheritdoc/>
    public string GetKey(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value switch
        {
            Enum enumValue => value.GetType().Name + enumValue,
            ISmartEnum smartEnumValue => value.GetType().Name + smartEnumValue.Name,
            _ => value.GetType().Name + value
        };
    }

    /// <inheritdoc/>
    public string GetKey<T>(T value) => GetKey((object)value!);
}
