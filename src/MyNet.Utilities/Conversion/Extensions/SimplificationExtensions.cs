// -----------------------------------------------------------------------
// <copyright file="SimplificationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Utilities.Conversion;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class SimplificationExtensions
{
    extension<TUnit>(Quantity<TUnit> quantity)
        where TUnit : struct, Enum
    {
        /// <summary>
        /// Simplifies a quantity by converting it to the most appropriate unit within a specified range. The method iterates through all possible units of the given type, converts the quantity to each unit, and selects the one that results in a value greater than or equal to 1 while being the smallest among those options. Optional minimum and maximum units can be specified to limit the range of units considered for simplification.
        /// </summary>
        /// <param name="min">Optional minimum unit for simplification.</param>
        /// <param name="max">Optional maximum unit for simplification.</param>
        /// <returns>The simplified quantity.</returns>
        public Quantity<TUnit> Simplify(TUnit? min = null, TUnit? max = null)
        {
            var (value, unit) = quantity.Value.Simplify(quantity.Unit, min, max);

            return Quantity.Of(value, unit);
        }
    }

    extension(double value)
    {
        /// <summary>
        /// Simplifies a value by converting it to the most appropriate unit within a specified range. The method iterates through all possible units of the given type, converts the value to each unit, and selects the one that results in a value greater than or equal to 1 while being the smallest among those options. Optional minimum and maximum units can be specified to limit the range of units considered for simplification.
        /// </summary>
        /// <param name="unit">Current unit of the value.</param>
        /// <param name="min">Optional minimum unit for simplification.</param>
        /// <param name="max">Optional maximum unit for simplification.</param>
        /// <typeparam name="TUnit">Type of the unit.</typeparam>
        /// <returns>A tuple containing the simplified value and its corresponding unit.</returns>
        public (double NewValue, TUnit NewUnit) Simplify<TUnit>(TUnit unit, TUnit? min = null, TUnit? max = null)
            where TUnit : struct, Enum
            => value.Simplify(typeof(TUnit), unit, min, max);

        /// <summary>
        /// Simplifies a value by converting it to the most appropriate unit within a specified range. The method iterates through all possible units of the given type, converts the value to each unit, and selects the one that results in a value greater than or equal to 1 while being the smallest among those options. Optional minimum and maximum units can be specified to limit the range of units considered for simplification.
        /// </summary>
        /// <param name="type">Type of unit.</param>
        /// <param name="unit">Current unit of the value.</param>
        /// <param name="min">Optional minimum unit for simplification.</param>
        /// <param name="max">Optional maximum unit for simplification.</param>
        /// <typeparam name="TUnit">Type of the unit.</typeparam>
        /// <returns>A tuple containing the simplified value and its corresponding unit.</returns>
        public (double NewValue, TUnit NewUnit) Simplify<TUnit>(Type type, TUnit unit, TUnit? min = null, TUnit? max = null)
            where TUnit : struct, Enum
        {
            var values = (TUnit[])Enum.GetValues(type);

            var bestValue = value;
            var bestUnit = unit;
            var hasBestCandidate = false;
            var bestUnderOneValue = double.MinValue;
            var bestUnderOneUnit = unit;
            var hasUnderOneCandidate = false;

            foreach (var u in values)
            {
                if (min.HasValue && Comparer<TUnit>.Default.Compare(u, min.Value) < 0)
                    continue;

                if (max.HasValue && Comparer<TUnit>.Default.Compare(u, max.Value) > 0)
                    continue;

                var converted = UnitConverterRegistry.Get<TUnit>().Convert(value, unit, u);

                switch (converted)
                {
                    case >= 1 when !hasBestCandidate || converted < bestValue:
                        bestValue = converted;
                        bestUnit = u;
                        hasBestCandidate = true;
                        break;
                    case < 1 when !hasUnderOneCandidate || converted > bestUnderOneValue:
                        bestUnderOneValue = converted;
                        bestUnderOneUnit = u;
                        hasUnderOneCandidate = true;
                        break;
                }
            }

            return hasBestCandidate
                ? (bestValue, bestUnit)
                : hasUnderOneCandidate
                    ? (bestUnderOneValue, bestUnderOneUnit)
                    : (bestValue, bestUnit);
        }
    }
}
