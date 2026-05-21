// -----------------------------------------------------------------------
// <copyright file="MassUnit.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Specifies a unit of measurement for mass, ranging from milligrams to kilograms.
/// </summary>
public enum MassUnit
{
    Milligram = -3,
    Centigram = -2,
    Decigram = -1,
    Gram = 0,
    Decagram = 1,
    Hectogram = 2,
    Kilogram = 3
}
