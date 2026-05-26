// -----------------------------------------------------------------------
// <copyright file="UnitConverterRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Primitives.Conversion;

/// <summary>
/// Provides a registry for unit converters, allowing for the registration and retrieval of converters based on their associated unit type.
/// </summary>
public static class UnitConverterRegistry
{
    private static readonly Dictionary<Type, object> Converters = [];

    static UnitConverterRegistry()
    {
        Register(new DataSizeConverter());
        Register(new LengthConverter());
        Register(new MassConverter());
        Register(new TimeConverter());
        Register(new TemperatureConverter());
    }

    /// <summary>
    /// Registers a unit converter for a specific unit type. The converter is stored in the registry using the unit type as the key.
    /// </summary>
    /// <param name="converter">The unit converter to register.</param>
    /// <typeparam name="TUnit">The type of the unit.</typeparam>
    public static void Register<TUnit>(IUnitConverter<TUnit> converter)
        where TUnit : struct, Enum => Converters[typeof(TUnit)] = converter;

    /// <summary>
    /// Retrieves a registered unit converter for a specific unit type.
    /// </summary>
    /// <typeparam name="TUnit">The type of the unit.</typeparam>
    /// <returns>The registered unit converter.</returns>
    public static IUnitConverter<TUnit> Get<TUnit>()
        where TUnit : struct, Enum
        => Converters.TryGetValue(typeof(TUnit), out var converter)
            ? (IUnitConverter<TUnit>)converter
            : throw new InvalidOperationException($"No converter registered for unit type '{typeof(TUnit).Name}'.");

    /// <summary>
    /// Retrieves a registered unit converter for a specific unit type, using the provided type as the key.
    /// </summary>
    /// <param name="type">The type of the unit.</param>
    /// <returns>The registered unit converter.</returns>
    public static IUnitConverter<TUnit> Get<TUnit>(Type type)
        where TUnit : struct, Enum
        => Converters.TryGetValue(type, out var converter)
            ? (IUnitConverter<TUnit>)converter
            : throw new InvalidOperationException($"No converter registered for unit type '{type.Name}'.");
}
