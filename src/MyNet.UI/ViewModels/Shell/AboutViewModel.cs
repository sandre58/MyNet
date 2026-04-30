// -----------------------------------------------------------------------
// <copyright file="AboutViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Helpers;
using MyNet.UI.Resources;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.Shell;

public class AboutViewModel : WorkspaceViewModel
{
    public string? Version { get; } = ApplicationHelper.GetVersion();

    public string? Message { get; set; }

    public string? Copyright { get; } = ApplicationHelper.GetCopyright();

    public string? Product { get; } = ApplicationHelper.GetProductName();

    public string? Company { get; } = ApplicationHelper.GetCompany();

    public string? Description { get; } = ApplicationHelper.GetDescription();

    protected override string CreateTitle() => UiResources.AboutX.FormatWith(Product ?? string.Empty);
}
