// -----------------------------------------------------------------------
// <copyright file="IEventAware.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines an interface for objects that are aware of events of a specific type. This interface allows an object to react to events by implementing the OnEvent method, which will be called whenever an event of the specified type occurs. By implementing this interface, an object can subscribe to and handle events in a decoupled manner, allowing for better separation of concerns and improved maintainability in an application. The generic type parameter TEvent represents the type of event that the object is interested in and will react to when it occurs.
/// </summary>
/// <typeparam name="TEvent">The type of event that the object is interested in.</typeparam>
public interface IEventAware<in TEvent>
{
    /// <summary>
    /// Handles the specified event. This method will be called whenever an event of the specified type occurs, allowing the object to react to the event in a decoupled manner. By implementing this method, an object can subscribe to and handle events of the specified type, enabling better separation of concerns and improved maintainability in an application. The parameter e represents the event that occurred and contains any relevant information associated with that event, allowing the object to respond appropriately based on the event's data and context.
    /// </summary>
    /// <param name="e">The event that occurred.</param>
    void OnEvent(TEvent e);
}

/// <summary>
/// Defines a contract for objects that are aware of time zone changes and can handle them asynchronously. Implementing this interface allows an object to react to changes in the system's time zone settings in an asynchronous manner, enabling it to update any properties or displays that are affected by the time zone change, such as date and time displays, without blocking the main thread. This is particularly useful for applications that need to perform time-consuming operations when reacting to time zone changes, such as fetching data or performing complex calculations, while still maintaining a responsive user interface.
/// </summary>
/// <typeparam name="TEvent">The type of the event that the object is aware of.</typeparam>
public interface IAsyncEventAware<in TEvent>
{
    /// <summary>
    /// Asynchronously handles the specified event, allowing the object to react to changes in the system's time zone settings without blocking the main thread. This method is called when the relevant event occurs, and it can perform any necessary updates to properties or displays that are affected by the time zone change, such as date and time displays. By implementing this method, you can ensure that your application remains responsive while still reacting appropriately to time zone changes, even if the handling of those changes involves time-consuming operations.
    /// </summary>
    /// <param name="e">The event data associated with the time zone change.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnEventAsync(TEvent e, CancellationToken cancellationToken = default);
}
