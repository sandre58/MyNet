// -----------------------------------------------------------------------
// <copyright file="SplashScreenParallelGroup.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.UI.ViewModels.SplashScreen;

/// <summary>
/// A group of <see cref="SplashScreenTask"/> that are executed in parallel.
/// The group itself is treated as a single step in the overall sequence.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SplashScreenParallelGroup"/> class.
/// </remarks>
/// <param name="groupMessage">Factory that produces the localized group message.</param>
/// <param name="tasks">The tasks to run concurrently.</param>
public sealed class SplashScreenParallelGroup(Func<string> groupMessage, IEnumerable<SplashScreenTask> tasks)
{
    /// <summary>Gets the localized message for the group.</summary>
    public Func<string> GroupMessage { get; } = groupMessage;

    /// <summary>Gets the tasks in this parallel group.</summary>
    public IReadOnlyList<SplashScreenTask> Tasks { get; } = [.. tasks];
}
