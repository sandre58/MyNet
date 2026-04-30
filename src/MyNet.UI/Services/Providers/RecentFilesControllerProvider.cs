// -----------------------------------------------------------------------
// <copyright file="RecentFilesControllerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.Observable;
using MyNet.UI.ViewModels.FileHistory;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Filtering.Filters;
using MyNet.UI.ViewModels.List.Sorting;
using MyNet.Utilities.Comparison;

namespace MyNet.UI.Services.Providers;

/// <summary>
/// Provides list parameters for recent files, including filters and sorting.
/// </summary>
public class RecentFilesControllerProvider : ListParametersProvider
{
    /// <summary>
    /// Provides filters for recent files.
    /// </summary>
    /// <returns>A new instance of <see cref="RecentFilesFilters"/>.</returns>
    public override IFiltersViewModel ProvideFilters() => new RecentFilesFilters();

    /// <summary>
    /// Provides sorting for recent files.
    /// </summary>
    /// <returns>A new instance of <see cref="RecentFilesSorting"/>.</returns>
    public override ISortingViewModel ProvideSorting() => new RecentFilesSorting();
}

/// <summary>
/// Implements filters for recent files, supporting text-based filtering on name and path.
/// </summary>
public class RecentFilesFilters : ObservableObject, IFiltersViewModel
{
    /// <inheritdoc/>
    public event EventHandler<FiltersChangedEventArgs>? FiltersChanged;

    /// <summary>
    /// Gets or sets the text used for filtering recent files by name or path.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <inheritdoc/>
    public void Refresh() => OnTextChanged();

    /// <inheritdoc/>
    public void Clear() => Text = string.Empty;

    /// <inheritdoc/>
    public void Reset() => Text = string.Empty;

    /// <summary>
    /// Raises the <see cref="FiltersChanged"/> event with the current filter values.
    /// </summary>
    protected virtual void OnTextChanged()
        => FiltersChanged?.Invoke(this, new(
        [
            new CompositeFilterViewModel(new StringFilterViewModel(nameof(RecentFileViewModel.Name)) { Value = Text }),
            new CompositeFilterViewModel(new FileNameFilterViewModel(nameof(RecentFileViewModel.Path)) { Value = Text }, LogicalOperator.Or)
        ]));
}

/// <summary>
/// Implements sorting for recent files, supporting sorting by name or last access date.
/// </summary>
public class RecentFilesSorting : ObservableObject, ISortingViewModel
{
    /// <summary>
    /// Gets or sets the property used for sorting recent files.
    /// </summary>
    public RecentFilesSortingProperty SortingProperty { get; set; } = RecentFilesSortingProperty.LastAccessDate;

    /// <summary>
    /// Gets or sets a value indicating whether sorting is ascending.
    /// </summary>
    public bool IsAscending { get; set; }

    /// <inheritdoc/>
    public event EventHandler<SortingChangedEventArgs>? SortingChanged;

    /// <summary>
    /// Raises the <see cref="SortingChanged"/> event when the sorting property changes.
    /// </summary>
    protected virtual void OnSortingPropertyChanged() => Sort();

    /// <summary>
    /// Raises the <see cref="SortingChanged"/> event when the sorting direction changes.
    /// </summary>
    protected virtual void OnIsAscendingChanged() => Sort();

    /// <summary>
    /// Raises the <see cref="SortingChanged"/> event with the current sorting values.
    /// </summary>
    private void Sort()
        => SortingChanged?.Invoke(this, new([new SortingPropertyViewModel(SortingProperty.ToString(), IsAscending ? ListSortDirection.Ascending : ListSortDirection.Descending)]));

    /// <inheritdoc/>
    public void Reset()
    {
        SortingProperty = RecentFilesSortingProperty.LastAccessDate;
        IsAscending = false;
        Sort();
    }
}

/// <summary>
/// Specifies the properties by which recent files can be sorted.
/// </summary>
public enum RecentFilesSortingProperty
{
    /// <summary>
    /// Sort by file name.
    /// </summary>
    Name,

    /// <summary>
    /// Sort by last access date.
    /// </summary>
    LastAccessDate
}
