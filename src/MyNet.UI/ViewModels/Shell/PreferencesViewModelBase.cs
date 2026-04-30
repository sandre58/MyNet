// -----------------------------------------------------------------------
// <copyright file="PreferencesViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Resources;
using MyNet.UI.Services;
using MyNet.UI.ViewModels.Edition;

namespace MyNet.UI.ViewModels.Shell;

public class PreferencesViewModelBase(IPersistentPreferencesService preferencesService) : EditionViewModel
{
    protected override string CreateTitle() => UiResources.Preferences;

    protected override void ResetCore() => preferencesService.Reset();

    protected override void RefreshCore() => preferencesService.Reload();

    protected override void SaveCore() => preferencesService.Save();
}
