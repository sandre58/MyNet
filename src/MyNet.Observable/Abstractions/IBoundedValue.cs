// -----------------------------------------------------------------------
// <copyright file="IBoundedValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for a bounded value that can be observed for changes and can be reset to a default value. This interface extends the <see cref="IObservableValue{T}"/> interface, which allows subscribers to be notified when the value changes, and the <see cref="IResettable{T}"/> interface, which provides a method to reset the value to a default state. The generic type parameter T is constrained to be a struct that implements the <see cref="INumber{T}"/> interface, ensuring that it can represent numeric values and supports basic arithmetic operations. The Value property is nullable, allowing for the possibility of an undefined or uninitialized state, while the Min and Max properties define the bounds within which the value can exist. This interface is useful in scenarios where you want to represent a numeric value that has specific limits and can be observed for changes, such as in data binding scenarios or when implementing view models in MVVM architecture.
/// </summary>
/// <typeparam name="T">The type of the numeric value.</typeparam>
public interface IBoundedValue<T> : IObservableValue<T>, IResettable<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// Gets or sets the current value of the bounded value. This property is nullable, allowing for the possibility of an undefined or uninitialized state. When setting the value, it should be validated against the Min and Max properties to ensure that it falls within the defined bounds. If the value is set outside of these bounds, it may throw an exception or be adjusted to fit within the bounds, depending on the implementation. Subscribers to this property will be notified whenever the value changes, allowing for reactive programming patterns and data binding scenarios where changes to the value need to be reflected in the user interface or other parts of the application.
    /// </summary>
    new T? Value { get; set; }

    /// <summary>
    /// Gets or sets the minimum value of the bounded value. This property is nullable, allowing for the possibility of an undefined or uninitialized state. When setting the minimum value, it should be validated against the current value and the maximum value to ensure consistency. If the minimum value is set outside of these bounds, it may throw an exception or be adjusted to fit within the bounds, depending on the implementation.
    /// </summary>
    T? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of the bounded value. This property is nullable, allowing for the possibility of an undefined or uninitialized state. When setting the maximum value, it should be validated against the current value and the minimum value to ensure consistency. If the maximum value is set outside of these bounds, it may throw an exception or be adjusted to fit within the bounds, depending on the implementation.
    /// </summary>
    T? Max { get; set; }
}
