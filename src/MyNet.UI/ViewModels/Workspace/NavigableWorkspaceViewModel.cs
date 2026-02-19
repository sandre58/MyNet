// -----------------------------------------------------------------------
// <copyright file="NavigableWorkspaceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using MyNet.Observable.Attributes;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.ViewModels.Workspace;

[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public class NavigableWorkspaceViewModel : WorkspaceViewModel, INavigableWorkspaceViewModel
{
    public IWorkspaceViewModel? ParentPage { get; private set; }

    public virtual void SetParentPage(IWorkspaceViewModel parentPage) => ParentPage = parentPage;

    public virtual void LoadParameters(INavigationParameters? parameters)
    {
        if (parameters?.Get<object>(TabParameterKey) is { } tab)
            GoToTab(tab);
    }

    public virtual void OnNavigated(NavigationContext navigationContext)
    {
        var canRefresh = CanRefreshOnNavigatedTo(navigationContext);
        LoadParameters(navigationContext.Parameters);

        // Optimize: Only refresh if not loaded or explicitly required
        // Use async refresh to avoid blocking UI
        if (!IsLoaded || canRefresh)
        {
            // Fire and forget async refresh to improve perceived performance
            _ = Task.Run(RefreshAsync);
        }
    }

    public virtual void OnNavigatingFrom(NavigatingContext navigatingContext) { }

    public virtual void OnNavigatingTo(NavigatingContext navigatingContext) => navigatingContext.Cancel = !CanNavigateTo(navigatingContext);

    protected virtual bool CanRefreshOnNavigatedTo(NavigationContext navigationContext) => !IsLoaded;

    protected virtual bool CanNavigateTo(NavigatingContext navigatingContext) => navigatingContext.OldPage is null || !Equals(navigatingContext.OldPage, navigatingContext.Page) || (!navigatingContext.Parameters?.Equals(NavigationService.CurrentContext) ?? false);
}
