// -----------------------------------------------------------------------
// <copyright file="ApplicationInfo.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Helpers;

namespace MyNet.UI.Services;

/// <inheritdoc />
public sealed class ApplicationInfo : IApplicationInfo
{
    /// <inheritdoc />
    public string ProductName { get; } = ApplicationHelper.GetProductName();

    /// <inheritdoc />
    public string? Version { get; } = ApplicationHelper.GetVersion();

    /// <inheritdoc />
    public string? Copyright { get; } = ApplicationHelper.GetCopyright();

    /// <inheritdoc />
    public string? Company { get; } = ApplicationHelper.GetCompany();

    /// <inheritdoc />
    public string? Description { get; } = ApplicationHelper.GetDescription();
}
