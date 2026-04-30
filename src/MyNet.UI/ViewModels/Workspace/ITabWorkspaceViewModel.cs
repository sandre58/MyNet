// -----------------------------------------------------------------------
// <copyright file="ITabWorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Defines the contract for a tab workspace view model, which represents a workspace that can host multiple sub-workspaces in a tabbed interface. This interface extends the IWorkspaceViewModel interface to include functionality for managing a collection of sub-workspaces and selecting a specific sub-workspace. The IWorkspaceHost interface is implemented to provide access to the collection of sub-workspaces and allow for selecting a specific sub-workspace by reference or by index. This interface can be implemented by view models that represent tabbed workspaces within the application, allowing for consistent management of their state and behavior while hosting multiple sub-workspaces.
/// </summary>
public interface ITabWorkspaceViewModel : IWorkspaceViewModel, IWorkspaceHost<IWorkspaceViewModel>;
