// -----------------------------------------------------------------------
// <copyright file="PluralizationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Globalization.Inflection;

/// <summary>
/// Provides pluralization and singularization services based on the current culture or a specified culture. It uses an <see cref="IInflector"/> resolved from the <see cref="ILocalizationServiceResolver"/> to perform the actual inflection operations.
/// </summary>
/// <param name="resolver">The resolver used to obtain the appropriate <see cref="IInflector"/>.</param>
/// <param name="cultureContext">The context providing the current culture.</param>
public sealed class PluralizationService(ILocalizationServiceResolver resolver, ICultureContext cultureContext) : IPluralizationService
{
    /// <inheritdoc/>
    public string Pluralize(string word, CultureInfo? culture = null) => GetInflector(culture).Pluralize(word);

    /// <inheritdoc/>
    public string Singularize(string word, CultureInfo? culture = null) => GetInflector(culture).Singularize(word);

    /// <inheritdoc/>
    public bool IsPlural(decimal count, CultureInfo? culture = null) => GetInflector(culture).IsPlural(count);

    /// <inheritdoc/>
    public PluralCategory GetPluralCategory(decimal count, CultureInfo? culture = null) => GetInflector(culture).GetPluralCategory(count);

    /// <summary>
    /// Resolves the appropriate <see cref="IInflector"/> for the specified culture, or the current culture if none is provided.
    /// </summary>
    /// <param name="culture">The culture for which to resolve the inflector.</param>
    /// <returns>The resolved <see cref="IInflector"/>.</returns>
    private IInflector GetInflector(CultureInfo? culture) => resolver.ForCulture(culture ?? cultureContext.CurrentCulture).GetRequired<IInflector>();
}
