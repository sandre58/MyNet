// -----------------------------------------------------------------------
// <copyright file="RecentFilesListOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Filtering.Filters;
using MyNet.UI.ViewModels.List.Sorting;

namespace MyNet.UI.ViewModels.FileHistory;

/// <summary>
/// List configuration for recent files.
/// </summary>
internal sealed class RecentFilesListOptions(
    FiltersViewModel<RecentFileViewModel> filters,
    StringFilterViewModel<RecentFileViewModel> nameFilter,
    StringFilterViewModel<RecentFileViewModel> pathFilter,
    ISortingViewModel<RecentFileViewModel> sorting)
{
    /// <summary>
    /// Creates filtering and sorting view models for <see cref="RecentFilesViewModel"/>.
    /// </summary>
    public static RecentFilesListOptions Create()
    {
        var nameFilter = new StringFilterViewModel<RecentFileViewModel>(
            nameof(RecentFileViewModel.DisplayName),
            x => x.DisplayName);

        var pathFilter = new StringFilterViewModel<RecentFileViewModel>(
            nameof(RecentFileViewModel.Path),
            x => x.Path);

        var searchGroup = new FilterGroupViewModel<RecentFileViewModel> { Operator = LogicalOperator.Or };
        searchGroup.Add(nameFilter);
        searchGroup.Add(pathFilter);

        var root = new FilterGroupViewModel<RecentFileViewModel>();
        root.Add(searchGroup);

        var filters = new FiltersViewModel<RecentFileViewModel>(root);

        var sorting = SortingViewModelBuilder<RecentFileViewModel>.Create(builder =>
        {
            builder.AddProperty(
                x => x.DisplayName,
                property => property
                    .WithDisplayName(nameof(RecentFileViewModel.DisplayName))
                    .Ascending());

            builder.AddProperty(
                x => x.LastAccessDate,
                property => property
                    .WithDisplayName(nameof(RecentFileViewModel.LastAccessDate))
                    .Descending());
        });

        return new(filters, nameFilter, pathFilter, sorting);
    }

    public FiltersViewModel<RecentFileViewModel> Filters { get; } = filters;

    public StringFilterViewModel<RecentFileViewModel> NameFilter { get; } = nameFilter;

    public StringFilterViewModel<RecentFileViewModel> PathFilter { get; } = pathFilter;

    public ISortingViewModel<RecentFileViewModel> Sorting { get; } = sorting;

    /// <summary>
    /// Updates the shared search text applied to name and path filters.
    /// </summary>
    public void SetSearchText(string? value)
    {
        NameFilter.Value = value;
        PathFilter.Value = value;
        Filters.Apply();
    }
}
