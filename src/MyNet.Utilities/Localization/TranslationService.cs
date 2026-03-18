// -----------------------------------------------------------------------
// <copyright file="TranslationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;

namespace MyNet.Utilities.Localization;

public sealed class TranslationService
{
    private static readonly Dictionary<CultureInfo, TranslationService> RegisteredServices = [];
    private static readonly Dictionary<string, ResourceManager> Resources = [];

    private static readonly Lock LockCulture = new();
    private static readonly Lock LockResources = new();

    private TranslationService(CultureInfo culture) => Culture = culture;

    public static TranslationService Current => Get(CultureInfo.CurrentCulture);

    public CultureInfo Culture { get; }

    public string this[string key] => Translate(key);

    public string this[string key, string filename] => Translate(key, filename);

    public static TranslationService GetOrCurrent(CultureInfo? cultureInfo = null) =>
        cultureInfo is not null ? Get(cultureInfo) : Current;

    public static TranslationService Get(CultureInfo culture)
    {
        lock (LockCulture)
        {
            if (!RegisteredServices.ContainsKey(culture))
            {
                RegisteredServices.Add(culture, new TranslationService(culture));
            }
        }

        lock (LockCulture)
        {
            return RegisteredServices[culture];
        }
    }

    public static void RegisterResources(string resourceKey, ResourceManager resourceManager)
    {
        lock (LockResources)
        {
            _ = Resources.TryAdd(resourceKey, resourceManager);
        }
    }

    public string Translate(string key)
    {
        lock (LockResources)
        {
            var result = Resources.Select(r => r.Value.GetString(key, Culture)).NotNull().LastOrDefault();
            return !string.IsNullOrEmpty(result) ? result : key;
        }
    }

    public string Translate(string key, string filename)
    {
        lock (LockResources)
        {
            return Resources.TryGetValue(filename, out var value) && value.GetString(key, Culture) is { } result
                ? result
                : key;
        }
    }
}
