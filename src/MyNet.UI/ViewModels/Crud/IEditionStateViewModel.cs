// -----------------------------------------------------------------------
// <copyright file="IEditionStateViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Crud;

/// <summary>
/// Defines a minimal contract for edition state tracking.
/// </summary>
public interface IEditionStateViewModel
{
    /// <summary>
    /// Gets a value indicating whether the editor has pending changes.
    /// </summary>
    bool IsDirty { get; }
}
