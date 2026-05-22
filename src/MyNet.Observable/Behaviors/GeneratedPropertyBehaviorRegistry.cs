// -----------------------------------------------------------------------
// <copyright file="GeneratedPropertyBehaviorRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Provides a registry for generated property behaviors that can be applied to ObservableObject instances. This class allows for the registration of forwarding properties, which are properties that forward property changed notifications from the owner to a specified property name. The registry maintains a mapping of owner types to their registered forwarding properties and ensures that the behaviors are applied to the appropriate ObservableObject instances when requested. By using this registry, developers can easily manage and apply generated property behaviors across their ObservableObject instances, enhancing the functionality and maintainability of their applications that utilize the MyNet.Observable framework.
/// </summary>
public static class GeneratedPropertyBehaviorRegistry
{
    private static readonly ConcurrentDictionary<Type, List<ForwardPropertyRegistration>> Registrations = new();
    private static readonly ConditionalWeakTable<ObservableObject, object?> AppliedOwners = [];

    /// <summary>
    /// Registers a forwarding property for the specified owner type and property name. This method adds a new forwarding property registration to the registry for the given owner type. If a registration for the same property name already exists for the owner type, it will not be added again. The concatenatePropertyName parameter indicates whether the property name should be concatenated with the owner's property name when forwarding notifications. By calling this method, developers can specify which properties should forward their change notifications to other properties, allowing for more flexible and dynamic behavior in their ObservableObject instances.
    /// </summary>
    /// <param name="ownerType">The type of the owner object.</param>
    /// <param name="propertyName">The name of the property to forward.</param>
    /// <param name="concatenatePropertyName">Indicates whether the property name should be concatenated with the owner's property name when forwarding notifications.</param>
    public static void RegisterForwardProperty(Type ownerType, string propertyName, bool concatenatePropertyName = true)
    {
        var list = Registrations.GetOrAdd(ownerType, static _ => []);

        lock (list)
        {
            if (list.Exists(x => string.Equals(x.PropertyName, propertyName, StringComparison.Ordinal)))
                return;

            list.Add(new(propertyName, concatenatePropertyName));
        }
    }

    /// <summary>
    /// Applies the registered forwarding property behaviors to the specified ObservableObject instance. This method checks if the given owner has already had the behaviors applied, and if not, it adds the owner to the AppliedOwners table to prevent duplicate applications. It then iterates through the type hierarchy of the owner, starting from its actual type and moving up to its base types, checking for any registered forwarding properties for each type. If any registrations are found, it creates a snapshot of the registrations to avoid issues with concurrent modifications and registers a new PropertyChangedForwardingBehavior for each registered property on the owner. This ensures that the appropriate forwarding behaviors are applied to the ObservableObject instance based on its type and the registered properties in the registry.
    /// </summary>
    /// <param name="owner">The ObservableObject instance to which the behaviors should be applied.</param>
    public static void Apply(ObservableObject owner)
    {
        if (AppliedOwners.TryGetValue(owner, out _))
            return;

        AppliedOwners.Add(owner, null);

        for (var current = owner.GetType(); current is not null && typeof(ObservableObject).IsAssignableFrom(current); current = current.BaseType)
        {
            if (!Registrations.TryGetValue(current, out var regs))
                continue;

            ForwardPropertyRegistration[] snapshot;
            lock (regs)
            {
                snapshot = [.. regs];
            }

            foreach (var registration in snapshot)
            {
                owner.RegisterBehavior(
                    new PropertyChangedForwardingBehavior(owner, registration.PropertyName, registration.ConcatenatePropertyName),
                    registration.PropertyName,
                    nameof(PropertyChangedForwardingBehavior));
            }
        }
    }

    /// <summary>
    /// Represents a registration for a forwarding property, containing the property name and a flag indicating whether to concatenate the property name with the owner's property name when forwarding notifications. This record is used to store the necessary information for each registered forwarding property in the registry, allowing for easy management and application of the corresponding behaviors to ObservableObject instances. The PropertyName field specifies the name of the property to forward, while the ConcatenatePropertyName field indicates whether the forwarded notifications should include the owner's property name as a prefix.
    /// </summary>
    /// <param name="PropertyName">The name of the property to forward.</param>
    /// <param name="ConcatenatePropertyName">A flag indicating whether to concatenate the property name with the owner's property name when forwarding notifications.</param>
    private sealed record ForwardPropertyRegistration(string PropertyName, bool ConcatenatePropertyName);
}
