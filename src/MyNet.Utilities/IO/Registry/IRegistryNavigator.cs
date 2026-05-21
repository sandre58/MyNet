// -----------------------------------------------------------------------
// <copyright file="IRegistryNavigator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Utilities.IO.Registry;

/// <summary>
/// Interface for navigating the Windows Registry, allowing retrieval of subkeys and counting of child keys under a given registry path.
/// </summary>
public interface IRegistryNavigator
{
    /// <summary>
    /// Retrieves the child registry paths under the specified parent registry path.
    /// </summary>
    /// <param name="parent">The parent registry path.</param>
    /// <returns>An enumerable collection of child registry paths.</returns>
    IEnumerable<RegistryPath> GetChildren(RegistryPath parent);

    /// <summary>
    /// Counts the number of child registry paths under the specified parent registry path.
    /// </summary>
    /// <param name="parent">The parent registry path.</param>
    /// <returns>The number of child registry paths.</returns>
    int Count(RegistryPath parent);
}
