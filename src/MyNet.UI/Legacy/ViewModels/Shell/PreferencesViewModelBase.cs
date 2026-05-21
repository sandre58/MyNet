// -----------------------------------------------------------------------
// <copyright file="PreferencesViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.UI.Legacy.ViewModels.Edition;
using MyNet.UI.Resources;
using MyNet.UI.Services;

namespace MyNet.UI.Legacy.ViewModels.Shell;

public class PreferencesViewModelBase : EditionViewModel
{
    private readonly IPersistentPreferencesService _preferencesService;

    public PreferencesViewModelBase(IPersistentPreferencesService preferencesService, IEnumerable<INavigableWorkspaceViewModel> subWorkspaces)
    {
        _preferencesService = preferencesService;

        AddSubWorkspaces(subWorkspaces);
    }

    protected override string CreateTitle() => UiResources.Preferences;

    protected override void ResetCore() => _preferencesService.Reset();

    protected override void RefreshCore() => _preferencesService.Reload();

    protected override void SaveCore() => _preferencesService.Save();
}
