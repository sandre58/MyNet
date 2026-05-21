// -----------------------------------------------------------------------
// <copyright file="ISmartEnum.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Represents a smart enumeration member with a display name.
/// This interface defines the contract for enum-like types that can be used throughout the framework.
/// </summary>
/// <remarks>
/// SmartEnum is a pattern that provides type-safe enumeration with additional properties.
/// This interface marks an object as a smart enumeration instance.
/// Do not mix localization concerns here - use Humanizer for that.
/// </remarks>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "SmartEnum is a well-known pattern and the name is appropriate for its purpose.")]
public interface ISmartEnum
{
    /// <summary>
    /// Gets the display name of this enumeration member.
    /// </summary>
    /// <remarks>
    /// This is typically the name of the field used to declare the instance,
    /// but can be overridden in derived classes for custom representation.
    /// </remarks>
    string Name { get; }
}
