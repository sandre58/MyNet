// -----------------------------------------------------------------------
// <copyright file="IWorkspaceHost.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Defines the contract for a workspace host, which manages a collection of workspaces and allows for selecting a specific workspace. The Workspaces property provides access to the collection of workspaces, while the SelectedWorkspace and SelectedIndex properties indicate the currently selected workspace and its index in the collection. The Select methods allow for selecting a workspace either by reference or by index. This interface can be implemented by view models or other components that need to manage multiple workspaces within the application, such as tabbed interfaces or multi-document interfaces.
/// </summary>
/// <typeparam name="TWorkspace">The type of workspace managed by the host.</typeparam>
public interface IWorkspaceHost<TWorkspace>
{
    /// <summary>
    /// Gets the collection of workspaces managed by the host. This property provides access to the list of available workspaces that can be selected and interacted with. The collection is read-only, meaning that it cannot be modified directly through this property, but it can be updated internally by the host as needed to reflect changes in the available workspaces.
    /// </summary>
    ReadOnlyObservableCollection<TWorkspace> Workspaces { get; }

    /// <summary>
    /// Gets the currently selected workspace. This property indicates which workspace is currently active or focused within the host. It can be null if no workspace is selected.
    /// </summary>
    TWorkspace? SelectedWorkspace { get; }

    /// <summary>
    /// Gets the index of the currently selected workspace. This property indicates the position of the selected workspace within the collection of workspaces. It can be -1 if no workspace is selected.
    /// </summary>
    int SelectedIndex { get; }

    /// <summary>
    /// Selects the specified workspace. This method allows for changing the currently active workspace by providing a reference to the desired workspace.
    /// </summary>
    /// <param name="workspace">The workspace to select.</param>
    void Select(TWorkspace workspace);

    /// <summary>
    /// Selects the workspace at the specified index. This method allows for changing the currently active workspace by providing the index of the desired workspace within the collection.
    /// </summary>
    /// <param name="index">The index of the workspace to select.</param>
    void Select(int index);
}
