// -----------------------------------------------------------------------
// <copyright file="OrdinalizationOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Text;

namespace MyNet.Humanizer.Ordinalizing;

public sealed class OrdinalizationOptions
{
    /// <summary>
    /// Gets or initializes the grammatical gender to use for ordinalization. This property is optional and can be set to null if the language does not require grammatical gender for ordinalization or if the caller does not wish to specify it. When set, the ordinalizer may use this information to determine the correct ordinal suffix or form based on the grammatical gender of the number being ordinalized, which can affect the output in languages that have different ordinal forms for masculine, feminine, and neuter.
    /// </summary>
    public GrammaticalGender? Gender { get; init; }
}
