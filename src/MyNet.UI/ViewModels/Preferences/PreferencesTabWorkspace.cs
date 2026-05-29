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
/// <remarks>
/// Initializes a new instance of the <see cref="PreferencesTabWorkspace"/> class.
/// </remarks>
internal sealed class PreferencesTabWorkspace(ICommandFactory? commandFactory, ICultureService? cultureService) : TabWorkspaceViewModel(commandFactory, cultureService)
{
    /// <summary>
    /// Adds pages to the tab host.
    /// </summary>
    public void AddPages(IEnumerable<IWorkspaceViewModel> pages) => AddWorkspaces(pages);
}
