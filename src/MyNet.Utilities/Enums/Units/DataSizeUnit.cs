// -----------------------------------------------------------------------
// <copyright file="DataSizeUnit.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Specifies a unit of measurement for data sizes, ranging from bytes to terabytes.
/// </summary>
public enum DataSizeUnit
{
    Byte,
    Kilobyte,
    Megabyte,
    Gigabyte,
    Terabyte
}
