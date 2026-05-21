// -----------------------------------------------------------------------
// <copyright file="DialogBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Default implementation of <see cref="IDialogBuilder"/> for non-typed dialogs.
/// </summary>
/// <param name="service">The content dialog service used to display the dialog.</param>
/// <param name="dialog">The dialog view model to be displayed.</param>
public class DialogBuilder(IContentDialogService service, IDialog dialog) : IDialogBuilder
{
    private readonly DialogOptions _options = new() { Dialog = dialog };

    /// <inheritdoc />
    public IDialogBuilder AsModal()
    {
        _options.IsModal = true;
        return this;
    }

    /// <inheritdoc />
    public IDialogBuilder AsNonModal()
    {
        _options.IsModal = false;
        return this;
    }

    /// <inheritdoc />
    public IDialogBuilder WithTitle(string title)
    {
        _options.Title = title;
        return this;
    }

    /// <inheritdoc />
    public IDialogBuilder WithOwner(object owner)
    {
        _options.Owner = owner;
        return this;
    }

    /// <inheritdoc />
    public IDialogBuilder CloseOnOverlayClick(bool value = true)
    {
        _options.CloseOnOverlayClick = value;
        return this;
    }

    /// <inheritdoc />
    public Task<DialogResult<bool>> ShowAsync(CancellationToken cancellationToken = default)
        => service.ShowAsync(dialog, _options, cancellationToken);
}

/// <summary>
/// Default implementation of <see cref="IDialogBuilder{TResult}"/> for typed dialogs.
/// </summary>
/// <typeparam name="TResult">The type of the value returned by the dialog.</typeparam>
/// <param name="service">The content dialog service used to display the dialog.</param>
/// <param name="dialog">The typed dialog view model to be displayed.</param>
public sealed class DialogBuilder<TResult>(IContentDialogService service, IDialog<TResult> dialog) : IDialogBuilder<TResult>
{
    private readonly DialogOptions _options = new() { Dialog = dialog };

    /// <inheritdoc />
    public IDialogBuilder<TResult> AsModal()
    {
        _options.IsModal = true;
        return this;
    }

    /// <inheritdoc />
    IDialogBuilder IDialogBuilder.AsModal() => AsModal();

    /// <inheritdoc />
    public IDialogBuilder<TResult> AsNonModal()
    {
        _options.IsModal = false;
        return this;
    }

    /// <inheritdoc />
    IDialogBuilder IDialogBuilder.AsNonModal() => AsNonModal();

    /// <inheritdoc />
    public IDialogBuilder<TResult> WithTitle(string title)
    {
        _options.Title = title;
        return this;
    }

    /// <inheritdoc />
    IDialogBuilder IDialogBuilder.WithTitle(string title) => WithTitle(title);

    /// <inheritdoc />
    public IDialogBuilder<TResult> WithOwner(object owner)
    {
        _options.Owner = owner;
        return this;
    }

    /// <inheritdoc />
    IDialogBuilder IDialogBuilder.WithOwner(object owner) => WithOwner(owner);

    /// <inheritdoc />
    public IDialogBuilder<TResult> CloseOnOverlayClick(bool value = true)
    {
        _options.CloseOnOverlayClick = value;
        return this;
    }

    /// <inheritdoc />
    IDialogBuilder IDialogBuilder.CloseOnOverlayClick(bool value) => CloseOnOverlayClick(value);

    /// <inheritdoc />
    // Call the non-typed overload so the return type matches Task<DialogResult<bool>>.
    Task<DialogResult<bool>> IDialogBuilder.ShowAsync(CancellationToken cancellationToken)
        => service.ShowAsync((IDialog)dialog, _options, cancellationToken);

    /// <inheritdoc />
    public Task<DialogResult<TResult>> ShowAsync(CancellationToken cancellationToken = default)
        => service.ShowAsync(dialog, _options, cancellationToken);
}
