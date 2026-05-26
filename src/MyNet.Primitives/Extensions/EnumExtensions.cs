// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class EnumExtensions
{
    extension<T>(T options)
        where T : Enum
    {
        /// <summary>
        /// Counts how many flags are set on an enum value.
        /// </summary>
        /// <returns>The number of flags that are set.</returns>
        public int CountFlags() => Enum.GetValues(typeof(T)).Cast<Enum>().Count(options.HasFlag);
    }
}
