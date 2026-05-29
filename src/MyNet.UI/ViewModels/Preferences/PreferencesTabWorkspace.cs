// -----------------------------------------------------------------------
// <copyright file="PreferencesTabWorkspace.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Globalization.Culture;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.UI.ViewModels.Preferences;

/// <summary>
/// Tab host used by <see cref="PreferencesViewModel"/>.
/// </summary>
internal sealed class PreferencesTabWorkspace : TabWorkspaceViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PreferencesTabWorkspace"/> class.
    /// </summary>
    public PreferencesTabWorkspace(ICommandFactory? commandFactory, ICultureService? cultureService)
        : base(commandFactory, cultureService)
    {
    }

    /// <summary>
    /// Adds pages to the tab host.
    /// </summary>
    public void AddPages(IEnumerable<IWorkspaceViewModel> pages) => AddWorkspaces(pages);
}
