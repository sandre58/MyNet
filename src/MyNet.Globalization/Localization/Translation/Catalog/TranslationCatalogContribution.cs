// -----------------------------------------------------------------------
// <copyright file="TranslationCatalogContribution.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Globalization.Localization.Translation.Catalog;

/// <summary>
/// Delegate-based translation catalog contribution.
/// </summary>
public sealed class TranslationCatalogContribution(Action<ITranslationResourceRegistry> apply, int priority = 0) : ITranslationCatalogContribution
{
    /// <inheritdoc />
    public int Priority { get; } = priority;

    /// <inheritdoc />
    public void Apply(ITranslationResourceRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        apply(registry);
    }
}
