// -----------------------------------------------------------------------
// <copyright file="ImportDialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using FluentValidation;
using MyNet.Observable;
using MyNet.Observable.Behaviors;
using MyNet.UI.Commands;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.ViewModels.Dialog;

namespace MyNet.UI.ViewModels.Import;

/// <summary>
/// Provides a dialog workflow for importing selected items.
/// </summary>
/// <typeparam name="T">The import item type.</typeparam>
public class ImportDialogViewModel<T> : DialogViewModel<IReadOnlyCollection<T>>
    where T : ImportItemViewModel
{
    private readonly INotificationPublisher? _notificationPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportDialogViewModel{T}"/> class.
    /// </summary>
    /// <param name="list">The import list view model.</param>
    /// <param name="notificationPublisher">Optional notification publisher used to display validation errors.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    /// <param name="validator">Optional validator used to validate this view model.</param>
    public ImportDialogViewModel(
        ImportListViewModel<T> list,
        INotificationPublisher? notificationPublisher = null,
        ICommandFactory? commandFactory = null,
        IValidator? validator = null)
        : base(commandFactory)
    {
        List = list;
        _notificationPublisher = notificationPublisher;

        var commands = commandFactory ?? RelayCommandFactory.Default;
        ValidateCommand = commands.Create(ValidateAndClose, CanValidate);
        CancelCommand = commands.Create(Cancel);

        this.UseTracking()
            .UseValidation(validator ?? new ImportDialogViewModelValidator<T>());
    }

    /// <summary>
    /// Gets the import list view model.
    /// </summary>
    public ImportListViewModel<T> List { get; }

    /// <summary>
    /// Gets the number of items marked for import.
    /// </summary>
    public int ImportItemCount => List.ImportItems.Count;

    /// <summary>
    /// Gets validation error messages from the registered <see cref="IValidationBehavior"/>.
    /// </summary>
    public IReadOnlyCollection<string> Errors =>
        Behaviors.TryGet<IValidationBehavior>(out var behavior) ? behavior.Errors : [];

    /// <summary>
    /// Gets a value indicating whether <see cref="Errors"/> contains at least one message.
    /// </summary>
    public bool HasErrors => Errors.Count > 0;

    /// <summary>
    /// Gets the command that validates and closes the dialog.
    /// </summary>
    public ICommand ValidateCommand { get; }

    /// <summary>
    /// Gets the command that cancels and closes the dialog.
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <summary>
    /// Determines whether validation can execute.
    /// </summary>
    protected virtual bool CanValidate() => ImportItemCount > 0;

    /// <summary>
    /// Displays validation errors to the user.
    /// </summary>
    /// <param name="errors">The validation errors to display.</param>
    protected virtual void ShowValidationErrors(IEnumerable<string> errors)
    {
        if (_notificationPublisher is null)
            return;

        foreach (var error in errors.Where(x => !string.IsNullOrWhiteSpace(x)))
            _notificationPublisher.Publish(new MessageNotification(error, severity: NotificationSeverity.Error));
    }

    /// <summary>
    /// Validates the view model using the registered <see cref="IValidationBehavior"/>.
    /// </summary>
    /// <returns><see langword="true"/> when validation succeeds; otherwise <see langword="false"/>.</returns>
    protected bool TryValidate() =>
        Behaviors.TryGet<IValidationBehavior>(out var validationBehavior) && validationBehavior.Validate();

    private void ValidateAndClose()
    {
        if (!TryValidate())
        {
            ShowValidationErrors(Errors);
            return;
        }

        Close(List.ImportItems);
    }

    private void Cancel()
    {
        SetCancelled();
        RequestClose();
    }
}
