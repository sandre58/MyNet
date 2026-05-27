// -----------------------------------------------------------------------
// <copyright file="ImportDialogViewModel.cs" company="St�phane ANDRE">
// Copyright (c) St�phane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DynamicData;
using MyNet.UI.Resources;
using MyNet.Primitives.Providers;

namespace MyNet.UI.Legacy.ViewModels.Import;

[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public class ImportDialogViewModel<T> : ImportDialogViewModel<T, ImportablesListViewModel<T>>
    where T : ImportableViewModel
{
    public ImportDialogViewModel(ICollection<T> itemsProvider,
        IListParametersProvider? parametersProvider = null,
        SelectionMode selectionMode = SelectionMode.Multiple,
        string? title = null)
        : this(new(itemsProvider, parametersProvider, selectionMode), title) { }

    public ImportDialogViewModel(IItemsProvider<T> itemsProvider,
        bool loadItems = true,
        IListParametersProvider? parametersProvider = null,
        SelectionMode selectionMode = SelectionMode.Multiple,
        string? title = null)
        : this(new(itemsProvider, loadItems, parametersProvider, selectionMode), title) { }

    public ImportDialogViewModel(ISourceProvider<T> itemsProvider,
        IListParametersProvider? parametersProvider = null,
        SelectionMode selectionMode = SelectionMode.Multiple,
        string? title = null)
        : this(new(itemsProvider, parametersProvider, selectionMode), title) { }

    public ImportDialogViewModel(IObservable<IChangeSet<T>> itemsProvider,
        IListParametersProvider? parametersProvider = null,
        SelectionMode selectionMode = SelectionMode.Multiple,
        string? title = null)
        : this(new(itemsProvider, parametersProvider, selectionMode), title) { }

    protected ImportDialogViewModel(ImportablesListViewModel<T> list, string? title = null)
        : base(list, title) { }
}

[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public abstract class ImportDialogViewModel<T, TListViewModel> : ListDialogViewModelBase<T, TListViewModel>
    where T : ImportableViewModel
    where TListViewModel : ImportablesListViewModel<T>
{
    protected ImportDialogViewModel(TListViewModel list, string? title = null)
        : base(list) => Title = title ?? UiResources.Import;

    protected override bool CanValidate() => List.ImportItems.Any();

    protected override void Validate()
    {
        if (List.ImportItems.Any(x => !x.ValidateProperties()))
        {
            GetValidationErrors().ToList().ForEach(x => ToasterManager.ShowError(x));
            return;
        }

        base.Validate();
    }
}
