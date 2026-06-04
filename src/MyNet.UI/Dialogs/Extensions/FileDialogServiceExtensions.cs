// -----------------------------------------------------------------------
// <copyright file="FileDialogServiceExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Dialogs.FileDialogs;

#pragma warning disable IDE0130
namespace MyNet.UI.Dialogs;
#pragma warning restore IDE0130

/// <summary>
/// Fluent helpers for file dialogs on <see cref="IDialogService"/>.
/// </summary>
public static class FileDialogServiceExtensions
{
    extension(IDialogService dialogService)
    {
        /// <summary>
        /// Starts configuring a save file dialog.
        /// </summary>
        public SaveFileDialogBuilder SaveFile() => new(dialogService);

        /// <summary>
        /// Starts configuring an open file dialog.
        /// </summary>
        public OpenFileDialogBuilder OpenFile() => new(dialogService);

        /// <summary>
        /// Starts configuring a folder picker dialog.
        /// </summary>
        public OpenFolderDialogBuilder OpenFolder() => new(dialogService);
    }
}
