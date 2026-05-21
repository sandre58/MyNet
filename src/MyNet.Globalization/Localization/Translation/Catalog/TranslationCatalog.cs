// -----------------------------------------------------------------------
// <copyright file="TranslationCatalog.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Resources;

namespace MyNet.Globalization.Localization.Translation.Catalog;

/// <summary>
/// Immutable implementation of <see cref="ITranslationCatalog"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TranslationCatalog"/> class.
/// </remarks>
/// <param name="resources">The registered translation resources.</param>
internal sealed class TranslationCatalog(ImmutableDictionary<string, ResourceManager> resources) : ITranslationCatalog
{
    private readonly FrozenDictionary<string, ResourceManager> _resources = resources.ToFrozenDictionary();

    /// <inheritdoc />
    public IReadOnlyDictionary<string, ResourceManager> Resources => _resources;

    /// <inheritdoc />
    public bool TryGetResource(string resourceKey, out ResourceManager resourceManager) => _resources.TryGetValue(resourceKey, out resourceManager!);
}
