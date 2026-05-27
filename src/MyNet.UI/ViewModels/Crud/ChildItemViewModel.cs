// -----------------------------------------------------------------------
// <copyright file="ChildItemViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MyNet.UI.Commands;
using MyNet.UI.Loading;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Item view model that follows a parent <see cref="IItemViewModel{T}"/> and updates when the parent's item changes.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ChildItemViewModel{T}"/> class.
/// </remarks>
public abstract class ChildItemViewModel<T>(ICommandFactory? commandFactory = null) : ItemViewModel<T>(commandFactory)
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup.")]
    private IDisposable? _parentSubscription;

    /// <summary>
    /// Attaches to a parent item view model.
    /// </summary>
    public virtual void Attach(IItemViewModel<T> parent)
    {
        ArgumentNullException.ThrowIfNull(parent);

        Detach();
        SetItem(parent.Item);
        _parentSubscription = parent.ObserveItemChanged(OnParentItemChanged);
    }

    /// <summary>
    /// Detaches from the parent item view model.
    /// </summary>
    public virtual void Detach()
    {
        _parentSubscription?.Dispose();
        _parentSubscription = null;
    }

    /// <summary>
    /// Called when the parent's item changes.
    /// </summary>
    protected virtual void OnParentItemChanged(T? item) => SetItem(item);

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        Detach();
        base.DisposeManagedResources();
    }
}
