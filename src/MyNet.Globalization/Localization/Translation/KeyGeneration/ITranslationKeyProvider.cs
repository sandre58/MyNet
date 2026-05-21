// -----------------------------------------------------------------------
// <copyright file="ITranslationKeyProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Globalization.Localization.Translation.KeyGeneration;

/// <summary>
/// Defines a contract for generating resource keys based on input values. Implementations of this interface can provide custom logic to create unique and meaningful keys for translation resources, facilitating the retrieval of localized strings in a consistent manner.
/// </summary>
public interface ITranslationKeyProvider
{
    /// <summary>
    /// Generates a resource key based on the provided input value. The implementation of this method should create a unique and meaningful key that can be used to identify the corresponding translation resource in a localization system.
    /// </summary>
    /// <param name="value">The input value for which to generate a resource key.</param>
    /// <returns>The generated resource key.</returns>
    string GetKey(object value);

    /// <summary>
    /// Generates a resource key based on the provided input value of a specific type. The implementation of this method should create a unique and meaningful key that can be used to identify the corresponding translation resource in a localization system.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input value for which to generate a resource key.</param>
    /// <returns>The generated resource key.</returns>
    string GetKey<T>(T value);
}
