// -----------------------------------------------------------------------
// <copyright file="TabWorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PropertyChanged;

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Provides a reusable base implementation for tabbed workspace view models.
/// </summary>
public abstract class TabWorkspaceViewModel : WorkspaceViewModel, ITabWorkspaceViewModel
{
    private readonly ObservableCollection<IWorkspaceViewModel> _workspaces = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="TabWorkspaceViewModel"/> class.
    /// </summary>
    protected TabWorkspaceViewModel() => Workspaces = new(_workspaces);

    /// <summary>
    /// Gets the hosted workspaces.
    /// </summary>
    public ReadOnlyObservableCollection<IWorkspaceViewModel> Workspaces { get; }

    /// <summary>
    /// Gets the currently selected workspace.
    /// </summary>
    [AlsoNotifyFor(nameof(SelectedIndex))]
    public IWorkspaceViewModel? SelectedWorkspace { get; private set; }

    /// <summary>
    /// Gets the selected workspace index.
    /// </summary>
    public int SelectedIndex => SelectedWorkspace is null ? -1 : _workspaces.IndexOf(SelectedWorkspace);

    /// <summary>
    /// Selects the specified workspace.
    /// </summary>
    /// <param name="workspace">The workspace to select.</param>
    public virtual void Select(IWorkspaceViewModel workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        if (!_workspaces.Contains(workspace))
            throw new ArgumentException(null, nameof(workspace));

        SelectedWorkspace = workspace;
    }

    /// <summary>
    /// Selects the workspace at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index to select.</param>
    public virtual void Select(int index)
    {
        if (index < 0 || index >= _workspaces.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        SelectedWorkspace = _workspaces[index];
    }

    /// <summary>
    /// Adds a hosted workspace.
    /// </summary>
    /// <param name="workspace">The workspace to add.</param>
    protected virtual void AddWorkspace(IWorkspaceViewModel workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        _workspaces.Add(workspace);

        if (workspace.State != LoadState.Loaded)
            _ = workspace.LoadAsync();

        SelectedWorkspace ??= workspace;
    }

    /// <summary>
    /// Adds several hosted workspaces.
    /// </summary>
    /// <param name="workspaces">The workspaces to add.</param>
    protected void AddWorkspaces(IEnumerable<IWorkspaceViewModel> workspaces)
    {
        ArgumentNullException.ThrowIfNull(workspaces);

        foreach (var workspace in workspaces)
            AddWorkspace(workspace);
    }

    /// <summary>
    /// Removes a hosted workspace.
    /// </summary>
    /// <param name="workspace">The workspace to remove.</param>
    /// <returns><see langword="true"/> when the workspace was removed; otherwise <see langword="false"/>.</returns>
    protected virtual bool RemoveWorkspace(IWorkspaceViewModel workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        var removed = _workspaces.Remove(workspace);

        if (!removed)
            return false;

        if (ReferenceEquals(SelectedWorkspace, workspace))
            SelectedWorkspace = _workspaces.FirstOrDefault();

        return true;
    }

    /// <summary>
    /// Clears all hosted workspaces.
    /// </summary>
    protected void ClearWorkspaces()
    {
        _workspaces.Clear();
        SelectedWorkspace = null;
    }
}
