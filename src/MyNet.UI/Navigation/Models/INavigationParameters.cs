// -----------------------------------------------------------------------
// <copyright file="INavigationParameters.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace MyNet.UI.Navigation.Models;

/// <summary>
/// Defines the contract for navigation parameters passed between pages.
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "This interface serves as a marker for navigation parameters and can be extended in the future with common properties or methods.")]
public interface INavigationParameters;
