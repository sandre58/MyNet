// -----------------------------------------------------------------------
// <copyright file="NavigationResult.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Navigation.Models;

/// <summary>
/// Represents the result of a navigation operation.
/// </summary>
/// <param name="Status">The status of the navigation operation.</param>
/// <param name="ErrorMessage">The error message, if any.</param>
/// <param name="Exception">The exception, if any.</param>
public sealed record NavigationResult(NavigationStatus Status, string? ErrorMessage = null, Exception? Exception = null);
