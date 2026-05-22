// -----------------------------------------------------------------------
// <copyright file="IModificationTrackingBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Defines the contract for a modification tracking behavior that provides modification tracking support for an ObservableObject. This interface extends both IObservableBehavior and IModificationAware, indicating that it is a behavior that can be attached to an ObservableObject and provides modification tracking capabilities. By implementing this interface, a class can provide functionality for tracking modifications of the properties of an ObservableObject, allowing for better data integrity and user feedback in applications that utilize the MyNet.Observable framework.
/// </summary>
public interface IModificationTrackingBehavior : IPropertyChangedBehavior, IModificationAware;
