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

public abstract class EditionViewModel : DialogViewModel<bool>
{
    private readonly IDialogService _dialogService;
    private readonly INotificationPublisher _notificationPublisher;
    private bool _closingByCommand;

    public ICommand SaveCommand { get; }

    public ICommand SaveAndCloseCommand { get; }

    public ICommand CancelCommand { get; }

    protected EditionViewModel(
        IDialogService dialogService,
        INotificationPublisher notificationPublisher,
        ICommandFactory? commandFactory = null,
        IValidator? validator = null)
        : base(commandFactory)
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));

        var commands = commandFactory ?? RelayCommandFactory.Default;

        CancelCommand = commands.Create(async () => await CancelAsync().ConfigureAwait(false));
        SaveCommand = commands.Create(async () => await SaveAsync().ConfigureAwait(false), CanSave);
        SaveAndCloseCommand = commands.Create(async () => await SaveAndCloseAsync().ConfigureAwait(false), CanSave);

        Mode = ScreenMode.Creation;

        this.UseTracking()
            .UseValidation(validator ?? EmptyValidator.Instance);
    }

    protected override string? CreateTitle(CultureInfo culture) => Mode == ScreenMode.Edition ? UiResources.Edition : UiResources.Creation;

    public override Task OnOpenedAsync()
    {
        _closingByCommand = false;
        return base.OnOpenedAsync();
    }

    protected virtual Task<MessageBoxResult> SavingRequestAsync(CancellationToken cancellationToken = default)
        => _dialogService.ShowQuestionWithCancelAsync(MessageResources.ItemSavingQuestion, UiResources.Edition);

    public override async Task<bool> CanCloseAsync()
    {
        if (_closingByCommand || !this.IsModified())
            return true;

        var result = await SavingRequestAsync().ConfigureAwait(false);

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

    protected virtual async Task<bool> CanCloseWithYesResultAsync(CancellationToken cancellationToken = default)
    {
        var isSaved = await SaveAsync(cancellationToken).ConfigureAwait(false);

        if (isSaved)
            SetResult(true);

        return isSaved;
    }

    protected virtual void OnClosingWithNoResult(CancelEventArgs e) => SetResult(false);

    protected virtual void OnClosingWithCancelResult(CancelEventArgs e) => e.Cancel = true;

    #region Cancel

    protected virtual async Task<bool> CanCancelAsync(CancellationToken cancellationToken = default)
        => !this.IsModified() || await _dialogService.ShowQuestionAsync(MessageResources.ItemModificationCancellingQuestion, UiResources.Edition).ConfigureAwait(false) == MessageBoxResult.Yes;

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

    protected bool Save(CancellationToken cancellationToken = default)
        => SaveAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();

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

    protected virtual void ShowValidationErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
            _notificationPublisher.Publish(new MessageNotification(error, severity: NotificationSeverity.Error));
    }

    protected abstract void SaveCore();

    protected virtual Task SaveCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SaveCore();
        return Task.CompletedTask;
    }

    protected virtual void OnSaveSucceeded() { }

    protected virtual void OnSaveFailed(IReadOnlyCollection<string> errors) { }

    protected virtual void OnSaveRequested(CancelEventArgs args)
    {
    }

    protected virtual bool CanSave() => true;

    #endregion Save

    #region SaveAndClose

    protected virtual bool SaveAndClose(CancellationToken cancellationToken = default)
    {
        if (!Save(cancellationToken))
            return false;

        _closingByCommand = true;
        Close(true);
        return true;
    }

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
