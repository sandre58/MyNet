// -----------------------------------------------------------------------
// <copyright file="ContentDialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNet.UI.ViewModels;

namespace MyNet.UI.Dialogs.ContentDialogs;

public class ContentDialogService(IEnumerable<IDialogStrategy> strategies) : IContentDialogService
{
    private readonly List<IDialogViewModel> _openedDialogs = [];

    /// <inheritdoc />
    public IList<IDialogViewModel> OpenedDialogs => _openedDialogs;

    public async Task<bool?> ShowAsync<TViewModel>(
        TViewModel viewModel,
        DialogOptions? options = null)
        where TViewModel : IDialogViewModel
    {
        var strategy = strategies
                           .FirstOrDefault(x => x.CanHandle(viewModel, options))
                       ?? throw new InvalidOperationException("No dialog strategy found.");

        _openedDialogs.Add(viewModel);

        await strategy.ShowAsync(viewModel, options).ConfigureAwait(false);

        if (viewModel is IDialogViewModel<bool?> boolDialog)
            return await boolDialog.Result.Task.ConfigureAwait(false);

        return null;
    }

    /// <inheritdoc />
    public async Task<bool?> ShowAsync<T>(T viewModel, Action<T>? closeAction)
        where T : IDialogViewModel
    {
        var result = await ShowAsync(viewModel, options: null).ConfigureAwait(false);
        closeAction?.Invoke(viewModel);
        return result;
    }

    /// <inheritdoc />
    public Task<bool?> ShowModalAsync<T>(T viewModel)
        where T : IDialogViewModel
        => ShowAsync(viewModel, options: null);

    /// <inheritdoc />
    public async Task<bool?> CloseAsync(IDialogViewModel dialog)
    {
        var strategy = strategies
                           .FirstOrDefault(x => x.CanHandle(dialog, null))
                       ?? throw new InvalidOperationException("No dialog strategy found.");

        await strategy.CloseAsync(dialog).ConfigureAwait(false);
        _ = _openedDialogs.Remove(dialog);

        if (dialog is IDialogViewModel<bool?> boolDialog)
            return await boolDialog.Result.Task.ConfigureAwait(false);

        return null;
    }
}
