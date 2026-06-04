// -----------------------------------------------------------------------
// <copyright file="FileExportByColumnsViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FluentValidation;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Notifications;

namespace MyNet.UI.ViewModels.Export;

/// <summary>
/// Provides a reusable base implementation for file exports with selectable columns.
/// </summary>
/// <typeparam name="T">The exported item type.</typeparam>
/// <typeparam name="TColumn">The column metadata type.</typeparam>
public abstract class FileExportByColumnsViewModelBase<T, TColumn> : FileExportViewModelBase<T>
{
    private readonly IReadOnlyDictionary<TColumn, bool> _defaultColumns;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileExportByColumnsViewModelBase{T, TColumn}"/> class.
    /// </summary>
    /// <param name="dialogService">The dialog service used to select a destination file.</param>
    /// <param name="fileType">The supported file type constraints.</param>
    /// <param name="defaultExportName">Function used to generate default file name (without extension).</param>
    /// <param name="defaultColumns">The default columns selection.</param>
    /// <param name="columnKeySelector">Function used to generate column keys.</param>
    /// <param name="presetColumns">Optional column presets.</param>
    /// <param name="defaultFolder">Optional default destination folder.</param>
    /// <param name="notificationPublisher">Optional notification publisher used to display validation/export errors.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    /// <param name="validator">Optional validator used to validate the view model.</param>
    protected FileExportByColumnsViewModelBase(
        IDialogService dialogService,
        ExportFileType fileType,
        Func<string> defaultExportName,
        IReadOnlyDictionary<TColumn, bool> defaultColumns,
        Func<TColumn, string> columnKeySelector,
        IEnumerable<ExportColumnsPreset>? presetColumns = null,
        string? defaultFolder = null,
        INotificationPublisher? notificationPublisher = null,
        ICommandFactory? commandFactory = null,
        IValidator? validator = null)
        : base(
            dialogService,
            fileType,
            defaultExportName,
            defaultFolder,
            notificationPublisher,
            commandFactory,
            validator ?? new FileExportByColumnsViewModelValidator<T, TColumn>())
    {
        ArgumentNullException.ThrowIfNull(defaultColumns);
        ArgumentNullException.ThrowIfNull(columnKeySelector);

        _defaultColumns = defaultColumns;
        ColumnKeySelector = columnKeySelector;

        Columns = [.. _defaultColumns.Select(x => new ExportColumnViewModel<TColumn>(x.Key, ColumnKeySelector(x.Key), x.Value))];
        PresetColumns = [.. presetColumns ?? []];

        SetSelectedColumnsCommand = Commands.Create<IReadOnlyCollection<string>>(x => SetSelectedColumns(x ?? []));
    }

    /// <summary>
    /// Gets or sets a value indicating whether column header translations should be exported.
    /// </summary>
    public bool ShowHeaderColumnTranslation { get; set; } = true;

    /// <summary>
    /// Gets column presets.
    /// </summary>
    public IReadOnlyCollection<ExportColumnsPreset> PresetColumns { get; }

    /// <summary>
    /// Gets selected columns wrappers.
    /// </summary>
    public ObservableCollection<ExportColumnViewModel<TColumn>> Columns { get; }

    /// <summary>
    /// Gets the command that applies a selected columns preset.
    /// </summary>
    public ICommand SetSelectedColumnsCommand { get; }

    /// <summary>
    /// Gets selector used to map a column to its key.
    /// </summary>
    protected Func<TColumn, string> ColumnKeySelector { get; }

    /// <summary>
    /// Gets selected columns metadata in display order.
    /// </summary>
    protected IReadOnlyCollection<TColumn> SelectedColumns => [.. Columns.Where(x => x.IsSelected).Select(x => x.Column)];

    /// <summary>
    /// Gets all columns metadata in display order.
    /// </summary>
    protected IReadOnlyCollection<TColumn> OrderedColumns => [.. Columns.Select(x => x.Column)];

    /// <summary>
    /// Applies selected columns by keys.
    /// </summary>
    /// <param name="columnKeys">The selected column keys.</param>
    protected void SetSelectedColumns(IEnumerable<string> columnKeys)
    {
        var selected = new HashSet<string>(columnKeys, StringComparer.Ordinal);
        foreach (var column in Columns)
            column.IsSelected = selected.Contains(column.Key);
    }

    /// <summary>
    /// Applies a new column order according to provided keys.
    /// </summary>
    /// <param name="columnKeys">The ordered keys.</param>
    protected void SetColumnsOrder(IEnumerable<string> columnKeys)
    {
        var newIndex = 0;
        foreach (var key in columnKeys)
        {
            var currentIndex = Columns.Select(x => x.Key).ToList().IndexOf(key);
            if (currentIndex <= -1)
                continue;

            Columns.Move(currentIndex, newIndex);
            newIndex++;
        }
    }

    /// <summary>
    /// Resets column order to default order.
    /// </summary>
    protected virtual void ResetColumnsOrder() => SetColumnsOrder(_defaultColumns.Keys.Select(ColumnKeySelector));
}

/// <summary>
/// Represents a named preset for selected export columns.
/// </summary>
public sealed record ExportColumnsPreset(string Name, IReadOnlyCollection<string> ColumnKeys);
