// -----------------------------------------------------------------------
// <copyright file="IObservableValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines an interface for an observable value that notifies listeners when its value changes. This interface extends the INotifyPropertyChanged interface, allowing it to be used in data binding scenarios where changes to the value need to be reflected in the user interface. The Value property represents the current value of the observable, and it can be of any type specified by the generic type parameter T. By implementing this interface, an object can provide a way for other components to observe changes to its value and react accordingly, enabling better separation of concerns and improved maintainability in an application.
/// </summary>
/// <typeparam name="T">The type of the value being observed.</typeparam>
public interface IObservableValue<out T> : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the current value of the observable. This property represents the value that is being observed, and it can be of any type specified by the generic type parameter T. When the value changes, the PropertyChanged event will be raised, allowing any listeners to react to the change and update accordingly. By implementing this property, an object can provide a way for other components to access its current value and observe changes to it, enabling better separation of concerns and improved maintainability in an application.
    /// </summary>
    T? Value { get; }
}
