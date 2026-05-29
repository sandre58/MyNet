// -----------------------------------------------------------------------
// <copyright file="FileExportViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentValidation;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.FileDialogs;
using MyNet.UI.Notifications;

namespace MyNet.UI.ViewModels.Export;

/// <summary>
/// Provides a reusable base implementation for file export dialogs.
/// </summary>
/// <typeparam name="T">The exported item type.</typeparam>
public abstract class FileExportViewModelBase<T> : ExportViewModelBase<T>
{
    private readonly IDialogService _dialogService;
    private readonly string _defaultFolder;
    private readonly Func<string> _defaultExportName;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileExportViewModelBase{T}"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service used to select a destination file.</param>
    /// <param name="fileType">The supported file type constraints.</param>
    /// <param name="defaultExportName">Function used to generate default file name (without extension).</param>
    /// <param name="defaultFolder">Optional default destination folder.</param>
    /// <param name="notificationPublisher">Optional notification publisher used to display validation/export errors.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    /// <param name="validator">Optional validator used to validate this view model.</param>
    protected FileExportViewModelBase(
        IDialogService dialogService,
        ExportFileType fileType,
        Func<string> defaultExportName,
        string? defaultFolder = null,
        INotificationPublisher? notificationPublisher = null,
        ICommandFactory? commandFactory = null,
        IValidator? validator = null)
        : base(notificationPublisher, commandFactory, validator ?? new FileExportViewModelValidator<T>())
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        FileType = fileType ?? throw new ArgumentNullException(nameof(fileType));
        _defaultExportName = defaultExportName ?? throw new ArgumentNullException(nameof(defaultExportName));
        _defaultFolder = string.IsNullOrWhiteSpace(defaultFolder)
            ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            : defaultFolder;

        var commands = commandFactory ?? RelayCommandFactory.Default;
        SetFilePathCommand = commands.Create(() => SetFilePathAsync());
    }

    /// <summary>
    /// Gets file type constraints.
    /// </summary>
    public ExportFileType FileType { get; }

    /// <summary>
    /// Gets or sets destination file path.
    /// </summary>
    public string? Destination { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the command that lets the user select a destination path.
    /// </summary>
    public ICommand SetFilePathCommand { get; }

    /// <inheritdoc />
    public override void Load(ICollection<T> items)
    {
        base.Load(items);
        var directory = Path.GetDirectoryName(Destination) ?? _defaultFolder;
        Destination = Path.Combine(directory, Path.ChangeExtension(_defaultExportName(), FileType.DefaultExtension));
    }

    private async Task SetFilePathAsync(CancellationToken cancellationToken = default)
    {
        var settings = new SaveFileDialogSettings
        {
            FileName = Path.GetFileNameWithoutExtension(Destination) ?? string.Empty,
            InitialDirectory = GetInitialDirectory(),
            Filters = FileType.DialogFilter,
            DefaultExtension = FileType.DefaultExtension
        };

        var result = await _dialogService.ShowSaveFileDialogAsync(settings, cancellationToken).ConfigureAwait(false);

        if (result is { IsCancelled: false, Files.Count: > 0 })
            Destination = result.Files[0];
    }

    private string GetInitialDirectory()
    {
        if (!string.IsNullOrWhiteSpace(Destination))
        {
            var directory = Path.GetDirectoryName(Destination);
            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
                return directory;
        }

        return _defaultFolder;
    }
}
