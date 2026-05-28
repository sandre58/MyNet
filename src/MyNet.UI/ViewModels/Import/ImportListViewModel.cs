// -----------------------------------------------------------------------
// <copyright file="ImportListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MyNet.UI.Commands;

namespace MyNet.UI.ViewModels.Import;

/// <summary>
/// Provides list operations for import items.
/// </summary>
/// <typeparam name="T">The import item type.</typeparam>
public sealed class ImportListViewModel<T> : ViewModelBase
    where T : ImportItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImportListViewModel{T}"/> class.
    /// </summary>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    public ImportListViewModel(ICommandFactory? commandFactory = null)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;
        ImportSelectionCommand = commands.Create(ImportSelection, CanApplyToSelection);
        DoNotImportSelectionCommand = commands.Create(DoNotImportSelection, CanApplyToSelection);
    }

    /// <summary>
    /// Gets items available for import.
    /// </summary>
    public ObservableCollection<T> Items { get; } = [];

    /// <summary>
    /// Gets items marked for import.
    /// </summary>
    public IReadOnlyCollection<T> ImportItems => [.. Items.Where(x => x.Import)];

    /// <summary>
    /// Gets the command that marks selected items for import.
    /// </summary>
    public ICommand ImportSelectionCommand { get; }

    /// <summary>
    /// Gets the command that marks selected items as not imported.
    /// </summary>
    public ICommand DoNotImportSelectionCommand { get; }

    /// <summary>
    /// Replaces list items.
    /// </summary>
    public void Load(IEnumerable<T> items)
    {
        Items.Clear();

        foreach (var item in items)
            Items.Add(item);

        RaiseSelectionCommandsCanExecuteChanged();
    }

    /// <summary>
    /// Marks selected items for import.
    /// </summary>
    public void ImportSelection() => ApplyOnSelection(x => x.Import = true);

    /// <summary>
    /// Marks selected items as not imported.
    /// </summary>
    public void DoNotImportSelection() => ApplyOnSelection(x => x.Import = false);

    private bool CanApplyToSelection() => Items.Any(x => x.IsSelected);

    private void ApplyOnSelection(System.Action<T> action)
    {
        foreach (var item in Items.Where(x => x.IsSelected))
            action(item);
    }

    private void RaiseSelectionCommandsCanExecuteChanged()
    {
        (ImportSelectionCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
        (DoNotImportSelectionCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
    }
}
