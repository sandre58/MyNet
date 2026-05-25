// -----------------------------------------------------------------------
// <copyright file="IResettable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines an interface for objects that can be reset to a default state. This interface provides a contract for implementing classes to determine if they can be reset, retrieve the default value, and perform the reset operation. By implementing this interface, an object can provide a way for other components to reset its state to a known default configuration, which can be useful in scenarios where an object needs to be reused or returned to its initial state after certain operations. The generic type parameter T represents the type of the default value that the object can be reset to, allowing for flexibility in the types of objects that can implement this interface.
/// </summary>
/// <typeparam name="T">The type of the default value that the object can be reset to.</typeparam>
public interface IResettable<T>
    where T : struct
{
    /// <summary>
    /// Determines whether the object can be reset to a default state. This method allows implementing classes to indicate whether they are currently in a state that can be reset, which can be useful for preventing unnecessary reset operations or providing feedback to users about the reset capability of the object. By implementing this method, an object can provide a way for other components to check if it is eligible for resetting before attempting to perform the reset operation, enabling better control and user experience in an application.
    /// </summary>
    /// <returns><c>true</c> if the object can be reset; otherwise, <c>false</c>.</returns>
    bool CanReset();

    /// <summary>
    /// Gets the default value that the object can be reset to. This property allows implementing classes to provide a way for other components to retrieve the default value that the object can be reset to, enabling better control and user experience in an application.
    /// </summary>
    T? DefaultValue { get; }

    /// <summary>
    /// Resets the object to its default state. This method allows implementing classes to provide a way for other components to reset the object's state to a known default configuration, which can be useful in scenarios where an object needs to be reused or returned to its initial state after certain operations.
    /// </summary>
    void Reset();
}
