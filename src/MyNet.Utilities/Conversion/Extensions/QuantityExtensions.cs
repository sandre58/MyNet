// -----------------------------------------------------------------------
// <copyright file="QuantityExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Conversion;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class QuantityExtensions
{
    extension(int value)
    {
        /// <summary>
        /// Creates a new quantity with the specified value and unit. This method allows you to easily create a quantity from an integer value by specifying the desired unit.
        /// </summary>
        /// <param name="unit">The unit of measurement for the quantity.</param>
        /// <typeparam name="TUnit">The type of the unit, which must be an enum.</typeparam>
        /// <returns>A new instance of <see cref="Quantity"/> with the specified value and unit.</returns>
        public Quantity<TUnit> Of<TUnit>(TUnit unit)
            where TUnit : struct, Enum
            => Quantity.Of(value, unit);
    }

    extension(double value)
    {
        /// <summary>
        /// Creates a new quantity with the specified value and unit. This method allows you to easily create a quantity from an integer value by specifying the desired unit.
        /// </summary>
        /// <param name="unit">The unit of measurement for the quantity.</param>
        /// <typeparam name="TUnit">The type of the unit, which must be an enum.</typeparam>
        /// <returns>A new instance of <see cref="Quantity"/> with the specified value and unit.</returns>
        public Quantity<TUnit> Of<TUnit>(TUnit unit)
            where TUnit : struct, Enum
            => Quantity.Of(value, unit);
    }

    extension(decimal value)
    {
        /// <summary>
        /// Creates a new quantity with the specified value and unit. This method allows you to easily create a quantity from an integer value by specifying the desired unit.
        /// </summary>
        /// <param name="unit">The unit of measurement for the quantity.</param>
        /// <typeparam name="TUnit">The type of the unit, which must be an enum.</typeparam>
        /// <returns>A new instance of <see cref="Quantity"/> with the specified value and unit.</returns>
        public Quantity<TUnit> Of<TUnit>(TUnit unit)
            where TUnit : struct, Enum
            => Quantity.Of((double)value, unit);
    }

    extension<TUnit>(Quantity<TUnit> quantity)
        where TUnit : struct, Enum
    {
        /// <summary>
        /// Converts the quantity to the specified target unit using the appropriate converter from the registry.
        /// </summary>
        /// <param name="target">The target unit to convert the quantity to.</param>
        /// <returns>A new <see cref="Quantity{TUnit}"/> instance representing the converted quantity.</returns>
        public Quantity<TUnit> To(TUnit target)
        {
            var converter = UnitConverterRegistry.Get<TUnit>();

            var converted = converter.Convert(quantity.Value, quantity.Unit, target);

            return new(converted, target);
        }

        /// <summary>
        /// Adds another quantity to this quantity, converting the other quantity to this quantity's unit if necessary.
        /// </summary>
        /// <param name="right">The quantity to add.</param>
        /// <returns>A new <see cref="Quantity{TUnit}"/> instance representing the sum of the two quantities.</returns>
        public Quantity<TUnit> Add(Quantity<TUnit> right)
        {
            var converter = UnitConverterRegistry.Get<TUnit>();

            var rightValue = converter.Convert(right.Value, right.Unit, quantity.Unit);

            return new(quantity.Value + rightValue, quantity.Unit);
        }

        /// <summary>
        /// Subtracts another quantity from this quantity, converting the other quantity to this quantity's unit if necessary.
        /// </summary>
        /// <param name="right">The quantity to subtract.</param>
        /// <returns>A new <see cref="Quantity{TUnit}"/> instance representing the difference of the two quantities.</returns>
        public Quantity<TUnit> Subtract(Quantity<TUnit> right)
        {
            var converter = UnitConverterRegistry.Get<TUnit>();

            var rightValue = converter.Convert(right.Value, right.Unit, quantity.Unit);

            return new(quantity.Value - rightValue, quantity.Unit);
        }
    }
}
