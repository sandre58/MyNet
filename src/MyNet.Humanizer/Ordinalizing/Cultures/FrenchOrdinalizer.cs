// -----------------------------------------------------------------------
// <copyright file="FrenchOrdinalizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Culture;
using MyNet.Text;

namespace MyNet.Humanizer.Ordinalizing.Cultures;

/// <summary>
/// Provides functionality to convert numbers into their ordinal string representations in French, taking into account grammatical gender for the first ordinal form. In French, the ordinal form of "1" changes based on the grammatical gender of the noun it modifies: "1er" for masculine and "1re" for feminine. For all other numbers, the ordinal form is created by appending "ème" to the cardinal number. This class implements the IOrdinalizer interface, allowing it to be used in a consistent way with other ordinalizers for different languages and cultures within the MyNet.Humanizer library.
/// </summary>
public sealed class FrenchOrdinalizer : OrdinalizerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FrenchOrdinalizer"/> class.
    /// </summary>
    internal FrenchOrdinalizer()
        : base(SupportedCultures.French)
    {
    }

    /// <inheritdoc/>
    public override string Ordinalize(long number, OrdinalizationOptions? options = null)
    {
        var gender = options?.Gender ?? GrammaticalGender.Masculine;

        return number switch
        {
            1 when gender == GrammaticalGender.Feminine => "1re",
            1 => "1er",
            _ => $"{number}ème"
        };
    }
}
