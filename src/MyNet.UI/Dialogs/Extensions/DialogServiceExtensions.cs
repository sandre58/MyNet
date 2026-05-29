// -----------------------------------------------------------------------
// <copyright file="DialogServiceExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Locators;

#pragma warning disable IDE0130
namespace MyNet.UI.Dialogs;
#pragma warning restore IDE0130

/// <summary>
/// Extension methods that provide DI-friendly helpers around <see cref="IDialogService"/>.
/// They replace the need for a global static dialog manager.
/// </summary>
public static class DialogServiceExtensions
{
    extension(IDialogService dialogService)
    {
        /// <summary>
        /// Resolves a dialog instance from the view-model locator and shows it.
        /// </summary>
        /// <typeparam name="TDialog">The dialog type to resolve and show.</typeparam>
        /// <param name="viewModelLocator">The locator used to resolve the dialog.</param>
        /// <param name="options">Optional dialog options.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A non-typed dialog result.</returns>
        public Task<DialogResult<bool>> ShowAsync<TDialog>(IViewModelLocator viewModelLocator,
            DialogOptions? options = null,
            CancellationToken cancellationToken = default)
            where TDialog : class, IDialog
            => dialogService.ShowAsync(ResolveFromLocator<TDialog>(dialogService, viewModelLocator), options, cancellationToken);

        /// <summary>
        /// Resolves a typed dialog instance from the view-model locator and shows it.
        /// </summary>
        /// <typeparam name="TDialog">The typed dialog type to resolve and show.</typeparam>
        /// <typeparam name="TResult">The typed result value returned by the dialog.</typeparam>
        /// <param name="viewModelLocator">The locator used to resolve the dialog.</param>
        /// <param name="options">Optional dialog options.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A strongly-typed dialog result.</returns>
        public Task<DialogResult<TResult>> ShowAsync<TDialog, TResult>(IViewModelLocator viewModelLocator,
            DialogOptions? options = null,
            CancellationToken cancellationToken = default)
            where TDialog : class, IDialog<TResult>
            => dialogService.ShowAsync(ResolveFromLocator<TDialog>(dialogService, viewModelLocator), options, cancellationToken);

        /// <summary>
        /// Creates a non-typed dialog builder after resolving the dialog from the view-model locator.
        /// </summary>
        public IDialogBuilder Create<TDialog>(IViewModelLocator viewModelLocator)
            where TDialog : class, IDialog
            => dialogService.Create(ResolveFromLocator<TDialog>(dialogService, viewModelLocator));

        /// <summary>
        /// Creates a typed dialog builder after resolving the dialog from the view-model locator.
        /// </summary>
        public IDialogBuilder<TResult> Create<TDialog, TResult>(IViewModelLocator viewModelLocator)
            where TDialog : class, IDialog<TResult>
            => dialogService.Create(ResolveFromLocator<TDialog>(dialogService, viewModelLocator));
    }

    private static TDialog ResolveFromLocator<TDialog>(IDialogService dialogService, IViewModelLocator viewModelLocator)
        where TDialog : class
    {
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(viewModelLocator);

        var dialog = viewModelLocator.Get<TDialog>();

        return dialog ?? throw new InvalidOperationException($"Dialog type '{typeof(TDialog).FullName}' could not be resolved by IViewModelLocator.");
    }

    /// <summary>
    /// Resolves a dialog instance of type <typeparamref name="TDialog"/> from the provided <paramref name="serviceProvider"/>.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <typeparam name="TDialog">The type of the dialog to resolve.</typeparam>
    /// <returns>The resolved dialog instance.</returns>
    private static TDialog ResolveFromServiceProvider<TDialog>(IDialogService dialogService, IServiceProvider serviceProvider)
        where TDialog : class
    {
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var dialog = serviceProvider.GetService(typeof(TDialog)) as TDialog;

        return dialog ?? throw new InvalidOperationException($"Dialog type '{typeof(TDialog).FullName}' is not registered in the service provider.");
    }
}
