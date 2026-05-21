// -----------------------------------------------------------------------
// <copyright file="ConversionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using MyNet.Utilities.Conversion;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ConversionExtensions
{
    extension(double value)
    {
        /// <summary>
        /// Converts the value from one unit to another using the registered unit converters.
        /// </summary>
        /// <param name="from">The unit of the input value.</param>
        /// <param name="to">The unit to convert the value to.</param>
        /// <typeparam name="TUnit">The type of the unit.</typeparam>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double To<TUnit>(TUnit from, TUnit to)
            where TUnit : struct, Enum => UnitConverterRegistry.Get<TUnit>().Convert(value, from, to);

        /// <summary>
        /// Converts the value from its current unit to another unit using the registered unit converters. The current unit is determined by the type of the input value.
        /// </summary>
        /// <param name="to">The unit to convert the value to.</param>
        /// <typeparam name="TUnit">The type of the unit.</typeparam>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double To<TUnit>(TUnit to)
            where TUnit : struct, Enum => value.To(default, to);

        /// <summary>
        /// Converts the value from one unit to another using the registered unit converters. The type of the unit is determined by the provided type parameter.
        /// </summary>
        /// <param name="type">The type of the unit.</param>
        /// <param name="from">The unit of the input value.</param>
        /// <param name="to">The unit to convert the value to.</param>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double To<TUnit>(Type type, TUnit from, TUnit to)
            where TUnit : struct, Enum => UnitConverterRegistry.Get<TUnit>(type).Convert(value, from, to);
    }
}
