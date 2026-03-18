// -----------------------------------------------------------------------
// <copyright file="ListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using DynamicData;
using MyNet.Observable.Attributes;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Providers;
using MyNet.UI.Loading;
using MyNet.Utilities.Providers;

namespace MyNet.UI.ViewModels.List;

[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public class ListViewModel<T> : ListViewModelBase<T, ExtendedCollection<T>>
    where T : notnull
{
    public ListViewModel(ICollection<T> source, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : this(source, MyNet.UI.Threading.Scheduler.UiOrCurrent, parametersProvider, busyService) { }

    public ListViewModel(ICollection<T> source, IScheduler scheduler, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : base(new ExtendedCollection<T>(source, scheduler), parametersProvider, busyService) { }

    public ListViewModel(IItemsProvider<T> source, bool loadItems = true, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : this(source, loadItems, MyNet.UI.Threading.Scheduler.UiOrCurrent, parametersProvider, busyService) { }

    public ListViewModel(IItemsProvider<T> source, bool loadItems, IScheduler scheduler, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : base(new ExtendedCollection<T>(source, loadItems, scheduler), parametersProvider, busyService) { }

    public ListViewModel(ISourceProvider<T> source, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : this(source, MyNet.UI.Threading.Scheduler.UiOrCurrent, parametersProvider, busyService) { }

    public ListViewModel(ISourceProvider<T> source, IScheduler scheduler, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : base(new ExtendedCollection<T>(source, scheduler), parametersProvider, busyService) { }

    public ListViewModel(IObservable<IChangeSet<T>> source, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : this(source, MyNet.UI.Threading.Scheduler.UiOrCurrent, parametersProvider, busyService) { }

    public ListViewModel(IObservable<IChangeSet<T>> source, IScheduler scheduler, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : base(new ExtendedCollection<T>(source, scheduler), parametersProvider, busyService) { }

    public ListViewModel(IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : this(MyNet.UI.Threading.Scheduler.UiOrCurrent, parametersProvider, busyService) { }

    public ListViewModel(IScheduler scheduler, IListParametersProvider? parametersProvider = null, IBusyService? busyService = null)
        : base(new ExtendedCollection<T>(scheduler), parametersProvider, busyService) { }
}
