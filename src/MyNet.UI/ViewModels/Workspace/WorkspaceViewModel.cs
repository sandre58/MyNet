// -----------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using MyNet.Observable.Attributes;
using MyNet.UI.Commands;
using MyNet.UI.Loading;
using MyNet.UI.Navigation;
using MyNet.UI.Navigation.Models;
using MyNet.UI.Threading;
using MyNet.Utilities;
using PropertyChanged;

namespace MyNet.UI.ViewModels.Workspace;

[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public abstract class WorkspaceViewModel : ViewModelBase, IWorkspaceViewModel
{
    public const string TabParameterKey = "Tab";

    private readonly ReadOnlyObservableCollection<INavigableWorkspaceViewModel> _subworkspaces;
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Dispose in Cleanup")]
    private readonly SourceCache<INavigableWorkspaceViewModel, Guid> _allSubworkspaces = new(x => x.Id);

    // Cache for SelectedWorkspaceIndex to avoid repeated IndexOf calls
    private int _cachedSelectedWorkspaceIndex = -1;

    public event EventHandler? RefreshCompleted;

    #region Members

    [UpdateOnCultureChanged]
    public string? Title { get; set; }

    public bool IsEnabled { get; set; } = true;

    public ScreenMode Mode { get; set; } = ScreenMode.Read;

    public SubWorkspaceNavigationService NavigationService { get; }

    INavigationService IWorkspaceViewModel.NavigationService => NavigationService;

    [CanBeValidated]
    [CanSetIsModified]
    public ReadOnlyObservableCollection<INavigableWorkspaceViewModel> SubWorkspaces => _subworkspaces;

    protected IEnumerable<INavigableWorkspaceViewModel> AllWorkspaces => _allSubworkspaces.Items;

    public INavigableWorkspaceViewModel? SelectedWorkspace => NavigationService.CurrentContext?.Page as INavigableWorkspaceViewModel;

    public int SelectedWorkspaceIndex => _cachedSelectedWorkspaceIndex;

    public ICommand GoToTabCommand { get; set; }

    public ICommand GoToPreviousTabCommand { get; set; }

    public ICommand GoToNextTabCommand { get; set; }

    public ICommand RefreshCommand { get; }

    public ICommand ResetCommand { get; }

    #endregion Members

    protected WorkspaceViewModel(IBusyService? busyService = null)
     : base(busyService)
    {
        GoToTabCommand = CommandsManager.CreateNotNull<object>(GoToTab, CanGoToTab);
        GoToNextTabCommand = CommandsManager.Create(GoToNextTab, CanGoToNextTab);
        GoToPreviousTabCommand = CommandsManager.Create(GoToPreviousTab, CanGoToPreviousTab);
        RefreshCommand = CommandsManager.Create(async () => await RefreshAsync().ConfigureAwait(false), CanRefresh);
        ResetCommand = CommandsManager.Create(async () => await ResetWithCheckingAsync().ConfigureAwait(false), CanReset);

        var obs = _allSubworkspaces.Connect();
        Disposables.AddRange([

            obs.ForEachChange(x =>
            {
     if (x.Reason == ChangeReason.Add)
         {
       x.Current.SetParentPage(this);
 }
            }).Subscribe(),

            obs.DisposeMany()
         .AutoRefresh(x => x.IsEnabled)
          .Filter(x => x.IsEnabled)
     .ObserveOn(Scheduler.UiOrCurrent)
  .Bind(out _subworkspaces)
        .Subscribe(_ => InvalidateSelectedWorkspaceIndexCache())
  ]);

        NavigationService = new SubWorkspaceNavigationService(this);
        NavigationService.Navigated += OnSelectedSubWorkspaceChangedCallBack;

        UpdateTitle();
    }

    protected void AddSubWorkspace(INavigableWorkspaceViewModel workspace) => AddSubWorkspaces([workspace]);

    protected void AddSubWorkspaces(IEnumerable<INavigableWorkspaceViewModel> workspaces) => _allSubworkspaces.AddOrUpdate(workspaces);

    protected void RemoveSubWorkspace(INavigableWorkspaceViewModel workspace) => _allSubworkspaces.Remove(workspace);

    protected void RemoveSubWorkspaces(IEnumerable<INavigableWorkspaceViewModel> workspaces) => _allSubworkspaces.Remove(workspaces);

    protected void ClearSubWorkspaces() => _allSubworkspaces.Clear();

    protected void SetSubWorkspaces(IEnumerable<INavigableWorkspaceViewModel> workspaces) => _allSubworkspaces.Edit(x =>
  {
      x.Clear();
      x.AddOrUpdate(workspaces);
  });

    public T? GetSubWorkspace<T>()
        where T : INavigableWorkspaceViewModel
   => AllWorkspaces.OfType<T>().FirstOrDefault();

    private void InvalidateSelectedWorkspaceIndexCache()
    {
        var selected = SelectedWorkspace;
        _cachedSelectedWorkspaceIndex = selected is not null ? _subworkspaces.IndexOf(selected) : -1;
    }

    private void OnSelectedSubWorkspaceChangedCallBack(object? sender, NavigationEventArgs e)
    {
        InvalidateSelectedWorkspaceIndexCache();
        OnPropertyChanged(nameof(SelectedWorkspace));
        OnPropertyChanged(nameof(SelectedWorkspaceIndex));

        OnSelectedSubWorkspaceChanged(new NavigatingContext(e.OldPage, e.NewPage, e.Mode, e.Parameters));
    }

    [SuppressPropertyChangedWarnings]
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Used by children classes")]
    protected virtual void OnSelectedSubWorkspaceChanged(NavigatingContext navigatingContext) { }

    #region Refresh

    /// <summary>
    /// Refreshes the workspace and all its sub-workspaces asynchronously.
    /// Sub-workspaces are refreshed in parallel for optimal performance.
    /// This is the primary refresh method. For synchronous scenarios, use RefreshCommand which handles async execution.
    /// </summary>
    [SuppressMessage("Performance", "CA1849:Call async methods when in an async method", Justification = "Provide two options")]
    public virtual async Task RefreshAsync()
    {
        using (IsModifiedSuspender.Suspend())
        {
            OnRefreshRequested();

            await ExecuteAsync(async () =>
                {
                    // Execute synchronous core logic first
                    RefreshCore();

                    // Execute asynchronous core logic if overridden
                    await RefreshCoreAsync().ConfigureAwait(false);

                    // Refresh all sub-workspaces in parallel for better performance
                    var subWorkSpacesList = SubWorkspaces;
                    if (subWorkSpacesList.Count > 0)
                    {
                        await Task.WhenAll(subWorkSpacesList.Select(w => w.RefreshAsync())).ConfigureAwait(false);
                    }

                    ResetValidation();
                    ResetIsModified();

                    OnRefreshCompleted();
                }).ConfigureAwait(false);

            _ = NavigationService.CheckSelectedWorkspace();

            RefreshCompleted?.Invoke(this, EventArgs.Empty);
            MarkAsLoaded();
        }
    }

    /// <summary>
    /// Override this method to perform synchronous refresh operations.
    /// For simple data initialization, property assignments, or lightweight operations.
    /// </summary>
    /// <example>
    /// <code>
    /// protected override void RefreshCore()
    /// {
    ///     Title = "My Workspace";
    ///     IsReadOnly = false;
    /// }
    /// </code>
    /// </example>
    protected virtual void RefreshCore() { }

    /// <summary>
    /// Override this method to perform asynchronous refresh operations.
    /// For I/O operations like database queries, API calls, or file loading.
    /// </summary>
    /// <example>
    /// <code>
    /// protected override async Task RefreshCoreAsync()
    /// {
    ///     Items = await _repository.GetAllAsync();
    ///     Statistics = await _service.ComputeStatsAsync();
    /// }
    /// </code>
    /// </example>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task RefreshCoreAsync() => Task.CompletedTask;

    protected virtual void OnRefreshRequested() { }

    protected virtual void OnRefreshCompleted() { }

    protected virtual bool CanRefresh() => true;

    #endregion Refresh

    #region GoToTab

    public virtual void GoToTab(object indexOrSubWorkspace)
    {
        // Early exit if already selected
        if (Equals(indexOrSubWorkspace, SelectedWorkspace) || Equals(indexOrSubWorkspace, _cachedSelectedWorkspaceIndex))
            return;

        _ = NavigationService.NavigateTo(indexOrSubWorkspace);
    }

    public virtual void GoToPreviousTab()
    {
        if (SelectedWorkspace is null) return;

        var currentIndex = _cachedSelectedWorkspaceIndex;
        if (currentIndex <= 0) return;

        GoToTab(currentIndex - 1);
    }

    public virtual void GoToNextTab()
    {
        if (SelectedWorkspace is null) return;

        var currentIndex = _cachedSelectedWorkspaceIndex;
        if (currentIndex < 0 || currentIndex >= SubWorkspaces.Count - 1) return;

        GoToTab(currentIndex + 1);
    }

    protected virtual bool CanGoToTab(object? indexOrSubWorkspace) => true;

    protected virtual bool CanGoToNextTab()
    {
        if (SelectedWorkspace is null) return false;

        var currentIndex = _cachedSelectedWorkspaceIndex;
        return currentIndex >= 0 && currentIndex < SubWorkspaces.Count - 1;
    }

    protected virtual bool CanGoToPreviousTab()
    {
        if (SelectedWorkspace is null) return false;

        var currentIndex = _cachedSelectedWorkspaceIndex;
        return currentIndex > 0;
    }

    #endregion GoToTab

    #region Reset

    protected async Task ResetWithCheckingAsync()
    {
        if (CheckCanReset())
            await ResetAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Resets the workspace asynchronously with busy indication.
    /// Sub-workspaces are reset in parallel for optimal performance.
    /// Use this when ResetCore contains heavy operations.
    /// </summary>
    [SuppressMessage("Performance", "CA1849:Call async methods when in an async method", Justification = "Provide two options")]
    public virtual async Task ResetAsync()
    {
        using (IsModifiedSuspender.Suspend())
        {
            await ExecuteAsync(async () =>
             {
                 // Execute synchronous core logic first
                 ResetCore();

                 // Execute asynchronous core logic if overridden
                 await ResetCoreAsync().ConfigureAwait(false);

                 // Reset all sub-workspaces in parallel for better performance
                 var subWorkspacesList = SubWorkspaces;
                 if (subWorkspacesList.Count > 0)
                 {
                     await Task.WhenAll(subWorkspacesList.Select(w => w.ResetAsync())).ConfigureAwait(false);
                 }

                 ResetIsModified();
             }).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Override this method to perform synchronous reset operations.
    /// For simple state resets, property clearing, or lightweight operations.
    /// </summary>
    /// <example>
    /// <code>
    /// protected override void ResetCore()
    /// {
    ///     SelectedItem = null;
    ///     SearchText = string.Empty;
    /// }
    /// </code>
    /// </example>
    protected virtual void ResetCore() { }

    /// <summary>
    /// Override this method to perform asynchronous reset operations.
    /// For operations that require I/O like clearing caches, resetting remote state, etc.
    /// </summary>
    /// <example>
    /// <code>
    /// protected override async Task ResetCoreAsync()
    /// {
    ///     await _cache.ClearAsync();
    ///     await _service.ResetUserPreferencesAsync();
    /// }
    /// </code>
    /// </example>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task ResetCoreAsync() => Task.CompletedTask;

    protected virtual bool CheckCanReset() => true;

    protected virtual bool CanReset() => true;

    #endregion Reset

    #region Culture Management

    protected virtual string CreateTitle() => string.Empty;

    protected void UpdateTitle()
    {
        var newTitle = CreateTitle();

        if (!string.IsNullOrEmpty(newTitle))
            Title = newTitle;
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        NavigationService.Navigated -= OnSelectedSubWorkspaceChangedCallBack;
        NavigationService.Dispose();
        _allSubworkspaces.Dispose();
    }

    protected virtual void OnModeChanged() => UpdateTitle();

    protected override void OnCultureChanged()
    {
        base.OnCultureChanged();

        UpdateTitle();
    }
    #endregion

    public override bool Equals(object? obj) => obj is WorkspaceViewModel workspace && Id == workspace.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
