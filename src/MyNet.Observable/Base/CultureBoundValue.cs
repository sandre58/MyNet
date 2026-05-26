// -----------------------------------------------------------------------
// <copyright file="CultureBoundValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Facade;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Observable.Metadata;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Observable value recomputed when the application culture changes, with per-culture caching.
/// </summary>
[ExemptFromGeneratedMetadata]
public class CultureBoundValue<T> : ObservableObject, IObservableValue<T>, IEventAware<CultureChangedEvent>
{
    private readonly Func<T?> _valueFactory;
    private readonly ICultureService _cultureService;
    private CultureInfo? _cachedCulture;
    private T? _cachedValue;
    private bool _hasCachedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureBoundValue{T}"/> class that uses <see cref="GlobalizationServices.Current"/> for culture changes.
    /// </summary>
    /// <param name="valueFactory">Factory invoked when the value must be computed for the current culture.</param>
    public CultureBoundValue(Func<T?> valueFactory)
        : this(valueFactory, GlobalizationServices.Current) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureBoundValue{T}"/> class with an explicit culture service.
    /// </summary>
    /// <param name="valueFactory">Factory invoked when the value must be computed for the current culture.</param>
    /// <param name="cultureService">Culture service used to detect culture changes.</param>
    public CultureBoundValue(Func<T?> valueFactory, ICultureService cultureService)
    {
        _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        _cultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));
        this.ReactOnCultureChanged(cultureService);
    }

    /// <inheritdoc />
    public virtual T? Value => GetOrCompute();

    /// <inheritdoc />
    public virtual void OnEvent(CultureChangedEvent e)
    {
        _hasCachedValue = false;
        NotifyPropertyChanged(nameof(Value));
    }

    /// <inheritdoc />
    public override string? ToString() => Value?.ToString();

    private T? GetOrCompute()
    {
        var culture = _cultureService.CurrentCulture;

        if (_hasCachedValue && Equals(_cachedCulture, culture))
            return _cachedValue;

        _cachedCulture = culture;
        _cachedValue = _valueFactory();
        _hasCachedValue = true;
        return _cachedValue;
    }
}
