// -----------------------------------------------------------------------
// <copyright file="SelectionDialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Windows.Input;
using MyNet.Observable.Collections.Selection;
using MyNet.UI.Commands;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.List.Selection;

namespace MyNet.UI.ViewModels.Dialog;

/// <summary>
/// ViewModel for a dialog allowing to select one or more items from a list. The selected items are returned when the dialog is closed with the Validate command.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public class SelectionDialogViewModel<T> : ListDialogViewModel<T, SelectableListViewModel<T>>
    where T : notnull
{
    /// <summary>
    /// Gets the command to execute when an item is double-clicked in the list. This command will validate the selection and close the dialog if the selection is valid (i.e., at least one item is selected and the selection mode is single).
    /// </summary>
    public ICommand DoubleClickCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionDialogViewModel{T}"/> class with the specified selectable list and an optional command factory. The constructor sets up the double-click command to validate the selection when an item is double-clicked, but only if there is at least one selected item and the selection mode is single.
    /// </summary>
    /// <param name="list">The selectable list to be displayed in the dialog.</param>
    /// <param name="busyService">Optional busy service used to manage loading state.</param>
    /// <param name="commandFactory">An optional command factory to create commands.</param>
    public SelectionDialogViewModel(SelectableListViewModel<T> list, IBusyService? busyService = null, ICommandFactory? commandFactory = null)
        : base(list, busyService, commandFactory)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;
        DoubleClickCommand = commands.Create(Validate, () => List is { SelectedCount: > 0, SelectionMode: SelectionMode.Single });
    }

    /// <summary>
    /// Validates the selection and closes the dialog, returning the selected items. This method is called when the Validate command is executed or when an item is double-clicked in the list (if the selection mode is single and at least one item is selected). It retrieves the selected items from the list and passes them to the Close method to close the dialog and return the selection result.
    /// </summary>
    protected override void Validate() => Close([.. List.SelectedItems]);

    /// <summary>
    /// Determines whether the Validate command can execute. This method returns true if there is at least one selected item in the list, allowing the user to validate their selection and close the dialog. If no items are selected, this method returns false, preventing the Validate command from executing and ensuring that the user cannot close the dialog without making a selection.
    /// </summary>
    /// <returns>True if the Validate command can execute; otherwise, false.</returns>
    protected override bool CanValidate() => List.SelectedCount > 0;
}
