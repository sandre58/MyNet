// -----------------------------------------------------------------------
// <copyright file="IWorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.ViewModels.Common;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Defines the contract for a workspace view model, which represents a single workspace or tab within the application. The IWorkspaceViewModel interface extends several other interfaces to provide functionality for identifying the workspace, loading its content, and managing its active state. The Title property allows for setting a display name for the workspace, while the Mode property indicates the screen mode in which the workspace is displayed (e.g., normal, maximized, or full-screen). This interface can be implemented by view models that represent individual workspaces within a tabbed interface or multi-document interface, allowing for consistent management of their state and behavior within the application.
/// </summary>
public interface IWorkspaceViewModel : IIdentifiable<Guid>, ILoadable, IActivable
{
    /// <summary>
    /// Gets or sets the title of the workspace, which can be used for display purposes in the user interface. This property allows for providing a meaningful name for the workspace that can help users identify its content or purpose when multiple workspaces are open. The title can be null if no specific name is assigned to the workspace.
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// Gets or sets the screen mode of the workspace, which indicates how the workspace is displayed within the application (e.g., normal, maximized, or full-screen).
    /// </summary>
    ScreenMode Mode { get; set; }
}
