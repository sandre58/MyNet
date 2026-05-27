// -----------------------------------------------------------------------
// <copyright file="ListDialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.List;

namespace MyNet.UI.ViewModels.Dialog;

/// <summary>
/// Provides a base implementation for dialog view models that display a list of items and allow the user to select one or more items from the list. The selected items are returned as a read-only collection when the dialog is closed with validation.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
/// <typeparam name="TList">The type of the list view model.</typeparam>
public class ListDialogViewModel<T, TList> : DialogViewModel<IReadOnlyCollection<T>>
    where T : notnull
    where TList : IListViewModel<T>
{
    /// <summary>
    /// Gets the list view model containing the items to be displayed in the dialog. This property is initialized through the constructor and provides access to the list of items that the user can select from. The selected items will be returned when the dialog is closed with validation.
    /// </summary>
    public TList List { get; }

    /// <summary>
    /// Gets the command to execute when the user validates their selection and closes the dialog. This command is initialized through the constructor and is responsible for closing the dialog and returning the selected items from the list as a read-only collection. The command can only execute if the CanValidate method returns true, which can be overridden in derived classes to implement custom validation logic based on the state of the list or the user's selection.
    /// </summary>
    public ICommand ValidateCommand { get; }

    /// <summary>
    /// Gets the command to execute when the user cancels the dialog. This command is initialized through the constructor and is responsible for closing the dialog without returning any selected items. When executed, it sets the dialog's state to cancelled and requests the dialog to close. The command can only execute if the CanCancel method returns true, which can be overridden in derived classes to implement custom logic for determining whether cancellation is allowed based on the current state of the dialog or the user's interactions.
    /// </summary>
    public ICommand CancelCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListDialogViewModel{T, TList}"/> class with the specified list view model and an optional command factory. The constructor sets up the Validate and Cancel commands using the provided command factory or a default relay command factory if none is provided. The Validate command is configured to execute the Validate method and can only execute if the CanValidate method returns true, while the Cancel command is configured to execute the Cancel method without any execution constraints.
    /// </summary>
    /// <param name="list">The list view model containing the items to be displayed in the dialog.</param>
    /// <param name="commandFactory">An optional command factory to create commands.</param>
    /// <exception cref="ArgumentNullException">Thrown if the list parameter is null.</exception>
    public ListDialogViewModel(TList list, ICommandFactory? commandFactory = null)
        : base(commandFactory)
    {
        ArgumentNullException.ThrowIfNull(list);
        var commands = commandFactory ?? RelayCommandFactory.Default;

        List = list;

        ValidateCommand = commands.Create(Validate, CanValidate);
        CancelCommand = commands.Create(Cancel);
    }

    /// <summary>
    /// Validates the user's selection and closes the dialog, returning the selected items from the list as a read-only collection. This method is called when the Validate command is executed. It retrieves the selected items from the List property and passes them to the Close method to close the dialog and return the selection result. Derived classes can override this method to implement custom validation logic or to modify how the selected items are retrieved from the list before closing the dialog.
    /// </summary>
    protected virtual void Validate() => Close([.. List.Items]);

    /// <summary>
    /// Cancels the dialog without returning any selected items. This method is called when the Cancel command is executed. It sets the dialog's state to cancelled by calling the SetCancelled method and then requests the dialog to close by calling the RequestClose method. Derived classes can override this method to implement custom logic for handling cancellation, such as prompting the user for confirmation before closing the dialog or performing cleanup operations before closing.
    /// </summary>
    protected virtual void Cancel()
    {
        SetCancelled();
        RequestClose();
    }

    /// <summary>
    /// Determines whether the Validate command can execute. This method returns true if the Validate command can be executed, allowing the user to validate their selection and close the dialog. Derived classes can override this method to implement custom validation logic based on the state of the list or the user's selection. If this method returns false, the Validate command will be disabled, preventing the user from closing the dialog without meeting the necessary validation criteria.
    /// </summary>
    /// <returns>True if the Validate command can execute; otherwise, false.</returns>
    protected virtual bool CanValidate() => true;

    /// <summary>
    /// Raises <see cref="ICommand.CanExecuteChanged"/> for <see cref="ValidateCommand"/>.
    /// </summary>
    protected void RaiseValidateCanExecuteChanged() => (ValidateCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
}
