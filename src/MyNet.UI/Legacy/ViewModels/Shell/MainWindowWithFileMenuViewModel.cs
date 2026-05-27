// -----------------------------------------------------------------------
// <copyright file="MainWindowWithFileMenuViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Loading;
using MyNet.UI.Services;

namespace MyNet.UI.Legacy.ViewModels.Shell;

public class MainWindowWithFileMenuViewModel(
    FileMenuViewModelBase fileMenuViewModel,
    INotificationsManager notificationsManager,
    IAppCommandsService appCommandsService,
    IBusyService applicationBusy,
    IObservableGlobalization globalization) : MainWindowViewModelBase(notificationsManager, appCommandsService, applicationBusy, globalization)
{
    public FileMenuViewModelBase FileMenuViewModel { get; } = fileMenuViewModel;

    protected override void Cleanup()
    {
        FileMenuViewModel.Dispose();
        base.Cleanup();
    }
}
