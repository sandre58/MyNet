// -----------------------------------------------------------------------
// <copyright file="ValidationGraphVisitor.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using MyNet.Utilities;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Provides functionality to traverse an object graph and validate objects that implement the IValidationAware interface. This class is designed to handle complex object graphs, including those with cycles, by maintaining a set of visited objects to prevent infinite loops. The Validate method performs a depth-first traversal of the object graph starting from a specified root object, invoking the Validate method on any object that implements the IValidationAware interface. The method returns true if all validatable objects in the graph are valid, and false if any object is invalid. This allows for comprehensive validation of an entire object graph with a single method call, ensuring that all relevant objects are checked for validity without the need for manual traversal or cycle detection.
/// </summary>
public static class ValidationGraphVisitor
{
    /// <summary>
    /// Performs a depth-first traversal of the object graph starting from the specified root object, invoking the Validate method on any object that implements the IValidationAware interface. The traversal is designed to handle complex object graphs, including those with cycles, by maintaining a set of visited objects to prevent infinite loops. The method returns true if all objects in the graph that implement IValidationAware are valid, and false if any object is invalid. This allows you to easily validate an entire object graph by simply calling this method with the root object, ensuring that all relevant objects are checked for validity without having to manually traverse the graph or worry about cycles.
    /// </summary>
    /// <param name="root">The root object of the graph to validate.</param>
    /// <returns>True if all validatable objects in the graph are valid; otherwise, false.</returns>
    public static bool Validate(object? root)
    {
        if (root is null)
            return true;

        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        return ValidateInternal(root, visited);
    }

    /// <summary>
    /// Performs a depth-first traversal of the object graph starting from the specified current object, invoking the Validate method on any object that implements the IValidationAware interface. The method uses a HashSet to keep track of visited objects to prevent infinite loops in case of cycles in the graph. It returns true if all validatable objects in the graph are valid, and false if any object is invalid. This internal method is called recursively for each child object in the graph, allowing for a comprehensive validation of the entire object graph starting from the root.
    /// </summary>
    /// <param name="current">The current object being validated.</param>
    /// <param name="visited">A set of objects that have already been visited to prevent infinite loops.</param>
    /// <returns>True if all validatable objects in the graph are valid; otherwise, false.</returns>
    private static bool ValidateInternal(object current, HashSet<object> visited)
    {
        if (!visited.Add(current))
            return true;

        var result = true;

        switch (current)
        {
            case IValidationAware validation:
                result &= validation.Validate();
                break;
            case ObservableObject observableObject when observableObject.TryGetBehavior<IValidationAware>(out var behavior):
                result &= behavior.Validate();
                break;
        }

        TraverseChildren(current, child => result &= ValidateInternal(child, visited));

        return result;
    }

    /// <summary>
    /// Traverses the child objects of the specified current object, invoking the provided visitor action for each child object that is not a simple type or a string. The method uses reflection to get the public properties of the current object and checks their values. If a property value is an IEnumerable (but not a string), it iterates through the items in the collection and invokes the visitor action for each item. For other complex types, it directly invokes the visitor action on the property value. This method allows for a comprehensive traversal of the object graph, ensuring that all relevant child objects are visited while avoiding simple types and strings that do not need to be validated.
    /// </summary>
    /// <param name="current">The current object whose child objects are to be traversed.</param>
    /// <param name="visitor">An action to be invoked for each child object.</param>
    private static void TraverseChildren(object current, Action<object> visitor)
    {
        var properties = current.GetType().GetPublicProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(current);

            if (value is null)
                continue;

            if (property.PropertyType.IsSimple())
                continue;

            switch (value)
            {
                case string:
                    continue;

                case IEnumerable enumerable:
                    {
                        foreach (var item in enumerable)
                        {
                            if (item is null)
                                continue;

                            visitor(item);
                        }

                        break;
                    }

                default:
                    visitor(value);
                    break;
            }
        }
    }
}
