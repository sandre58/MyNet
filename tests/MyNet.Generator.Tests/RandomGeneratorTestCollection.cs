// -----------------------------------------------------------------------
// <copyright file="RandomGeneratorTestCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyNet.Generator.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "RandomGeneratorTestCollection is a xUnit convention and must be public and non-generic.")]
[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "RandomGeneratorTestCollection is a xUnit convention and must be public.")]
public sealed class RandomGeneratorTestCollection
{
    internal const string Name = "RandomGenerator";
}
