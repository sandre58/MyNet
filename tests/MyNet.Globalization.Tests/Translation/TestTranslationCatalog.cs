// -----------------------------------------------------------------------
// <copyright file="TestTranslationCatalog.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Resources;
using MyNet.Globalization.Localization.Translation.Catalog;

namespace MyNet.Globalization.Tests.Translation;

internal sealed class TestTranslationCatalog : ITranslationCatalog, ITranslationResourceRegistry
{
    private readonly ConcurrentDictionary<string, ResourceManager> _resources = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, ResourceManager> Resources => _resources;

    public void Register(string resourceKey, ResourceManager resourceManager)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);
        ArgumentNullException.ThrowIfNull(resourceManager);

        _resources[resourceKey] = resourceManager;
    }

    public bool TryGetResource(string resourceKey, out ResourceManager resourceManager)
        => _resources.TryGetValue(resourceKey, out resourceManager!);
}
