// -----------------------------------------------------------------------
// <copyright file="PatternFakerExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Generator;
using MyNet.Text.Randomize;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Fakers;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class PatternFakerExtensions
{
    extension(ITextRandomGenerator random)
    {
        /// <summary>
        /// Generates a random string based on a randomly selected pattern from the provided list, using the specified text random generator to replace placeholders in the pattern.
        /// </summary>
        /// <param name="generator">The random generator used to select a pattern from the list.</param>
        /// <param name="patterns">The list of patterns to choose from.</param>
        /// <returns>A randomly generated string based on the selected pattern.</returns>
        public string RandomizeWithRandomPattern(IRandomGenerator generator, IReadOnlyList<string> patterns)
        {
            var pattern = generator.Item(patterns);

            return random.Randomize(pattern);
        }
    }
}
