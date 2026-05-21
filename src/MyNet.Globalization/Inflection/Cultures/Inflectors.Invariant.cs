// -----------------------------------------------------------------------
// <copyright file="Inflectors.Invariant.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Inflection.Cultures;

public static partial class Inflectors
{
    /// <summary>
    /// Gets an inflector that uses the invariant culture and has no specific inflection rules. This class can be used as a default or placeholder inflector when no specific culture or inflection rules are needed, effectively treating all words as uninflected and using the invariant culture for any culture-specific operations related to inflection.
    /// </summary>
    public static IInflector Invariant { get; } = Inflector.Create(CultureInfo.InvariantCulture).Build();
}
