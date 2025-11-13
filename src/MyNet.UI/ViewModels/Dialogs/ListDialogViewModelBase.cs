// -----------------------------------------------------------------------
// <copyright file="ListDialogViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Windows.Input;
using MyNet.Observable.Attributes;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Dialogs;

[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public abstract class ListDialogViewModelBase<T, TListViewModel> : WorkspaceDialogViewModel
    where T : notnull
    where TListViewModel : IListViewModel<T>
{
    protected ListDialogViewModelBase(TListViewModel list)
    {
        List = list;
        CancelCommand = CommandsManager.Create(Cancel, CanCancel);
        ValidateCommand = CommandsManager.Create(Validate, CanValidate);
    }

    public TListViewModel List { get; }

    public ICommand ValidateCommand { get; }

    public ICommand CancelCommand { get; }

    protected virtual void Cancel() => Close(false);

    protected virtual void Validate() => Close(true);

    protected virtual bool CanValidate() => true;

    protected virtual bool CanCancel() => true;

    protected override void RefreshCore() => List.Refresh();
}
