// -----------------------------------------------------------------------
// <copyright file="ImportablesListViewModel.cs" company="St�phane ANDRE">
// Copyright (c) St�phane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DynamicData;
using MyNet.UI.Commands;
using MyNet.Primitives.Providers;

namespace MyNet.UI.Legacy.ViewModels.Import;

[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public class ImportablesListViewModel<T> : SelectionListViewModel<T>
    where T : ImportableViewModel
{
    private readonly ReadOnlyObservableCollection<T> _importItems;

    public ImportablesListViewModel(ICollection<T> source, IListParametersProvider? parametersProvider = null, SelectionMode selectionMode = SelectionMode.Multiple)
        : this(SelectableCollectionFactory.FromCollection(source, selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent), parametersProvider) { }

    public ImportablesListViewModel(IItemsProvider<T> source, bool loadItems = true, IListParametersProvider? parametersProvider = null, SelectionMode selectionMode = SelectionMode.Multiple)
        : this(SelectableCollectionFactory.FromItemsProvider(source, loadItems, selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent), parametersProvider) { }

    public ImportablesListViewModel(ISourceProvider<T> source, IListParametersProvider? parametersProvider = null, SelectionMode selectionMode = SelectionMode.Multiple)
        : this(SelectableCollectionFactory.FromSourceProvider(source, selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent), parametersProvider) { }

    public ImportablesListViewModel(IObservable<IChangeSet<T>> source, IListParametersProvider? parametersProvider = null, SelectionMode selectionMode = SelectionMode.Multiple)
        : this(SelectableCollectionFactory.FromObservable(source, selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent), parametersProvider) { }

    public ImportablesListViewModel(IListParametersProvider? parametersProvider = null, SelectionMode selectionMode = SelectionMode.Multiple)
        : this(SelectableCollectionFactory.Empty<T>(selectionMode: selectionMode, scheduler: Scheduler.UiOrCurrent), parametersProvider) { }

    protected ImportablesListViewModel(
        SelectableCollection<T> collection,
        IListParametersProvider? parametersProvider = null)
        : base(collection, parametersProvider)
    {
        ImportSelectionCommand = RelayCommandFactory.Default.Create(() => ApplyOnSelection(y => y.Import = true));
        DoNotImportSelectionCommand = RelayCommandFactory.Default.Create(() => ApplyOnSelection(y => y.Import = false));

        Disposables.Add(Collection.ConnectSource().AutoRefresh(x => x.Import).Filter(x => x.Import).Bind(out _importItems).Subscribe());
    }

    public ReadOnlyObservableCollection<T> ImportItems => _importItems;

    public ICommand ImportSelectionCommand { get; }

    public ICommand DoNotImportSelectionCommand { get; }
}
