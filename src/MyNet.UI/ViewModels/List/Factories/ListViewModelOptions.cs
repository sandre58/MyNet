// -----------------------------------------------------------------------
// <copyright file="ListViewModelOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Reactive.Concurrency;
using MyNet.UI.Commands;
using MyNet.UI.Loading;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Sorting;

namespace MyNet.UI.ViewModels.List.Factories;

/// <summary>
/// Aggregates optional dependencies used to configure list view models.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public record ListViewModelOptions<T>
    where T : notnull
{
    /// <summary>
    /// Gets filtering configuration.
    /// </summary>
    public IFiltersViewModel<T>? Filters { get; init; }

    /// <summary>
    /// Gets sorting configuration.
    /// </summary>
    public ISortingViewModel<T>? Sorting { get; init; }

    /// <summary>
    /// Gets grouping configuration.
    /// </summary>
    public IGroupingViewModel<T>? Grouping { get; init; }

    /// <summary>
    /// Gets paging configuration.
    /// </summary>
    public IPagingViewModel? Paging { get; init; }

    /// <summary>
    /// Gets busy service.
    /// </summary>
    public IBusyService? BusyService { get; init; }

    /// <summary>
    /// Gets scheduler used for asynchronous operations.
    /// </summary>
    public IScheduler? Scheduler { get; init; }

    /// <summary>
    /// Gets command factory used by list-related view models.
    /// </summary>
    public ICommandFactory? CommandFactory { get; init; }
}
