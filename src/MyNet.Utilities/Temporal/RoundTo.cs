// -----------------------------------------------------------------------
// <copyright file="RoundTo.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Used for rounding precision.
/// </summary>
public enum RoundTo
{
    /// <summary>
    /// Second precision.
    /// </summary>
    Second,

    /// <summary>
    /// Minute precision.
    /// </summary>
    Minute,

    /// <summary>
    /// Hour precision.
    /// </summary>
    Hour,

    /// <summary>
    /// Day precision.
    /// </summary>
    Day
}
