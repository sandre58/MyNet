// -----------------------------------------------------------------------
// <copyright file="SplashScreenViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Helpers;
using MyNet.UI.Resources;
using MyNet.UI.ViewModels.Dialog;
using MyNet.UI.ViewModels.Dialogs;
using MyNet.Utilities;
using MyNet.Utilities.Logging;

namespace MyNet.UI.ViewModels.SplashScreen;

/// <summary>
/// ViewModel for a splash screen that supports sequential tasks, parallel groups,
/// UI-thread tasks, background tasks, retry, progress reporting and cancellation.
/// </summary>
public class SplashScreenViewModel : DialogViewModel<bool>
{
    // Internal representation: either a single SplashScreenTask or a SplashScreenParallelGroup.
    private readonly List<object> _steps = [];

    #region Properties

    /// <summary>Gets the application version string.</summary>
    public string? Version { get; } = ApplicationHelper.GetVersion();

    /// <summary>Gets the application copyright notice.</summary>
    public string? Copyright { get; } = ApplicationHelper.GetCopyright();

    /// <summary>Gets the application product name.</summary>
    public string? ProductName { get; } = ApplicationHelper.GetProductName();

    /// <summary>Gets the application company name.</summary>
    public string? Company { get; } = ApplicationHelper.GetCompany();

    /// <summary>Gets the message displayed below the progress bar.</summary>
    public string? Message { get; private set; } = UiResources.Ready;

    /// <summary>Gets a value indicating whether the splash screen is busy executing tasks.</summary>
    public bool IsBusy { get; private set; }

    /// <summary>Gets the progress value in the range [0, 100].</summary>
    public double Progress { get; private set; }

    /// <summary>Gets the 1-based index of the step currently executing.</summary>
    public int CurrentStep { get; private set; }

    /// <summary>Gets the total number of steps (sequential tasks + parallel groups).</summary>
    public int TotalSteps { get; private set; }

    /// <summary>Gets a value indicating whether the last execution was canceled.</summary>
    public bool IsCancelled { get; private set; }

    /// <summary>Gets a value indicating whether the last execution encountered an unhandled error.</summary>
    public bool HasError { get; private set; }

    #endregion

    #region Events

    /// <summary>Raised just before a step starts.</summary>
    public event EventHandler<SplashScreenTaskEventArgs>? TaskStarted;

    /// <summary>Raised just after a step completes successfully.</summary>
    public event EventHandler<SplashScreenTaskEventArgs>? TaskCompleted;

    /// <summary>Raised when a step fails (after all retry attempts).</summary>
    public event EventHandler<SplashScreenTaskFailedEventArgs>? TaskFailed;

    #endregion

    #region Fluent API – adding sequential tasks

    /// <summary>Adds a background task.</summary>
    public SplashScreenViewModel AddTask(
        Func<string> message,
        Func<CancellationToken, Task> action,
        Func<bool>? canExecute = null,
        bool continueOnError = false,
        int retryCount = 0,
        TimeSpan retryDelay = default)
    {
        _steps.Add(new SplashScreenTask(message, action, canExecute, runOnUiThread: false, continueOnError, retryCount, retryDelay));
        return this;
    }

    /// <summary>Adds a background task (synchronous action wrapped in Task.Run).</summary>
    public SplashScreenViewModel AddTask(
        Func<string> message,
        Action action,
        Func<bool>? canExecute = null,
        bool continueOnError = false,
        int retryCount = 0,
        TimeSpan retryDelay = default)
        => AddTask(message, ct => Task.Run(action, ct), canExecute, continueOnError, retryCount, retryDelay);

    /// <summary>Adds a background task (string overload for message).</summary>
    public SplashScreenViewModel AddTask(
        string message,
        Func<CancellationToken, Task> action,
        Func<bool>? canExecute = null,
        bool continueOnError = false,
        int retryCount = 0,
        TimeSpan retryDelay = default)
        => AddTask(() => message, action, canExecute, continueOnError, retryCount, retryDelay);

    /// <summary>Adds a background task (string message + sync action).</summary>
    public SplashScreenViewModel AddTask(
        string message,
        Action action,
        Func<bool>? canExecute = null,
        bool continueOnError = false,
        int retryCount = 0,
        TimeSpan retryDelay = default)
        => AddTask(() => message, ct => Task.Run(action, ct), canExecute, continueOnError, retryCount, retryDelay);

    /// <summary>Adds a background task using a simple <see cref="Func{Task}"/> (no cancellation token).</summary>
    public SplashScreenViewModel AddTask(
        string message,
        Func<Task> task,
        Func<bool>? canExecute = null,
        bool continueOnError = false,
        int retryCount = 0,
        TimeSpan retryDelay = default)
        => AddTask(() => message, _ => task(), canExecute, continueOnError, retryCount, retryDelay);

    /// <summary>Adds a background task using a simple <see cref="Func{Task}"/> (no cancellation token).</summary>
    public SplashScreenViewModel AddTask(
        Func<string> message,
        Func<Task> task,
        Func<bool>? canExecute = null,
        bool continueOnError = false,
        int retryCount = 0,
        TimeSpan retryDelay = default)
        => AddTask(message, _ => task(), canExecute, continueOnError, retryCount, retryDelay);

    /// <summary>
    /// Adds a task that MUST run on the UI thread (e.g. loading resources that are UI-thread-affine).
    /// </summary>
    public SplashScreenViewModel AddUiTask(
        Func<string> message,
        Func<CancellationToken, Task> action,
        Func<bool>? canExecute = null,
        bool continueOnError = false)
    {
        _steps.Add(new SplashScreenTask(message, action, canExecute, runOnUiThread: true, continueOnError));
        return this;
    }

    /// <summary>Adds a synchronous task that MUST run on the UI thread.</summary>
    public SplashScreenViewModel AddUiTask(
        Func<string> message,
        Action action,
        Func<bool>? canExecute = null,
        bool continueOnError = false)
        => AddUiTask(message,
            _ =>
            {
                action();
                return Task.CompletedTask;
            },
            canExecute,
            continueOnError);

    /// <summary>Adds a synchronous UI task (string message).</summary>
    public SplashScreenViewModel AddUiTask(
        string message,
        Action action,
        Func<bool>? canExecute = null,
        bool continueOnError = false)
        => AddUiTask(() => message, action, canExecute, continueOnError);

    /// <summary>Adds an async UI task (string message).</summary>
    public SplashScreenViewModel AddUiTask(
        string message,
        Func<CancellationToken, Task> action,
        Func<bool>? canExecute = null,
        bool continueOnError = false)
        => AddUiTask(() => message, action, canExecute, continueOnError);

    #endregion

    #region Fluent API – adding parallel groups

    /// <summary>
    /// Adds a group of tasks that are executed in parallel. The group counts as a single step.
    /// Individual tasks within the group can still be configured with <see cref="SplashScreenTask"/>.
    /// </summary>
    public SplashScreenViewModel AddParallelGroup(Func<string> groupMessage, IEnumerable<SplashScreenTask> tasks)
    {
        _steps.Add(new SplashScreenParallelGroup(groupMessage, tasks));
        return this;
    }

    /// <inheritdoc cref="AddParallelGroup(Func{string},IEnumerable{SplashScreenTask})"/>
    public SplashScreenViewModel AddParallelGroup(string groupMessage, IEnumerable<SplashScreenTask> tasks)
        => AddParallelGroup(() => groupMessage, tasks);

    /// <summary>
    /// Convenience overload: adds a parallel group from simple (message, action) pairs.
    /// All tasks in the group run on background threads.
    /// </summary>
    public SplashScreenViewModel AddParallelGroup(string groupMessage, IEnumerable<(string Message, Action Action)> tasks)
        => AddParallelGroup(groupMessage, tasks.Select(t => new SplashScreenTask(() => t.Message, ct => Task.Run(t.Action, ct))));

    /// <summary>
    /// Convenience overload: adds a parallel group from (message, Func&lt;Task&gt;) pairs.
    /// </summary>
    public SplashScreenViewModel AddParallelGroup(string groupMessage, IEnumerable<(string Message, Func<Task> Task)> tasks)
        => AddParallelGroup(groupMessage, tasks.Select(t => new SplashScreenTask(() => t.Message, _ => t.Task())));

    #endregion

    #region Bulk helpers

    public SplashScreenViewModel AddTasks(IEnumerable<(string Message, Func<Task> Task)> tasks)
    {
        tasks.ForEach(x => AddTask(x.Message, x.Task));
        return this;
    }

    public SplashScreenViewModel AddTasks(IEnumerable<(string Message, Action Action)> tasks)
    {
        tasks.ForEach(x => AddTask(x.Message, x.Action));
        return this;
    }

    public SplashScreenViewModel AddTasks(IEnumerable<(Func<string> Message, Func<Task> Task)> tasks)
    {
        tasks.ForEach(x => AddTask(x.Message, x.Task));
        return this;
    }

    public SplashScreenViewModel AddTasks(IEnumerable<(Func<string> Message, Action Action)> tasks)
    {
        tasks.ForEach(x => AddTask(x.Message, x.Action));
        return this;
    }

    public SplashScreenViewModel AddTasks(IEnumerable<(Func<string> Message, Func<Task> Task, Func<bool> CanExecute)> tasks)
    {
        tasks.ForEach(x => AddTask(x.Message, _ => x.Task(), x.CanExecute));
        return this;
    }

    public SplashScreenViewModel AddTasks(IEnumerable<(Func<string> Message, Action Action, Func<bool> CanExecute)> tasks)
    {
        tasks.ForEach(x => AddTask(x.Message, x.Action, x.CanExecute));
        return this;
    }

    #endregion

    #region Execution

    /// <summary>
    /// Executes all registered steps in order.
    /// </summary>
    /// <param name="completedCallback">Invoked on the UI thread when all steps complete successfully.</param>
    /// <param name="failedCallback">Invoked on the UI thread when an unhandled exception occurs.</param>
    /// <param name="cancellationToken">Token that can be used to abort the sequence.</param>
    public async Task ExecuteAsync(
        Action? completedCallback = null,
        Action<Exception>? failedCallback = null,
        CancellationToken cancellationToken = default)
    {
        // Capture the UI synchronization context so property updates are dispatched correctly.
        var syncContext = SynchronizationContext.Current;

        var activeSteps = _steps
            .Where(s => s switch
            {
                SplashScreenTask t => t.CanExecute(),
                SplashScreenParallelGroup g => g.Tasks.Any(t => t.CanExecute()),
                _ => false
            })
            .ToList();

        SetOnContext(syncContext, () =>
        {
            IsBusy = true;
            IsCancelled = false;
            HasError = false;
            TotalSteps = activeSteps.Count;
            CurrentStep = 0;
            Progress = 0;
        });

        try
        {
            for (var i = 0; i < activeSteps.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var step = activeSteps[i];
                var stepIndex = i + 1;

                switch (step)
                {
                    case SplashScreenTask task:
                        await RunStepAsync(syncContext, task, stepIndex, activeSteps.Count, cancellationToken).ConfigureAwait(false);
                        break;

                    case SplashScreenParallelGroup group:
                        await RunParallelGroupAsync(syncContext, group, stepIndex, activeSteps.Count, cancellationToken).ConfigureAwait(false);
                        break;
                }

                SetOnContext(syncContext, () =>
                {
                    CurrentStep = stepIndex;
                    Progress = (double)stepIndex / activeSteps.Count * 100;
                });
            }

            InvokeOnContext(syncContext, completedCallback);
        }
        catch (OperationCanceledException)
        {
            SetOnContext(syncContext, () => IsCancelled = true);
        }
        catch (Exception e)
        {
            SetOnContext(syncContext, () => HasError = true);
            InvokeOnContext(syncContext, () => failedCallback?.Invoke(e));
        }
        finally
        {
            SetOnContext(syncContext, () =>
            {
                IsBusy = false;
                Message = UiResources.Ready;
            });
        }
    }

    #endregion

    #region Private helpers

    /// <summary>
    /// Posts an async action onto the UI synchronization context and awaits its completion.
    /// </summary>
    private static Task RunOnUiThreadAsync(SynchronizationContext? syncContext, Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        if (syncContext is null)
            return action(cancellationToken);

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        syncContext.Post(async _ =>
            {
                try
                {
                    await action(cancellationToken).ConfigureAwait(false);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            },
            null);
        return tcs.Task;
    }

    /// <summary>
    /// Posts a property-update action on the UI context (fire-and-forget, no wait needed for simple setters).
    /// </summary>
    private static void SetOnContext(SynchronizationContext? context, Action action)
    {
        if (context is not null)
            context.Post(_ => action(), null);
        else
            action();
    }

    /// <summary>
    /// Sends an action on the UI context synchronously to ensure completion before continuing.
    /// </summary>
    private static void InvokeOnContext(SynchronizationContext? context, Action? action)
    {
        if (action is null) return;

        if (context is not null)
            context.Send(_ => action(), null);
        else
            action();
    }

    private async Task RunStepAsync(
        SynchronizationContext? syncContext,
        SplashScreenTask task,
        int stepIndex,
        int totalSteps,
        CancellationToken cancellationToken)
    {
        var message = task.Message();
        SetOnContext(syncContext, () =>
        {
            CurrentStep = stepIndex;
            Message = message + "...";
        });

        TaskStarted?.Invoke(this, new SplashScreenTaskEventArgs(message, stepIndex, totalSteps));

        // Small yield so the UI can render the updated message.
        await Task.Delay(50, cancellationToken).ConfigureAwait(false);

        var attempts = 0;
        while (true)
        {
            try
            {
                using (LogManager.MeasureTime(message, PerformanceTraceLevel.Debug))
                {
                    if (task.RunOnUiThread)
                        await RunOnUiThreadAsync(syncContext, task.Action, cancellationToken).ConfigureAwait(false);
                    else
                        await task.Action(cancellationToken).ConfigureAwait(false);
                }

                TaskCompleted?.Invoke(this, new SplashScreenTaskEventArgs(message, stepIndex, totalSteps));
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (attempts < task.RetryCount)
                {
                    attempts++;
                    LogManager.Warning($"[SplashScreen] Step '{message}' failed (attempt {attempts}/{task.RetryCount}). Retrying in {task.RetryDelay.TotalMilliseconds} ms...");
                    await Task.Delay(task.RetryDelay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                TaskFailed?.Invoke(this, new SplashScreenTaskFailedEventArgs(message, stepIndex, totalSteps, ex));

                if (!task.ContinueOnError) throw;

                LogManager.Warning($"[SplashScreen] Step '{message}' failed and ContinueOnError=true. Skipping. Error: {ex.Message}");
                return;
            }
        }
    }

    private async Task RunParallelGroupAsync(
        SynchronizationContext? syncContext,
        SplashScreenParallelGroup group,
        int stepIndex,
        int totalSteps,
        CancellationToken cancellationToken)
    {
        var message = group.GroupMessage();
        SetOnContext(syncContext, () =>
        {
            CurrentStep = stepIndex;
            Message = message + "...";
        });

        await Task.Delay(50, cancellationToken).ConfigureAwait(false);

        var eligibleTasks = group.Tasks.Where(t => t.CanExecute()).ToList();

        // Run all eligible tasks concurrently; each handles its own retry/continueOnError.
        var parallelTasks = eligibleTasks.Select(t => RunStepAsync(syncContext, t, stepIndex, totalSteps, cancellationToken));
        await Task.WhenAll(parallelTasks).ConfigureAwait(false);
    }

    #endregion
}
