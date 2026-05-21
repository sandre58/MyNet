// -----------------------------------------------------------------------
// <copyright file="TranslationCatalogBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Resources;

namespace MyNet.Globalization.Localization.Translation.Catalog;

/// <summary>
/// Builder for constructing an immutable translation catalog by registering translation resources.
/// </summary>
internal sealed class TranslationCatalogBuilder : ITranslationResourceRegistry
{
    private readonly Dictionary<string, ResourceManager> _resources = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public void Register(string resourceKey, ResourceManager resourceManager)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);
        ArgumentNullException.ThrowIfNull(resourceManager);

        _resources[resourceKey] = resourceManager;
    }

    /// <summary>
    /// Builds the immutable translation catalog.
    /// </summary>
    /// <returns>The built catalog.</returns>
    public ITranslationCatalog Build() => new TranslationCatalog(_resources.ToImmutableDictionary(StringComparer.Ordinal));
}
