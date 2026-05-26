// -----------------------------------------------------------------------
// <copyright file="CountryFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Generator;
using MyNet.Geography;

namespace MyNet.Fakers.Geography;

/// <summary>
/// Current implementation of <see cref="ICountryFaker"/>.
/// </summary>
/// <param name="random">The random generator to use.</param>
public sealed class CountryFaker(IRandomGenerator random) : ICountryFaker
{
    /// <inheritdoc />
    public Country Country() => random.Item(global::MyNet.Geography.Country.All);

    /// <inheritdoc />
    public string Name() => random.Item(global::MyNet.Geography.Country.All).Name;

    /// <inheritdoc />
    public string Alpha2() => random.Item(global::MyNet.Geography.Country.All).Alpha2;

    /// <inheritdoc />
    public string Alpha3() => random.Item(global::MyNet.Geography.Country.All).Alpha3;

    /// <inheritdoc />
    public int Iso() => random.Item(global::MyNet.Geography.Country.All).Iso;
}
