// -----------------------------------------------------------------------
// <copyright file="Inflectors.French.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Culture;

namespace MyNet.Globalization.Inflection.Cultures;

public static partial class Inflectors
{
    /// <summary>
    /// Gets an inflector for the French language, which applies the pluralization and singularization rules specific to French. This inflector can be used to convert singular nouns to their plural forms and vice versa, following the linguistic conventions of the French language, including irregular forms and exceptions.
    /// </summary>
    public static IInflector French { get; } = Inflector.Create(SupportedCultures.French)
        .AddPluralRule("$", "s")
        .AddPluralRule("s$", "s")
        .AddPluralRule("x$", "x")
        .AddPluralRule("z$", "z")
        .AddPluralRule("(eau|au|eu)$", "$1x")
        .AddPluralRule("al$", "aux")
        .AddSingularRule("s$", string.Empty)
        .AddSingularRule("aux$", "al")
        .AddSingularRule("(eau|eu)x$", "$1")
        .AddIrregular("boyau", "boyaux")
        .AddIrregular("joyau", "joyaux")
        .AddIrregular("tuyau", "tuyaux")
        .AddIrregular("noyau", "noyaux")
        .AddIrregular("cabillau", "cabillaux")
        .AddIrregular("chaux", "chaux")
        .AddIrregular("faux", "faux")
        .AddIrregular("oeil", "yeux")
        .AddIrregular("pneu", "pneus")
        .AddIrregular("bleu", "bleus")
        .AddIrregular("émeu", "émeus")
        .AddIrregular("carnaval", "carnavals")
        .AddIrregular("caracal", "caracals")
        .AddIrregular("chacal", "chacals")
        .AddIrregular("choral", "chorals")
        .AddIrregular("corral", "corrals")
        .AddIrregular("étal", "étals")
        .AddIrregular("festival", "festivals")
        .AddIrregular("récital ", "récitals")
        .AddIrregular("val", "vals")
        .AddIrregular("aspirail", "aspiraux")
        .AddIrregular("corail", "coraux")
        .AddIrregular("émail", "émaux")
        .AddIrregular("fermail", "fermaux")
        .AddIrregular("soupirail", "soupiraux")
        .AddIrregular("travail", "travaux")
        .AddIrregular("vantail", "vantaux")
        .AddIrregular("vitrail", "vitraux")
        .AddIrregular("bail", "baux")
        .AddIrregular("bijou", "bijoux")
        .AddIrregular("caillou", "cailloux")
        .AddIrregular("chou", "choux")
        .AddIrregular("genou", "genoux")
        .AddIrregular("hibou", "hiboux")
        .AddIrregular("joujou", "joujoux")
        .AddIrregular("pou", "poux")
        .Build();
}
