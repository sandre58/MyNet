// -----------------------------------------------------------------------
// <copyright file="PropertyMutationContext.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Represents the context of a property mutation operation, providing information about the sender, property name, old value, and new value. This context is passed to property-changing and property-changed behaviors so they can react to mutations on an observable object.
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
}
