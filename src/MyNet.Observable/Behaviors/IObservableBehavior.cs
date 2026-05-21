// -----------------------------------------------------------------------
// <copyright file="IObservableBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Defines a behavior that can be added to an ObservableObject to enhance its functionality. Implementing this interface allows an object to be added as a behavior to an ObservableObject, enabling it to enhance the functionality of the observable object in a modular and reusable way. The Suspend method is used to temporarily suspend the behavior, allowing you to prevent it from executing its logic when certain conditions are met, such as during bulk updates or when the behavior is not needed. By implementing this interface, you can create custom behaviors that can be easily added to your observable objects, allowing you to enhance their functionality without modifying their original implementation or creating new derived classes.
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface to allow non-generic registration of behaviors.")]
public interface IObservableBehavior;
