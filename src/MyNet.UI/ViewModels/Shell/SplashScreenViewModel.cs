// -----------------------------------------------------------------------
// <copyright file="SplashScreenViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyNet.UI.Helpers;
using MyNet.UI.Resources;
using MyNet.UI.ViewModels.Dialog;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Dialog view model for application startup splash screens with queued startup tasks.
/// </summary>
public class SplashScreenViewModel : DialogViewModel
{
    private readonly Dictionary<Func<string>, (Func<Task> Action, Func<bool> CanExecute)> _tasks = [];

    /// <summary>
    /// Gets the application version.
    /// </summary>
    public string? Version { get; } = ApplicationHelper.GetVersion();

    /// <summary>
    /// Gets the current status message (suffix <c>"..."</c> is applied while a task runs).
    /// </summary>
    public string? Message { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the copyright notice.
    /// </summary>
    public string? Copyright { get; } = ApplicationHelper.GetCopyright();

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string? ProductName { get; } = ApplicationHelper.GetProductName();

    /// <summary>
    /// Gets the company name.
    /// </summary>
    public string? Company { get; } = ApplicationHelper.GetCompany();

    /// <summary>
    /// Gets a value indicating whether startup tasks are currently executing.
    /// </summary>
    public bool IsBusy { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplashScreenViewModel"/> class.
    /// </summary>
    public SplashScreenViewModel() => Message = UiResources.Ready;

    /// <summary>
    /// Enqueues an asynchronous startup task.
    /// </summary>
    public void AddTask(Func<string> message, Func<Task> task, Func<bool>? canExecute = null)
        => _tasks.Add(message, (task, canExecute ?? (() => true)));

    /// <summary>
    /// Enqueues a synchronous startup task executed on the thread pool.
    /// </summary>
    public void AddTask(Func<string> message, Action action, Func<bool>? canExecute = null)
        => AddTask(message, () => Task.Run(action), canExecute);

    /// <summary>
    /// Enqueues a synchronous startup task with a fixed message.
    /// </summary>
    public void AddTask(string message, Action action, Func<bool>? canExecute = null)
        => AddTask(() => message, action, canExecute);

    /// <summary>
    /// Enqueues an asynchronous startup task with a fixed message.
    /// </summary>
    public void AddTask(string message, Func<Task> task, Func<bool>? canExecute = null)
        => AddTask(() => message, task, canExecute);

    /// <summary>
    /// Enqueues multiple startup tasks.
    /// </summary>
    public void AddTasks(IEnumerable<(string Message, Func<Task> Task)> tasks)
    {
        foreach (var task in tasks)
            AddTask(task.Message, task.Task);
    }

    /// <summary>
    /// Enqueues multiple synchronous startup tasks.
    /// </summary>
    public void AddTasks(IEnumerable<(string Message, Action Action)> tasks)
    {
        foreach (var task in tasks)
            AddTask(task.Message, task.Action);
    }

    /// <summary>
    /// Enqueues multiple startup tasks with dynamic messages.
    /// </summary>
    public void AddTasks(IEnumerable<(Func<string> Message, Func<Task> Task)> tasks)
    {
        foreach (var task in tasks)
            AddTask(task.Message, task.Task);
    }

    /// <summary>
    /// Enqueues multiple synchronous startup tasks with dynamic messages.
    /// </summary>
    public void AddTasks(IEnumerable<(Func<string> Message, Action Action)> tasks)
    {
        foreach (var task in tasks)
            AddTask(task.Message, task.Action);
    }

    /// <summary>
    /// Enqueues multiple startup tasks with optional execution guards.
    /// </summary>
    public void AddTasks(IEnumerable<(Func<string> Message, Func<Task> Task, Func<bool> CanExecute)> tasks)
    {
        foreach (var task in tasks)
            AddTask(task.Message, task.Task, task.CanExecute);
    }

    /// <summary>
    /// Enqueues multiple synchronous startup tasks with optional execution guards.
    /// </summary>
    public void AddTasks(IEnumerable<(Func<string> Message, Action Action, Func<bool> CanExecute)> tasks)
    {
        foreach (var task in tasks)
            AddTask(task.Message, task.Action, task.CanExecute);
    }

    /// <summary>
    /// Runs queued startup tasks in registration order.
    /// </summary>
    /// <param name="completedCallBack">Optional callback invoked when all tasks complete successfully.</param>
    /// <param name="failedCallback">Optional callback invoked when a task throws.</param>
    public async Task ExecuteAsync(Action? completedCallBack = null, Action<Exception>? failedCallback = null)
    {
        try
        {
            IsBusy = true;
            foreach (var task in _tasks)
            {
                if (!task.Value.CanExecute.Invoke())
                    continue;

                var message = task.Key.Invoke();
                UpdateMessage(message);

                await Task.Delay(200).ConfigureAwait(false);
                await task.Value.Action.Invoke().ConfigureAwait(false);
            }

            completedCallBack?.Invoke();
        }
        catch (Exception e)
        {
            failedCallback?.Invoke(e);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateMessage(string message) => Message = message + "...";
}
