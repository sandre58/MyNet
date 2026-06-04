// -----------------------------------------------------------------------
// <copyright file="ItemEditionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentValidation;
using MyNet.Globalization.Culture;
using MyNet.Observable;
using MyNet.Observable.Validation.Validators;
using MyNet.UI.Commands;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Provides a reusable base implementation for editable item view models.
/// </summary>
/// <typeparam name="T">The edited item type.</typeparam>
public abstract class ItemEditionViewModel<T> : ItemViewModel<T>, IItemEditionViewModel<T>, IEditionStateViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemEditionViewModel{T}"/> class.
    /// </summary>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    /// <param name="validator">Optional validator used to validate the item edition view model.</param>
    /// <param name="cultureService">Optional culture service used to manage culture changes.</param>
    protected ItemEditionViewModel(
        ICommandFactory? commandFactory = null,
        IValidator<ItemEditionViewModel<T>>? validator = null,
        ICultureService? cultureService = null)
        : base(commandFactory, cultureService)
    {
        var commands = commandFactory.GetOrDefault();

        ApplyCommand = commands.Create(() => ApplyAsync(), CanApply);

        this.UseTracking()
            .UseValidation((IValidator?)validator ?? EmptyValidator.Instance);
    }

    /// <summary>
    /// Gets the command to apply the current changes.
    /// </summary>
    public ICommand ApplyCommand { get; }

    /// <summary>
    /// Gets the original item snapshot.
    /// </summary>
    public T? OriginalItem
    {
        get;
        private set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets a value indicating whether the edited item differs from the original snapshot.
    /// </summary>
    public bool IsDirty
    {
        get;
        private set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Sets the original item and initializes the current editable item from it.
    /// </summary>
    /// <param name="item">The original item.</param>
    public virtual void SetOriginalItem(T? item)
    {
        OriginalItem = CloneItem(item);
        SetItem(CloneItem(item));
        UpdateIsDirty();
    }

    /// <summary>
    /// Restores the current item from the original snapshot asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override Task OnResetAsync(CancellationToken cancellationToken = default)
    {
        ResetItem();
        UpdateIsDirty();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Restores the current item from the original snapshot.
    /// </summary>
    protected virtual void ResetItem() => SetItem(CloneItem(OriginalItem));

    /// <summary>
    /// Applies the changes to the item. By default, it updates the original snapshot with the current item state and recomputes the dirty state.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        OriginalItem = CloneItem(Item);
        UpdateIsDirty();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates an editable copy of the specified item.
    /// </summary>
    /// <param name="item">The item to clone.</param>
    /// <returns>A copy suitable for edition, or the original instance when cloning is not available.</returns>
    protected virtual T? CloneItem(T? item)
        => item switch
        {
            null => default,
            ICloneable cloneable => (T?)cloneable.Clone(),
            _ => item
        };

    /// <inheritdoc />
    protected override void HandleItemChanged()
    {
        base.HandleItemChanged();
        UpdateIsDirty();
    }

    /// <inheritdoc />
    protected override void HandleCurrentItemPropertyChanged(PropertyChangedEventArgs e)
    {
        base.HandleCurrentItemPropertyChanged(e);
        UpdateIsDirty();
    }

    /// <summary>
    /// Determines whether the item can be reset.
    /// </summary>
    /// <returns><see langword="true"/> when the item can be reset; otherwise <see langword="false"/>.</returns>
    protected override bool CanReset() => IsDirty;

    /// <summary>
    /// Determines whether the changes can be applied.
    /// </summary>
    /// <returns><see langword="true"/> when changes can be applied; otherwise <see langword="false"/>.</returns>
    protected virtual bool CanApply() => IsDirty;

    /// <summary>
    /// Recomputes the dirty state.
    /// </summary>
    protected void UpdateIsDirty() => IsDirty = !Equals(Item, OriginalItem);
}
