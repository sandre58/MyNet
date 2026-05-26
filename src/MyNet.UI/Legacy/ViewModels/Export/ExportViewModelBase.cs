// -----------------------------------------------------------------------
// <copyright file="ExportViewModelBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.Humanizer;
using MyNet.UI.Commands;
using MyNet.UI.Legacy.ViewModels.Workspace;
using MyNet.UI.Resources;
using MyNet.UI.Toasting;
using MyNet.Primitives.Exceptions;

namespace MyNet.UI.Legacy.ViewModels.Export;

public abstract class ExportViewModelBase<T> : WorkspaceDialogViewModel
{
    private IEnumerable<T>? _items;

    public bool SaveConfigurationOnValidate { get; set; } = true;

    public ICommand ExportAndCloseCommand { get; }

    public ICommand CancelCommand { get; }

    protected ExportViewModelBase()
    {
        ExportAndCloseCommand = RelayCommandFactory.Default.Create(async () => await ExportAndCloseAsync().ConfigureAwait(false));
        CancelCommand = RelayCommandFactory.Default.Create(() => Close(false));
    }

    public virtual void Load(IEnumerable<T> items)
    {
        _items = items;
        Title = nameof(UiResources.ExportXItems).TranslateAndFormatWithCount(_items.Count());
    }

    protected virtual async Task ExportAndCloseAsync()
    {
        if (_items?.Any() != true)
            throw new TranslatableException(UiResources.ExportNoItemsError);

        if (ValidateProperties())
        {
            if (SaveConfigurationOnValidate)
                SaveConfiguration();

            if (await ExportItemsAsync(_items).ConfigureAwait(false))
                Close(true);
        }
        else
        {
            var errors = GetValidationErrors().ToList();
            errors.ToList().ForEach(x => ToasterManager.ShowError(x));
        }
    }

    protected virtual void SaveConfiguration() { }

    protected virtual void LoadConfiguration() { }

    protected abstract Task<bool> ExportItemsAsync(IEnumerable<T> items);
}
