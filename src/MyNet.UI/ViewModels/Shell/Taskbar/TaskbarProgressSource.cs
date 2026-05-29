// -----------------------------------------------------------------------
// <copyright file="TaskbarProgressSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable;

namespace MyNet.UI.ViewModels.Shell.Taskbar;

/// <summary>
/// Taskbar progress source for shell chrome binding.
/// </summary>
public sealed class TaskbarProgressSource : ObservableObject, ITaskbarProgressSource
{
    /// <inheritdoc />
    public TaskbarProgressState ProgressState { get; private set => SetProperty(ref field, value); } = TaskbarProgressState.None;

    /// <inheritdoc />
    public double ProgressValue { get; private set => SetProperty(ref field, value); }

    /// <inheritdoc />
    public void SetProgress(TaskbarProgressState progressState, double? progressValue = null)
    {
        ProgressState = progressState;

        if (progressValue.HasValue)
            ProgressValue = progressValue.Value;
    }

    /// <inheritdoc />
    public void SetError(double progressValue = 1) => SetProgress(TaskbarProgressState.Error, progressValue);
}
