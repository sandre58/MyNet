// -----------------------------------------------------------------------
// <copyright file="BusyTaskbarCoordinator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.ViewModels.Shell.Taskbar;

/// <summary>
/// Synchronizes application <see cref="IBusyService"/> state with <see cref="ITaskbarProgressSource"/>.
/// </summary>
public sealed class BusyTaskbarCoordinator : IDisposable
{
    private readonly IBusyService _applicationBusy;
    private readonly ITaskbarProgressSource _taskbarProgress;
    private ProgressionBusy? _subscribedProgression;

    /// <summary>
    /// Initializes a new instance of the <see cref="BusyTaskbarCoordinator"/> class.
    /// </summary>
    public BusyTaskbarCoordinator(IBusyService applicationBusy, ITaskbarProgressSource taskbarProgress)
    {
        _applicationBusy = applicationBusy ?? throw new ArgumentNullException(nameof(applicationBusy));
        _taskbarProgress = taskbarProgress ?? throw new ArgumentNullException(nameof(taskbarProgress));

        _applicationBusy.PropertyChanged += OnApplicationBusyPropertyChanged;
        SyncFromApplicationBusy();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _applicationBusy.PropertyChanged -= OnApplicationBusyPropertyChanged;
        UnsubscribeProgression();
    }

    private void OnApplicationBusyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not (nameof(IBusyService.IsBusy) or nameof(IBusyService.CurrentBusy)))
            return;

        SyncFromApplicationBusy();
    }

    private void SyncFromApplicationBusy()
    {
        var progressionBusy = _applicationBusy.GetCurrent<ProgressionBusy>();
        UpdateProgressionSubscription(progressionBusy);

        var progressState = _applicationBusy.IsBusy
            ? TaskbarProgressState.Indeterminate
            : _taskbarProgress.ProgressState == TaskbarProgressState.Error
                ? TaskbarProgressState.Error
                : TaskbarProgressState.None;

        _taskbarProgress.SetProgress(progressState);
    }

    private void UpdateProgressionSubscription(ProgressionBusy? progressionBusy)
    {
        if (ReferenceEquals(_subscribedProgression, progressionBusy))
            return;

        UnsubscribeProgression();

        _subscribedProgression = progressionBusy;

        if (_subscribedProgression is not null && _applicationBusy.IsBusy)
            _subscribedProgression.PropertyChanged += OnProgressionBusyPropertyChanged;
    }

    private void UnsubscribeProgression()
    {
        if (_subscribedProgression is null)
            return;

        _subscribedProgression.PropertyChanged -= OnProgressionBusyPropertyChanged;
        _subscribedProgression = null;
    }

    private void OnProgressionBusyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ProgressionBusy progressionBusy)
            return;

        switch (e.PropertyName)
        {
            case nameof(ProgressionBusy.Value):
                _taskbarProgress.SetProgress(TaskbarProgressState.Normal, progressionBusy.Value);
                break;

            case nameof(ProgressionBusy.IsCancelling):
                _taskbarProgress.SetProgress(TaskbarProgressState.Paused, progressionBusy.Value);
                break;
        }
    }
}
