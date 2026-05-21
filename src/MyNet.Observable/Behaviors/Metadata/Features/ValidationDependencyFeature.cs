// -----------------------------------------------------------------------
// <copyright file="ValidationDependencyFeature.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Observable.Behaviors.Metadata.Features;

/// <summary>
/// Represents a feature that defines validation dependencies for an observable object. This feature allows you to specify which properties of an observable object are dependent on the validation state of other properties. The Dependents property is a hash set that contains the names of the properties that are dependent on the validation state of other properties. By using this feature, you can establish relationships between properties in terms of their validation requirements, enabling more complex validation scenarios where the validity of one property may affect the validity of others in an observable object. This can be particularly useful in scenarios where certain properties must be validated together or where the validation state of one property influences the validation state of others in an application.
/// </summary>
public sealed class ValidationDependencyFeature
{
    /// <summary>
    /// Gets the set of property names that are dependent on the validation state of other properties. This hash set allows you to specify which properties in an observable object are dependent on the validation state of other properties. By adding property names to this set, you can establish relationships between properties in terms of their validation requirements, enabling more complex validation scenarios where the validity of one property may affect the validity of others in an observable object. This can be particularly useful in scenarios where certain properties must be validated together or where the validation state of one property influences the validation state of others in an application.
    /// </summary>
    public HashSet<string> Dependents { get; } = new(StringComparer.Ordinal);
}
