// -----------------------------------------------------------------------
// <copyright file="ITranslationCatalogContribution.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Globalization.Localization.Translation.Catalog;

/// <summary>
/// Represents a startup-time contribution that can register resources into a translation catalog.
/// </summary>
public interface ITranslationCatalogContribution
{
    /// <summary>
    /// Gets the contribution priority.
    /// Higher priority contributions override lower priority ones.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Applies the contribution to the provided translation resource registry.
    /// </summary>
    /// <param name="registry">The translation resource registry to update.</param>
    void Apply(ITranslationResourceRegistry registry);
}
