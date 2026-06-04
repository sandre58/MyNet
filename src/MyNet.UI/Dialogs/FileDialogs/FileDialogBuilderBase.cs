// -----------------------------------------------------------------------
// <copyright file="FileDialogBuilderBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.FileDialogs;

/// <summary>
/// Base implementation for fluent file dialog builders.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
/// <typeparam name="TSettings">The dialog settings type.</typeparam>
public abstract class FileDialogBuilderBase<TBuilder, TSettings> : IFileDialogBuilder<TBuilder, TSettings>
    where TBuilder : FileDialogBuilderBase<TBuilder, TSettings>
    where TSettings : FileDialogSettings, new()
{
    /// <summary>
    /// Gets the settings being configured.
    /// </summary>
    protected TSettings Settings { get; } = new();

    /// <inheritdoc />
    public TBuilder WithFileName(string fileName)
    {
        Settings.FileName = fileName;
        return (TBuilder)this;
    }

    /// <inheritdoc />
    public TBuilder WithInitialDirectory(string initialDirectory)
    {
        Settings.InitialDirectory = initialDirectory;
        return (TBuilder)this;
    }

    /// <inheritdoc />
    public TBuilder WithFilters(string? filters)
    {
        Settings.Filters = filters;
        return (TBuilder)this;
    }

    /// <inheritdoc />
    public TBuilder WithDefaultExtension(string defaultExtension)
    {
        Settings.DefaultExtension = defaultExtension;
        return (TBuilder)this;
    }

    /// <inheritdoc />
    public TBuilder WithTitle(string title)
    {
        Settings.Title = title;
        return (TBuilder)this;
    }

    /// <inheritdoc />
    public TSettings Build() => Settings;

    /// <inheritdoc />
    public abstract Task<FileDialogResult> PickAsync(CancellationToken cancellationToken = default);
}
