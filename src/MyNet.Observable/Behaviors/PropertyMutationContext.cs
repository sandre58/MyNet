// -----------------------------------------------------------------------
// <copyright file="PropertyMutationContext.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Represents the context of a property mutation operation, providing information about the sender, property name, old value, new value, and a cancellation flag. This context can be used in scenarios where you want to intercept or react to property changes in an observable object, allowing you to access relevant details about the mutation and optionally cancel it if necessary.
/// </summary>
public sealed class PropertyMutationContext
{
    /// <summary>
    /// Gets the sender of the property mutation, which is the observable object that is undergoing the property change. This property is required and must be initialized when creating an instance of the <see cref="PropertyMutationContext"/> class. The sender provides context about which object is being modified, allowing event handlers or behaviors to access additional information or perform actions based on the specific object that is changing.
    /// </summary>
    public required ObservableObject Sender { get; init; }

    /// <summary>
    /// Gets the name of the property that is being mutated. This property is required and must be initialized when creating an instance of the <see cref="PropertyMutationContext"/> class. The property name provides context about which property is being modified, allowing event handlers or behaviors to access additional information or perform actions based on the specific property that is changing.
    /// </summary>
    public required string PropertyName { get; init; }

    /// <summary>
    /// Gets the old value of the property before the mutation. This property is required and must be initialized when creating an instance of the <see cref="PropertyMutationContext"/> class. The old value provides context about the previous state of the property, allowing event handlers or behaviors to access additional information or perform actions based on the previous value.
    /// </summary>
    public required object? OldValue { get; init; }

    /// <summary>
    /// Gets the new value of the property after the mutation. This property is required and must be initialized when creating an instance of the <see cref="PropertyMutationContext"/> class. The new value provides context about the updated state of the property, allowing event handlers or behaviors to access additional information or perform actions based on the new value.
    /// </summary>
    public required object? NewValue { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the property mutation should be canceled. This property can be set by event handlers or behaviors to prevent the property change from occurring, allowing for validation or other conditional logic to be applied before the mutation is finalized.
    /// </summary>
    public bool Cancel { get; set; }
}
