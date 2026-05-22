// -----------------------------------------------------------------------
// <copyright file="PropertyChangedForwardingFeature.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors.Metadata.Features;

/// <summary>
/// Marks a property as a wrapper whose child <see cref="System.ComponentModel.INotifyPropertyChanged"/>
/// notifications should be relayed to the owner via <see cref="PropertyChangedForwardingBehavior"/>.
/// </summary>
public sealed class PropertyChangedForwardingFeature
{
    /// <summary>
    /// Gets or sets a value indicating whether relayed property names are prefixed with the wrapper property name.
    /// </summary>
    public bool ConcatenatePropertyName { get; set; } = true;
}
