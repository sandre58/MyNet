// -----------------------------------------------------------------------
// <copyright file="NavigationMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Navigation.Models;

/// <summary>How the navigation was initiated.</summary>
public enum NavigationMode
{
    /// <summary>Standard forward navigation.</summary>
    Normal,

    /// <summary>Back stack navigation.</summary>
    Back,

    /// <summary>Forward stack navigation.</summary>
    Forward
}
