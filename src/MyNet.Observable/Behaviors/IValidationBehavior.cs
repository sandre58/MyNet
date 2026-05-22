// -----------------------------------------------------------------------
// <copyright file="IValidationBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Defines the contract for a validation behavior that provides validation support for an ObservableObject. This interface extends both IObservableBehavior and IValidationAware, indicating that it is a behavior that can be attached to an ObservableObject and provides validation capabilities. By implementing this interface, a class can provide functionality for validating the properties of an ObservableObject and notifying about validation errors, allowing for better data integrity and user feedback in applications that utilize the MyNet.Observable framework.
/// </summary>
public interface IValidationBehavior : IObservableBehavior, IValidationAware;
