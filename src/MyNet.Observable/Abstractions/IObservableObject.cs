// -----------------------------------------------------------------------
// <copyright file="IObservableObject.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for observable objects that support property change notifications, property changing notifications, and can be disposed of. This interface combines the functionality of INotifyPropertyChanged and INotifyPropertyChanging, allowing implementing classes to notify subscribers when a property is about to change and after it has changed. Additionally, by implementing IDisposable, it allows for proper resource management and cleanup when the object is no longer needed. This is particularly useful in scenarios where the observable object holds onto resources or needs to perform cleanup operations when it is disposed of, ensuring that resources are released appropriately and preventing memory leaks in applications that use data binding or other observable patterns.
/// </summary>
public interface IObservableObject : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable;
