// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyNet.Primitives;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Builds localized item lists for system enums and smart enums.
/// </summary>
public static class EnumExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Creates a list of <see cref="LocalizedEnum"/> values for a system enum type.
        /// </summary>
        /// <param name="excludedValues">Optional enum values to exclude from the result.</param>
        /// <returns>A localized item for each enum member.</returns>
        public IList<LocalizedEnum> GetLocalizedEnums(IEnumerable<object>? excludedValues = null)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (!type.IsEnum)
            {
                throw new ArgumentException($"Type '{type.FullName}' is not a system enum.");
            }

            var excludedList = excludedValues?.ToList();
            if (excludedList is not null)
            {
                ValidateExcludedValues(type, excludedList);
            }

            return [.. Enum.GetValues(type)
                .Cast<Enum>()
                .Where(x => excludedList?.Contains(x) != true)
                .Select(x => new LocalizedEnum(x))];
        }

        /// <summary>
        /// Creates a list of <see cref="LocalizedSmartEnum"/> values for a smart enum type.
        /// </summary>
        /// <param name="excludedValues">Optional smart enum values to exclude from the result.</param>
        /// <returns>A localized item for each smart enum member.</returns>
        public IList<LocalizedSmartEnum> GetLocalizedSmartEnums(IEnumerable<object>? excludedValues = null)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (!typeof(ISmartEnum).IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"Type '{type.FullName}' does not implement {nameof(ISmartEnum)}.");
            }

            var excludedList = excludedValues?.ToList();
            if (excludedList is not null)
            {
                ValidateExcludedValues(type, excludedList);
            }

            return [.. SmartEnumSource.GetAll(type)
                .Where(x => excludedList?.Contains(x) != true)
                .Select(x => new LocalizedSmartEnum(x))];
        }
    }

    /// <summary>
    /// Creates a list of <see cref="LocalizedEnum{TEnum}"/> values for a system enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="excludedValues">Optional enum values to exclude from the result.</param>
    /// <returns>A localized item for each enum member.</returns>
    public static IList<LocalizedEnum<TEnum>> GetLocalizedEnums<TEnum>(IEnumerable<TEnum>? excludedValues = null)
        where TEnum : struct, Enum
        => [.. Enum.GetValues<TEnum>()
            .Where(x => excludedValues?.Contains(x) != true)
            .Select(x => new LocalizedEnum<TEnum>(x))];

    /// <summary>
    /// Creates a list of <see cref="LocalizedSmartEnum{TSmartEnum}"/> values for a smart enum type.
    /// </summary>
    /// <typeparam name="TSmartEnum">The smart enum type.</typeparam>
    /// <param name="excludedValues">Optional smart enum values to exclude from the result.</param>
    /// <returns>A localized item for each smart enum member.</returns>
    public static IList<LocalizedSmartEnum<TSmartEnum>> GetLocalizedSmartEnums<TSmartEnum>(
        IEnumerable<TSmartEnum>? excludedValues = null)
        where TSmartEnum : class, ISmartEnum
        => [.. SmartEnumSource.GetAll<TSmartEnum>()
            .Where(x => excludedValues?.Contains(x) != true)
            .Select(x => new LocalizedSmartEnum<TSmartEnum>(x))];

    /// <summary>
    /// Validates excluded values against the expected enum or smart enum type.
    /// </summary>
    /// <param name="enumType">The enum or smart enum type.</param>
    /// <param name="excludedValues">The values to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    /// <exception cref="ArgumentException">Thrown when an excluded value has an invalid type.</exception>
    private static void ValidateExcludedValues(Type enumType, IEnumerable<object> excludedValues)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        ArgumentNullException.ThrowIfNull(excludedValues);

        var invalidType = enumType.IsEnum
            ? excludedValues
                .Select(v => Nullable.GetUnderlyingType(v.GetType()) ?? v.GetType())
                .FirstOrDefault(e => !e.IsEnum || e != enumType)
            : excludedValues
                .Select(v => v.GetType())
                .FirstOrDefault(e => e != enumType && !enumType.IsAssignableFrom(e));

        if (invalidType != null)
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Wrong type : {0}", invalidType.Name));
        }
    }
}
