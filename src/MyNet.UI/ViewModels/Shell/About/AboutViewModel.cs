// -----------------------------------------------------------------------
// <copyright file="AboutViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Primitives;
using MyNet.UI.Resources;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Shell.About;

/// <summary>
/// Workspace view model for the about screen.
/// </summary>
public sealed class AboutViewModel : WorkspaceViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
    /// </summary>
    public AboutViewModel(IApplicationInfo applicationInfo)
    {
        ArgumentNullException.ThrowIfNull(applicationInfo);

        Version = applicationInfo.Version;
        Copyright = applicationInfo.Copyright;
        Product = applicationInfo.ProductName;
        Company = applicationInfo.Company;
        Description = applicationInfo.Description;
    }

    /// <summary>
    /// Gets the application version.
    /// </summary>
    public string? Version { get; }

    /// <summary>
    /// Gets or sets an optional message.
    /// </summary>
    public string? Message { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the copyright notice.
    /// </summary>
    public string? Copyright { get; }

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string? Product { get; }

    /// <summary>
    /// Gets the company name.
    /// </summary>
    public string? Company { get; }

    /// <summary>
    /// Gets the product description.
    /// </summary>
    public string? Description { get; }

    /// <inheritdoc />
    protected override string CreateTitle(CultureInfo culture) => UiResources.AboutX.FormatWith(culture, Product ?? string.Empty);
}
