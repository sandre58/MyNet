// -----------------------------------------------------------------------
// <copyright file="ResxProviderBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Resources;
using MyNet.Fakers.Localization;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Fakers;

/// <summary>
/// Base class for RESX-based pattern providers. This class provides common functionality for loading localized datasets from RESX resources, including culture management and resource access. Derived classes can utilize the provided <see cref="LoadDataset"/> method to retrieve localized string lists based on specific keys, simplifying the implementation of culture-aware faker providers that rely on RESX resources for their datasets.
/// </summary>
/// <param name="culture">The culture associated with this provider, used for loading localized datasets from RESX resources.</param>
/// <param name="resourceManager">The resource manager used to load localized datasets from RESX resources.</param>
public abstract class ResxProviderBase(CultureInfo culture, ResourceManager resourceManager) : ICultureScoped
{
    /// <summary>
    /// Gets the culture associated with this provider, used for loading localized datasets from RESX resources.
    /// </summary>
    public CultureInfo Culture { get; } = culture ?? throw new ArgumentNullException(nameof(culture));

    /// <summary>
    /// Gets the resource manager used to load localized datasets from RESX resources. This is typically initialized in the constructor of derived classes with the appropriate resource manager for the specific dataset type.
    /// </summary>
    protected ResourceManager ResourceManager { get; } = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

    /// <summary>
    /// Loads a localized string list from a RESX resource using the provided key. This method utilizes the <see cref="ResourceDatasetLoader"/> to retrieve and cache the dataset based on the current culture and resource manager. Derived classes can call this method with specific keys to load their respective datasets from the associated RESX resources, ensuring that the data is properly localized according to the provider's culture.
    /// </summary>
    /// <param name="key">The key of the resource to load.</param>
    /// <returns>A read-only list of localized strings.</returns>
    protected ImmutableArray<string> LoadDataset(string key) => ResourceDatasetLoader.LoadList(ResourceManager, key, Culture);
}
