// -----------------------------------------------------------------------
// <copyright file="ImportDialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Input;
using FluentValidation;
using MyNet.Observable;
using MyNet.Observable.Behaviors;
using MyNet.UI.Commands;
using MyNet.UI.Notifications;
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

        ValidateCommand = Commands.Create(ValidateAndClose, CanValidate);
        CancelCommand = Commands.Create(Cancel);

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

    private void ValidateAndClose()
    {
        if (!this.TryValidateAndNotify(_notificationPublisher))
            return;

        Close(List.ImportItems);
    }

    private void Cancel()
    {
        SetCancelled();
        RequestClose();
    }
}
