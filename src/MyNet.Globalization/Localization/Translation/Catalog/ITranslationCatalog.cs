// -----------------------------------------------------------------------
// <copyright file="ITranslationCatalog.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Resources;

namespace MyNet.Globalization.Localization.Translation.Catalog;

/// <summary>
/// Defines the contract for a translation catalog that provides access to registered translation resources.
/// The catalog allows retrieving all registered <see cref="ResourceManager"/> instances, keyed by a unique resource key.
/// </summary>
public interface ITranslationCatalog
{
    /// <summary>
    /// Gets all registered translation resources.
    /// </summary>
    IReadOnlyDictionary<string, ResourceManager> Resources { get; }

    /// <summary>
    /// Attempts to retrieve a resource manager by its key.
    /// </summary>
    /// <param name="resourceKey">The unique resource key.</param>
    /// <param name="resourceManager">
    /// When this method returns, contains the matching resource manager if found.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the resource exists; otherwise <see langword="false"/>.
    /// </returns>
    bool TryGetResource(string resourceKey, out ResourceManager resourceManager);
}
