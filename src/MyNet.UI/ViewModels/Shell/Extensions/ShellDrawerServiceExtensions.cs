// -----------------------------------------------------------------------
// <copyright file="ShellDrawerServiceExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Convenience helpers for <see cref="IShellDrawerService"/>.
/// </summary>
public static class ShellDrawerServiceExtensions
{
    extension(IShellDrawerService shellDrawerService)
    {
        /// <summary>
        /// Shows the notifications drawer.
        /// </summary>
        public void ShowNotificationsDrawer() =>
            shellDrawerService.SetNotificationsDrawer(ShellDrawerAction.Show);

        /// <summary>
        /// Hides the notifications drawer.
        /// </summary>
        public void HideNotificationsDrawer() =>
            shellDrawerService.SetNotificationsDrawer(ShellDrawerAction.Hide);

        /// <summary>
        /// Toggles the notifications drawer.
        /// </summary>
        public void ToggleNotificationsDrawer() =>
            shellDrawerService.SetNotificationsDrawer(ShellDrawerAction.Toggle);

        /// <summary>
        /// Shows the file menu drawer when supported by the host.
        /// </summary>
        public void ShowFileMenuDrawer() =>
            shellDrawerService.SetFileMenuDrawer(ShellDrawerAction.Show);

        /// <summary>
        /// Hides the file menu drawer when supported by the host.
        /// </summary>
        public void HideFileMenuDrawer() =>
            shellDrawerService.SetFileMenuDrawer(ShellDrawerAction.Hide);

        /// <summary>
        /// Toggles the file menu drawer when supported by the host.
        /// </summary>
        public void ToggleFileMenuDrawer() =>
            shellDrawerService.SetFileMenuDrawer(ShellDrawerAction.Toggle);
    }
}
