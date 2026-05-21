// -----------------------------------------------------------------------
// <copyright file="OrdinalizerBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Humanizer.Ordinalizing;

/// <summary>
/// Base class for ordinalizers, providing a default implementation that simply returns the number as a string. This class can be extended to implement culture-specific ordinalization rules by overriding the Ordinalize method. The base implementation serves as a fallback for cultures that do not have specific ordinalization rules defined, ensuring that the method always returns a valid string representation of the number.
/// </summary>
public abstract class OrdinalizerBase(CultureInfo culture) : IOrdinalizer
{
    /// <summary>
    /// Gets the culture associated with this ordinalizer, which can be used for culture-specific formatting and rules in derived classes. The base implementation simply returns the provided culture, but derived classes can utilize this property to implement specific ordinalization logic based on the cultural context. This allows for flexibility in handling different languages and cultures while still providing a default behavior for cases where no specific rules are defined.
    /// </summary>
    public CultureInfo Culture => culture;

    /// <inheritdoc/>
    public virtual string Ordinalize(long number, OrdinalizationOptions? options = null) => number.ToString(culture);
}
