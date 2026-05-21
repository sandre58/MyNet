// -----------------------------------------------------------------------
// <copyright file="IPropertyChangingBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Defines a behavior that is executed when a property of an ObservableObject is changing. Implementing this interface allows an object to react to property changes by executing custom logic before a property changes. This is useful for implementing behaviors such as validation, modification tracking, or collection tracking, where you want to execute specific logic before a property changes. The ProcessPropertyChanging method is called before a property changes, and it receives the sender (the ObservableObject that raised the event), the name of the property that is changing, the value of the property before the change, and the value of the property after the change. By implementing this method, you can define custom behavior that should be executed before a property changes, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
/// </summary>
public interface IPropertyChangingBehavior : IObservableBehavior
{
    /// <summary>
    /// Handles the property changing event by executing custom logic before a property changes. This method is called before a property changes, and receives the sender (the ObservableObject that raised the event), the name of the property that is changing, the value of the property before the change, and the value of the property after the change. Implementing this method allows you to define custom behavior that should be executed before a property changes, such as validation, modification tracking, or collection tracking, allowing you to enhance the functionality of your observable objects in a modular and reusable way.
    /// </summary>
    /// <param name="context">The context of the property change, containing information about the property that is changing, its old value, and its new value.</param>
    void OnPropertyChanging(PropertyMutationContext context);
}
