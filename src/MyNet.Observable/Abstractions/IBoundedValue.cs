// -----------------------------------------------------------------------
// <copyright file="IBoundedValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using System.Numerics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for a bounded numeric value that can be observed for changes and reset to a default.
/// </summary>
/// <typeparam name="T">The type of the numeric value.</typeparam>
public interface IBoundedValue<T> : INotifyPropertyChanged, IResettable<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// Gets or sets the current value of the bounded value.
    /// </summary>
    T? Value { get; set; }

    /// <summary>
    /// Gets or sets the minimum value of the bounded value. This property is nullable, allowing for the possibility of an undefined or uninitialized state. When setting the minimum value, it should be validated against the current value and the maximum value to ensure consistency. If the minimum value is set outside of these bounds, it may throw an exception or be adjusted to fit within the bounds, depending on the implementation.
    /// </summary>
    T? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of the bounded value. This property is nullable, allowing for the possibility of an undefined or uninitialized state. When setting the maximum value, it should be validated against the current value and the minimum value to ensure consistency. If the maximum value is set outside of these bounds, it may throw an exception or be adjusted to fit within the bounds, depending on the implementation.
    /// </summary>
    T? Max { get; set; }
}
