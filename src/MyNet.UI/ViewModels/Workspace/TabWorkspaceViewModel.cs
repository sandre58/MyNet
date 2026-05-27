// -----------------------------------------------------------------------
// <copyright file="TabWorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Loading;

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
    /// <param name="busyService">Optional busy service used to manage loading state.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    protected TabWorkspaceViewModel(IBusyService? busyService = null, ICommandFactory? commandFactory = null)
        : base(busyService, commandFactory)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;

        Workspaces = new(_workspaces);

        GoToTabCommand = commands.CreateRequired<object>(GoToTab, CanGoToTab);
        GoToNextTabCommand = commands.Create(GoToNextTab, CanGoToNextTab);
        GoToPreviousTabCommand = commands.Create(GoToPreviousTab, CanGoToPreviousTab);
    }

    /// <summary>
    /// Gets the command to navigate to a specific tab by index or workspace reference.
    /// </summary>
    public ICommand GoToTabCommand { get; }

    /// <summary>
    /// Gets the command to navigate to the next tab.
    /// </summary>
    public ICommand GoToNextTabCommand { get; }

    /// <summary>
    /// Gets the command to navigate to the previous tab.
    /// </summary>
    public ICommand GoToPreviousTabCommand { get; }

    /// <summary>
    /// Gets the hosted workspaces.
    /// </summary>
    public ReadOnlyObservableCollection<IWorkspaceViewModel> Workspaces { get; }

    /// <summary>
    /// Gets the currently selected workspace.
    /// </summary>
    public IWorkspaceViewModel? SelectedWorkspace
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value))
                return;

            NotifyPropertyChanged(nameof(SelectedIndex));
            RaiseTabNavigationCanExecuteChanged();
        }
    }

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
            _ = ExecuteAsync(workspace.LoadAsync);

        SelectedWorkspace ??= workspace;
        RaiseTabNavigationCanExecuteChanged();
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

        RaiseTabNavigationCanExecuteChanged();
        return true;
    }

    /// <summary>
    /// Clears all hosted workspaces.
    /// </summary>
    protected void ClearWorkspaces()
    {
        _workspaces.Clear();
        SelectedWorkspace = null;
        RaiseTabNavigationCanExecuteChanged();
    }

    /// <summary>
    /// Navigates to the specified tab by index or workspace reference.
    /// </summary>
    /// <param name="indexOrWorkspace">An <see cref="int"/> index or an <see cref="IWorkspaceViewModel"/> reference.</param>
    protected virtual void GoToTab(object indexOrWorkspace)
    {
        switch (indexOrWorkspace)
        {
            case int index:
                Select(index);
                break;
            case IWorkspaceViewModel workspace:
                Select(workspace);
                break;
        }
    }

    /// <summary>
    /// Determines whether navigation to the specified tab is allowed.
    /// </summary>
    protected virtual bool CanGoToTab(object target) => _workspaces.Count > 0;

    /// <summary>
    /// Navigates to the next tab.
    /// </summary>
    protected virtual void GoToNextTab()
    {
        if (SelectedIndex < _workspaces.Count - 1)
            Select(SelectedIndex + 1);
    }

    /// <summary>
    /// Determines whether navigation to the next tab is allowed.
    /// </summary>
    protected virtual bool CanGoToNextTab() => SelectedWorkspace is not null && SelectedIndex < _workspaces.Count - 1;

    /// <summary>
    /// Navigates to the previous tab.
    /// </summary>
    protected virtual void GoToPreviousTab()
    {
        if (SelectedIndex > 0)
            Select(SelectedIndex - 1);
    }

    /// <summary>
    /// Determines whether navigation to the previous tab is allowed.
    /// </summary>
    protected virtual bool CanGoToPreviousTab() => SelectedWorkspace is not null && SelectedIndex > 0;

    private void RaiseTabNavigationCanExecuteChanged()
    {
        (GoToTabCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
        (GoToNextTabCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
        (GoToPreviousTabCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
    }
}
