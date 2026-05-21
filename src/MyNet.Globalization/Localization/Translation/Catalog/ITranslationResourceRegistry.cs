// -----------------------------------------------------------------------
// <copyright file="ITranslationResourceRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Resources;

namespace MyNet.Globalization.Localization.Translation.Catalog;

/// <summary>
/// Defines the contract for a registry that allows registering translation resources (i.e., <see cref="ResourceManager"/> instances) under specific keys.
/// </summary>
public interface ITranslationResourceRegistry
{
    /// <summary>
    /// Registers a translation resource manager.
    /// If the key already exists, the previous value is replaced.
    /// </summary>
    /// <param name="resourceKey">The unique resource key.</param>
    /// <param name="resourceManager">The resource manager instance.</param>
    void Register(string resourceKey, ResourceManager resourceManager);
}
