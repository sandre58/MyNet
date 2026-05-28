// -----------------------------------------------------------------------
// <copyright file="StringListEditionViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Observable;
using MyNet.UI.Commands;

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Provides an editable list of strings with add/remove row commands.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StringListEditionViewModel"/> class.
/// </remarks>
/// <param name="commandFactory">Optional command factory used to create commands.</param>
public sealed class StringListEditionViewModel(ICommandFactory? commandFactory = null) : CollectionEditionViewModel<StringListItemViewModel>(commandFactory)
{
    /// <summary>
    /// Gets the number of non-empty values in <see cref="CollectionEditionViewModel{TRow}.Items"/>.
    /// </summary>
    public int Count => Items.Count(x => !string.IsNullOrWhiteSpace(x.Value));

    /// <summary>
    /// Loads values into the editor.
    /// Ensures there is always at least one editable row.
    /// </summary>
    /// <param name="values">The values to edit.</param>
    public void Load(IEnumerable<string>? values) => LoadItems(values?.Select(x => new StringListItemViewModel { Value = x }));

    /// <summary>
    /// Returns non-empty values from <see cref="CollectionEditionViewModel{TRow}.Items"/>.
    /// </summary>
    /// <returns>The current edited values.</returns>
    public IReadOnlyCollection<string> GetValues()
        => [.. Items.Select(x => x.Value).Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>()];

    /// <summary>
    /// Replaces the target collection content with current edited values.
    /// </summary>
    /// <param name="target">The target collection to update.</param>
    public void ApplyTo(ICollection<string> target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.Clear();

        foreach (var value in GetValues())
            target.Add(value);
    }

    /// <inheritdoc />
    protected override StringListItemViewModel CreateNewItem() => new();

    /// <inheritdoc />
    protected override bool CanAdd(StringListItemViewModel? item)
        => item is not null
           && Items.IndexOf(item) == Items.Count - 1
           && !string.IsNullOrWhiteSpace(item.Value);

    /// <inheritdoc />
    protected override bool CanRemove(StringListItemViewModel? item)
        => item is not null && (Items.Count > 1 || !string.IsNullOrWhiteSpace(item.Value));

    /// <inheritdoc />
    protected override void EnsureEditableRow()
    {
        if (!Items.Any())
            Items.Add(new());
    }

    /// <inheritdoc />
    protected override void OnItemsStateChanged()
    {
        (AddCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
        (RemoveCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
        NotifyPropertyChanged(nameof(Count));
    }
}

/// <summary>
/// Represents one editable string row.
/// </summary>
public sealed class StringListItemViewModel : ObservableObject
{
    /// <summary>
    /// Gets or sets the row value.
    /// </summary>
    public string? Value { get; set; }
}
