// -----------------------------------------------------------------------
// <copyright file="IFileDialogBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Fluent builder for file dialog settings and display.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
/// <typeparam name="TSettings">The dialog settings type.</typeparam>
public interface IFileDialogBuilder<out TBuilder, out TSettings>
    where TBuilder : IFileDialogBuilder<TBuilder, TSettings>
    where TSettings : FileDialogSettings
{
    /// <summary>
    /// Sets the file name shown in the dialog.
    /// </summary>
    TBuilder WithFileName(string fileName);

    /// <summary>
    /// Sets the initial directory shown in the dialog.
    /// </summary>
    TBuilder WithInitialDirectory(string initialDirectory);

    /// <summary>
    /// Sets the file type filter string.
    /// </summary>
    TBuilder WithFilters(string? filters);

    /// <summary>
    /// Sets the default file extension.
    /// </summary>
    TBuilder WithDefaultExtension(string defaultExtension);

    /// <summary>
    /// Sets the dialog title.
    /// </summary>
    TBuilder WithTitle(string title);

    /// <summary>
    /// Builds the settings instance without showing the dialog.
    /// </summary>
    TSettings Build();

    /// <summary>
    /// Shows the dialog asynchronously.
    /// </summary>
    Task<FileDialogResult> PickAsync(CancellationToken cancellationToken = default);
}
