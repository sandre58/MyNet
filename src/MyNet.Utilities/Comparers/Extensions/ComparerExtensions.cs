// -----------------------------------------------------------------------
// <copyright file="ComparerExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Utilities.Comparers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ComparerExtensions
{
    extension<T>(IComparer<T> comparer)
    {
        /// <summary>
        /// Returns a comparer that reverses the order of the current comparer.
        /// </summary>
        /// <returns>The reverse comparer.</returns>
        public IComparer<T> Reverse() => new ReverseComparer<T>(comparer);
    }
}
