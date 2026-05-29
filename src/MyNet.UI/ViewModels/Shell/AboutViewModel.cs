// -----------------------------------------------------------------------
// <copyright file="AboutViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Primitives;
using MyNet.UI.Helpers;
using MyNet.UI.Resources;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Workspace view model for the about screen.
/// </summary>
public sealed class AboutViewModel : WorkspaceViewModel
{
    /// <summary>
    /// Gets the application version.
    /// </summary>
    public string? Version { get; } = ApplicationHelper.GetVersion();

    /// <summary>
    /// Gets or sets an optional message.
    /// </summary>
    public string? Message { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the copyright notice.
    /// </summary>
    public string? Copyright { get; } = ApplicationHelper.GetCopyright();

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string? Product { get; } = ApplicationHelper.GetProductName();

    /// <summary>
    /// Gets the company name.
    /// </summary>
    public string? Company { get; } = ApplicationHelper.GetCompany();

    /// <summary>
    /// Gets the product description.
    /// </summary>
    public string? Description { get; } = ApplicationHelper.GetDescription();

    /// <inheritdoc />
    protected override string? CreateTitle(CultureInfo culture) => UiResources.AboutX.FormatWith(culture, Product ?? string.Empty);
}
