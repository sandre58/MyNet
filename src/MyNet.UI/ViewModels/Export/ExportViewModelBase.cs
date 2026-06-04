// -----------------------------------------------------------------------
// <copyright file="ExportViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentValidation;
using MyNet.Observable;
using MyNet.Observable.Behaviors;
using MyNet.UI.Commands;
using MyNet.UI.Notifications;
using MyNet.UI.ViewModels.Dialog;

namespace MyNet.UI.ViewModels.Export;

/// <summary>
/// Provides a reusable base implementation for export dialogs.
/// </summary>
/// <typeparam name="T">The exported item type.</typeparam>
public abstract class ExportViewModelBase<T> : DialogViewModel<bool>
{
    private readonly INotificationPublisher? _notificationPublisher;
    private ICollection<T>? _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportViewModelBase{T}"/> class.
    /// </summary>
    /// <param name="notificationPublisher">Optional notification publisher used to display validation/export errors.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    /// <param name="validator">Optional validator used to validate this view model.</param>
    protected ExportViewModelBase(
        INotificationPublisher? notificationPublisher = null,
        ICommandFactory? commandFactory = null,
        IValidator? validator = null)
        : base(commandFactory)
    {
        _notificationPublisher = notificationPublisher;
        ExportAndCloseCommand = Commands.Create(() => ExportAndCloseAsync());
        CancelCommand = Commands.Create(Cancel);

        this.UseTracking()
            .UseValidation(validator ?? new ExportViewModelValidator<T>());
    }

    /// <summary>
    /// Gets or sets a value indicating whether configuration should be saved before export.
    /// </summary>
    public bool SaveConfigurationOnValidate { get; set; } = true;

    /// <summary>
    /// Gets the number of items loaded for export.
    /// </summary>
    public int ExportItemCount { get; private set => SetProperty(ref field, value); }

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
    /// Gets the command that exports and closes the dialog.
    /// </summary>
    public ICommand ExportAndCloseCommand { get; }

    /// <summary>
    /// Gets the command that cancels the dialog.
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <summary>
    /// Loads items to export.
    /// </summary>
    /// <param name="items">The items to export.</param>
    public virtual void Load(ICollection<T> items)
    {
        _items = items;
        ExportItemCount = items.Count;
    }

    /// <summary>
    /// Saves the current export configuration.
    /// </summary>
    protected virtual void SaveConfiguration()
    {
    }

    /// <summary>
    /// Exports provided items.
    /// </summary>
    /// <param name="items">The items to export.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> when export succeeds; otherwise <see langword="false"/>.</returns>
    protected abstract Task<bool> ExportItemsAsync(ICollection<T> items, CancellationToken cancellationToken = default);

    private async Task ExportAndCloseAsync(CancellationToken cancellationToken = default)
    {
        if (!this.TryValidateAndNotify(_notificationPublisher))
            return;

        if (SaveConfigurationOnValidate)
            SaveConfiguration();

        var items = _items ?? [];
        var success = await ExportItemsAsync(items, cancellationToken).ConfigureAwait(false);

        if (success)
            Close(true);
    }

    private void Cancel()
    {
        SetCancelled();
        RequestClose();
    }
}
