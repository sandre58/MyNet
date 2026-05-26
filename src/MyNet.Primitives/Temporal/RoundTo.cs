// -----------------------------------------------------------------------
// <copyright file="RoundTo.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Primitives.Temporal;

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
