// -----------------------------------------------------------------------
// <copyright file="LoadState.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Defines the contract for an object that can be loaded asynchronously, refreshed, and reset, with a state indicating its loading status.
/// </summary>
public enum LoadState
{
    /// <summary>
    /// Indicates that the object has not been loaded yet and is in its initial state. No loading operation has been initiated.
    /// </summary>
    NotLoaded,

    /// <summary>
    /// Indicates that the object is currently in the process of loading. This state is active while the asynchronous loading operation is ongoing, and it may be used to display loading indicators or disable certain UI interactions until the loading is complete.
    /// </summary>
    Loading,

    /// <summary>
    /// Indicates that the object has been successfully loaded and is ready for use. This state signifies that the loading operation has completed without errors, and the object's data or resources are now available for interaction. UI elements can be enabled, and any loading indicators can be hidden when this state is active.
    /// </summary>
    Loaded,

    /// <summary>
    /// Indicates that an error occurred during the loading process. This state signifies that the loading operation encountered an issue, and the object's data or resources may not be available for interaction. UI elements can be disabled, and error messages or indicators can be displayed when this state is active.
    /// </summary>
    Error
}
