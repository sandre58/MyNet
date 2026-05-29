// -----------------------------------------------------------------------
// <copyright file="EditionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentValidation;
using MyNet.Globalization.Culture;
using MyNet.Observable;
using MyNet.Observable.Behaviors;
using MyNet.Observable.Validation.Validators;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.MessageBox;
using MyNet.UI.Loading.Models;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Resources;
using MyNet.UI.ViewModels.Dialog;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Provides a reusable base implementation for create/edit dialog view models.
/// It encapsulates save, save-and-close, cancel, validation and close-confirmation workflows.
/// </summary>
public abstract class EditionViewModel : DialogViewModel<bool>, IEditionStateViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INotificationPublisher _notificationPublisher;
    private bool _closingByCommand;

    /// <summary>
    /// Gets the command that validates and saves the edited data.
    /// </summary>
    public ICommand SaveCommand { get; }

    /// <summary>
    /// Gets the command that validates, saves and closes the dialog.
    /// </summary>
    public ICommand SaveAndCloseCommand { get; }

    /// <summary>
    /// Gets the command that cancels edition and closes the dialog.
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <inheritdoc />
    public bool IsDirty => this.IsModified();

    /// <summary>
    /// Initializes a new instance of the <see cref="EditionViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service used for confirmation prompts.</param>
    /// <param name="notificationPublisher">Notification publisher used to display validation errors.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    /// <param name="validator">Optional validator used to validate this view model.</param>
    /// <param name="cultureService">Optional culture service used to manage culture changes.</param>
    protected EditionViewModel(
        IDialogService dialogService,
        INotificationPublisher notificationPublisher,
        ICommandFactory? commandFactory = null,
        IValidator? validator = null,
        ICultureService? cultureService = null)
        : base(commandFactory, cultureService)
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));

        var commands = commandFactory ?? RelayCommandFactory.Default;

        CancelCommand = commands.Create(() => CancelAsync());
        SaveCommand = commands.Create(() => SaveAsync(), CanSave);
        SaveAndCloseCommand = commands.Create(() => SaveAndCloseAsync(), CanSave);

        Mode = ScreenMode.Creation;

        this.UseTracking()
            .UseValidation(validator ?? EmptyValidator.Instance);
    }

    /// <summary>
    /// Creates the localized title according to the current edition mode.
    /// </summary>
    /// <param name="culture">The culture used to create the title.</param>
    /// <returns>The localized title for creation or edition mode.</returns>
    protected override string? CreateTitle(CultureInfo culture) => Mode == ScreenMode.Edition ? UiResources.Edition : UiResources.Creation;

    /// <summary>
    /// Resets internal close-state when the dialog opens.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task OnOpenedAsync()
    {
        _closingByCommand = false;
        return base.OnOpenedAsync();
    }

    /// <summary>
    /// Asks whether pending changes should be saved before closing.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The user's decision.</returns>
    protected virtual Task<MessageBoxResult> SavingRequestAsync(CancellationToken cancellationToken = default)
        => _dialogService.ShowQuestionWithCancelAsync(MessageResources.ItemSavingQuestion, UiResources.Edition, cancellationToken);

    /// <summary>
    /// Determines whether the dialog can be closed.
    /// </summary>
    /// <returns><see langword="true"/> when closing is allowed; otherwise <see langword="false"/>.</returns>
    public override async Task<bool> CanCloseAsync()
    {
        if (_closingByCommand || !this.IsModified())
            return true;

        var result = await SavingRequestAsync(CancellationToken.None).ConfigureAwait(false);

        switch (result)
        {
            case MessageBoxResult.Yes:
                return await CanCloseWithYesResultAsync().ConfigureAwait(false);
            case MessageBoxResult.No:
                SetResult(false);
                return true;
            case MessageBoxResult.Cancel or MessageBoxResult.None:
                return false;
            default:
                return true;
        }
    }

    /// <summary>
    /// Handles close confirmation when the user chooses to save before closing.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> when close can continue; otherwise <see langword="false"/>.</returns>
    protected virtual async Task<bool> CanCloseWithYesResultAsync(CancellationToken cancellationToken = default)
    {
        var isSaved = await SaveAsync(cancellationToken).ConfigureAwait(false);

        if (isSaved)
            SetResult(true);

        return isSaved;
    }

    /// <summary>
    /// Handles closing behavior when the user chooses not to save.
    /// </summary>
    /// <param name="e">The cancel event arguments.</param>
    protected virtual void OnClosingWithNoResult(CancelEventArgs e) => SetResult(false);

    /// <summary>
    /// Handles closing behavior when the user cancels the close request.
    /// </summary>
    /// <param name="e">The cancel event arguments.</param>
    protected virtual void OnClosingWithCancelResult(CancelEventArgs e) => e.Cancel = true;

    #region Cancel

    /// <summary>
    /// Determines whether cancellation is allowed.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> when cancellation is allowed; otherwise <see langword="false"/>.</returns>
    protected virtual async Task<bool> CanCancelAsync(CancellationToken cancellationToken = default)
        => !this.IsModified() || await _dialogService.ShowQuestionAsync(MessageResources.ItemModificationCancellingQuestion, UiResources.Edition, cancellationToken).ConfigureAwait(false) == MessageBoxResult.Yes;

    /// <summary>
    /// Cancels edition and closes the dialog when allowed.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task CancelAsync(CancellationToken cancellationToken = default)
    {
        if (await CanCancelAsync(cancellationToken).ConfigureAwait(false))
        {
            _closingByCommand = true;
            Close(false);
        }
    }

    #endregion Cancel

    #region Save

    private async Task<bool> SaveInternalAsync(CancellationToken cancellationToken)
    {
        try
        {
            await BusyService.RunAsync<IndeterminateBusy>(async (_, ct) => await SaveCoreAsync(ct).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
            this.ResetIsModified();
            return true;
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
            return false;
        }
    }

    /// <summary>
    /// Saves changes synchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> when save succeeds; otherwise <see langword="false"/>.</returns>
    protected bool Save(CancellationToken cancellationToken = default)
        => SaveAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>
    /// Validates and saves changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> when save succeeds; otherwise <see langword="false"/>.</returns>
    protected async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        var args = new CancelEventArgs();
        OnSaveRequested(args);

        if (args.Cancel)
            return false;

        if (!this.Validate())
            return HandleValidationErrors();

        var isSaved = await SaveInternalAsync(cancellationToken).ConfigureAwait(false);

        if (!isSaved)
            return false;

        OnSaveSucceeded();

        return true;
    }

    private bool HandleValidationErrors()
    {
        var errors = Behaviors.Get<IValidationBehavior>().Errors;
        ShowValidationErrors(errors);
        OnSaveFailed(errors);

        return false;
    }

    /// <summary>
    /// Displays validation errors to the user.
    /// </summary>
    /// <param name="errors">The validation errors to display.</param>
    protected virtual void ShowValidationErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
            _notificationPublisher.Publish(new MessageNotification(error, severity: NotificationSeverity.Error));
    }

    /// <summary>
    /// Saves the current data synchronously.
    /// </summary>
    protected abstract void SaveCore();

    /// <summary>
    /// Saves the current data asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task SaveCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SaveCore();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called after a successful save operation.
    /// </summary>
    protected virtual void OnSaveSucceeded() { }

    /// <summary>
    /// Called after a failed save operation caused by validation errors.
    /// </summary>
    /// <param name="errors">The validation errors that caused the save failure.</param>
    protected virtual void OnSaveFailed(IReadOnlyCollection<string> errors) { }

    /// <summary>
    /// Called before save starts and can cancel the operation.
    /// </summary>
    /// <param name="args">The cancel event arguments.</param>
    protected virtual void OnSaveRequested(CancelEventArgs args)
    {
    }

    /// <summary>
    /// Determines whether save commands can execute.
    /// </summary>
    /// <returns><see langword="true"/> when save commands can execute; otherwise <see langword="false"/>.</returns>
    protected virtual bool CanSave() => true;

    #endregion Save

    #region SaveAndClose

    /// <summary>
    /// Saves data and closes the dialog.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise <see langword="false"/>.</returns>
    protected virtual bool SaveAndClose(CancellationToken cancellationToken = default)
    {
        if (!Save(cancellationToken))
            return false;

        _closingByCommand = true;
        Close(true);
        return true;
    }

    /// <summary>
    /// Saves data and closes the dialog asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise <see langword="false"/>.</returns>
    protected virtual async Task<bool> SaveAndCloseAsync(CancellationToken cancellationToken = default)
    {
        if (!await SaveAsync(cancellationToken).ConfigureAwait(false))
            return false;

        _closingByCommand = true;
        Close(true);
        return true;
    }

    #endregion SaveAndClose
}
