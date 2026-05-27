// -----------------------------------------------------------------------
// <copyright file="MainWindowViewModelBase.cs" company="St�phane ANDRE">
// Copyright (c) St�phane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Helpers;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;
using MyNet.UI.Messages;
using MyNet.UI.Services;
using MyNet.UI.Theming;
using PropertyChanged;

namespace MyNet.UI.Legacy.ViewModels.Shell;

public class MainWindowViewModelBase : LocalizableObject
{
    protected IObservableGlobalization Globalization { get; }

    public bool IsDebug { get; }

    public bool IsDark { get; set; }

    public CultureInfo? SelectedCulture { get; set; }

    public ObservableCollection<CultureInfo?> Cultures { get; } = [];

    public TaskbarProgressState ProgressState { get; set; } = TaskbarProgressState.None;

    public double ProgressValue { get; set; }

    public ICommand ToggleNotificationsCommand { get; }

    public ICommand ToggleFileMenuCommand { get; }

    public ICommand CloseDrawersCommand { get; }

    public ICommand ExitCommand { get; }

    public ICommand IsDarkCommand { get; }

    public ICommand IsLightCommand { get; }

    public string ProductName { get; } = ApplicationHelper.GetProductName();

    public virtual string Title => IsDebug ? $"{ProductName} [Debug]" : ProductName;

    public NotificationsViewModel NotificationsViewModel { get; }

    public IBusyService ApplicationBusy { get; }

    public MainWindowViewModelBase(
        INotificationsManager notificationsManager,
        IAppCommandsService appCommandsService,
        IBusyService applicationBusy,
        IObservableGlobalization globalization)
    {
#if DEBUG
        IsDebug = true;
#endif

        Globalization = globalization;
        NotificationsViewModel = new(notificationsManager);
        ApplicationBusy = applicationBusy;

        ToggleNotificationsCommand = RelayCommandFactory.Default.Create(() => Messenger.Default?.Send(new UpdateNotificationsVisibilityRequestedMessage(VisibilityAction.Toggle)), () => !DialogManager.HasOpenedDialogs && NotificationsViewModel.Notifications.Count != 0);
        ToggleFileMenuCommand = RelayCommandFactory.Default.Create(() => Messenger.Default?.Send(new UpdateFileMenuVisibilityRequestedMessage(VisibilityAction.Toggle)), () => !DialogManager.HasOpenedDialogs);
        CloseDrawersCommand = RelayCommandFactory.Default.Create(CloseDrawers, () => !DialogManager.HasOpenedDialogs);
        IsDarkCommand = RelayCommandFactory.Default.Create(() => IsDark = true);
        IsLightCommand = RelayCommandFactory.Default.Create(() => IsDark = false);
        ExitCommand = RelayCommandFactory.Default.Create(appCommandsService.Exit, () => !DialogManager.HasOpenedDialogs);

        applicationBusy.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is not (nameof(IBusyService.IsBusy) or nameof(IBusyService.CurrentBusy)))
                return;

            var currentBusy = applicationBusy.GetCurrent<ProgressionBusy>();
            var progressState = applicationBusy.IsBusy ? TaskbarProgressState.Indeterminate : ProgressState == TaskbarProgressState.Error ? TaskbarProgressState.Error : TaskbarProgressState.None;
            double? progressValue = null;

            if (currentBusy != null)
            {
                if (applicationBusy.IsBusy)
                    currentBusy.PropertyChanged += OnProgressBusyPropertyChanged;
                else
                    currentBusy.PropertyChanged -= OnProgressBusyPropertyChanged;
            }

            RefreshTaskBarState(progressState, progressValue);
        };

        Messenger.Default?.Register<UpdateTaskBarInfoMessage>(this, UpdateTaskBarInfo);

        using (PropertyChangedSuspender.Suspend())
        {
            Cultures.AddRange(Globalization.SupportedCultures);
            IsDark = ThemeManager.CurrentTheme?.Base?.IsDark ?? false;
            UpdateSelectedCulture();
        }

        ThemeManager.ThemeChanged += ThemeService_ThemeChanged;
    }

    [SuppressPropertyChangedWarnings]
    private void OnProgressBusyPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var progressionBusy = (ProgressionBusy?)sender;

        if (progressionBusy is null) return;

        switch (e.PropertyName)
        {
            case nameof(ProgressionBusy.Value):
                RefreshTaskBarState(TaskbarProgressState.Normal, progressionBusy.Value);
                break;

            case nameof(ProgressionBusy.IsCancelling):
                RefreshTaskBarState(TaskbarProgressState.Paused, progressionBusy.Value);
                break;
        }
    }

    protected virtual void CloseDrawers()
    {
        Messenger.Default?.Send(new UpdateNotificationsVisibilityRequestedMessage(VisibilityAction.Hide));
        Messenger.Default?.Send(new UpdateFileMenuVisibilityRequestedMessage(VisibilityAction.Hide));
    }

    #region TaskBar management

    private void UpdateTaskBarInfo(UpdateTaskBarInfoMessage obj) => RefreshTaskBarState(obj.ProgressState, obj.ProgressValue);

    private void RefreshTaskBarState(TaskbarProgressState state, double? progressValue = null)
    {
        ProgressState = state;
        if (progressValue.HasValue) ProgressValue = progressValue.Value;
    }

    #endregion

    #region Culture management

    // ReSharper disable once TailRecursiveCall
    private CultureInfo? GetSelectedCulture(CultureInfo culture) => Cultures.Contains(culture) ? culture : GetSelectedCulture(culture.Parent);

    private void UpdateSelectedCulture() => SelectedCulture = GetSelectedCulture(Globalization.Culture);

    protected virtual void OnSelectedCultureChanged() => Globalization.SetCulture(SelectedCulture?.ToString() ?? CultureInfo.InstalledUICulture.ToString());

    #endregion

    #region Theme management

    private void ThemeService_ThemeChanged(object? sender, ThemeChangedEventArgs e) => IsDark = e.CurrentTheme.Base?.IsDark ?? false;

    protected void OnIsDarkChanged() => ThemeManager.ApplyBase(IsDark ? ThemeManager.Dark! : ThemeManager.Light!);

    #endregion

    protected override void Cleanup()
    {
        Messenger.Default?.Unregister(this);
        NotificationsViewModel.Dispose();
        ThemeManager.ThemeChanged -= ThemeService_ThemeChanged;
        base.Cleanup();
    }
}
