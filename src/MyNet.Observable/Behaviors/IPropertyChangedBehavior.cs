// -----------------------------------------------------------------------
// <copyright file="IPropertyChangedBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Defines a behavior that is executed when a property of an ObservableObject changes. Implementing this interface allows an object to react to property changes by executing custom logic when a property changes. This is useful for implementing behaviors such as validation, modification tracking, or collection tracking, where you want to execute specific logic when a property changes. The ProcessPropertyChanged method is called when a property changes, and it receives the sender (the ObservableObject that raised the event), the name of the property that changed, the value of the property before the change, and the value of the property after the change. By implementing this method, you can define custom behavior that should be executed when a property changes, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
/// </summary>
public interface IPropertyChangedBehavior : IObservableBehavior
{
    /// <summary>
    /// Handles the property change event by executing custom logic when a property changes. This method is called when a property changes, and receives the sender (the ObservableObject that raised the event), the name of the property that changed, the value of the property before the change, and the value of the property after the change. Implementing this method allows you to define custom behavior that should be executed when a property changes, such as validation, modification tracking, or collection tracking, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="context">The context of the property change, containing information about the property that changed, its old value, and its new value.</param>
    void OnPropertyChanged(PropertyMutationContext context);
}
