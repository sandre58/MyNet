// -----------------------------------------------------------------------
// <copyright file="ObjectGraphMapper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyNet.Reflection;

internal static class ObjectGraphMapper
{
    private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!;

    /// <summary>
    /// Creates a deep clone of the specified object, recursively copying all fields and properties. If the <paramref name="options"/> parameter is provided with reference preservation enabled, the method will maintain object references to handle circular references and shared instances correctly. If the input object is null, the method returns null.
    /// </summary>
    /// <param name="obj">The object to clone.</param>
    /// <param name="options">The options for deep cloning.</param>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <returns>A deep clone of the specified object.</returns>
    public static T? Clone<T>(T? obj, DeepCloneOptions? options = null)
    {
        if (obj is null)
            return default;

        var visited = ShouldPreserveReferences(options)
            ? new Dictionary<object, object?>(ReferenceEqualityComparer.Instance)
            : null;

        return (T?)CloneInternal(obj, visited);
    }

    /// <summary>
    /// Recursively clones the specified object, handling reference preservation if a visited dictionary is provided. The method checks for simple types and delegates to avoid unnecessary cloning, and it uses reflection to copy fields for complex objects. For arrays, it handles multi-dimensional arrays by recursively cloning each element. If the input object is null, the method returns null.
    /// </summary>
    /// <param name="obj">The object to clone.</param>
    /// <param name="visited">A dictionary to track visited objects for reference preservation.</param>
    /// <returns>A deep clone of the specified object.</returns>
    private static object? CloneInternal(object? obj, IDictionary<object, object?>? visited)
    {
        if (obj is null)
            return null;

        var type = obj.GetType();

        if (type.IsSimple())
            return obj;

        if (visited != null && visited.TryGetValue(obj, out var existing))
            return existing;

        if (obj is Delegate)
            return obj;

        var clone = CloneMethod.Invoke(obj, null);

        visited?[obj] = clone;

        if (obj is Array sourceArray && clone is Array clonedArray)
        {
            CopyArrayElements(sourceArray, clonedArray, visited);
            return clone;
        }

        CopyFields(type, obj, clone, visited);

        return clone;
    }

    /// <summary>
    /// Copies the fields of the specified type from the source object to the target object, recursively cloning field values as needed. The method skips fields marked with the <see cref="NonSerializedAttribute"/> and handles both public and non-public instance fields. If a visited dictionary is provided, it will be used to preserve object references during cloning. The method continues to copy fields from base types until it reaches the top of the inheritance hierarchy.
    /// </summary>
    /// <param name="type">The type whose fields are to be copied.</param>
    /// <param name="source">The source object from which to copy field values.</param>
    /// <param name="target">The target object to which field values are copied.</param>
    /// <param name="visited">A dictionary to track visited objects for reference preservation.</param>
    private static void CopyFields(Type? type, object source, object? target, IDictionary<object, object?>? visited)
    {
        while (type != null)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(NonSerializedAttribute), true))
                    continue;

                var value = field.GetValue(source);
                var cloned = CloneInternal(value, visited);

                field.SetValue(target, cloned);
            }

            type = type.BaseType;
        }
    }

    /// <summary>
    /// Recursively copies the elements of an array from the source to the target, cloning each element as needed while preserving object references when a visited dictionary is provided.
    /// </summary>
    /// <param name="source">The source array whose values are copied.</param>
    /// <param name="target">The target array that receives the cloned values.</param>
    /// <param name="visited">A dictionary to track visited objects for reference preservation.</param>
    private static void CopyArrayElements(Array source, Array target, IDictionary<object, object?>? visited)
    {
        if (source.Rank != target.Rank)
            throw new ArgumentException("Source and target arrays must have the same rank.", nameof(target));

        var indices = new int[source.Rank];

        copyDimension(0);
        return;

        void copyDimension(int dimension)
        {
            var sourceLowerBound = source.GetLowerBound(dimension);
            var sourceUpperBound = source.GetUpperBound(dimension);

            if (sourceLowerBound != target.GetLowerBound(dimension) || sourceUpperBound != target.GetUpperBound(dimension))
                throw new ArgumentException("Source and target arrays must have the same bounds.", nameof(target));

            for (var i = sourceLowerBound; i <= sourceUpperBound; i++)
            {
                indices[dimension] = i;

                if (dimension < source.Rank - 1)
                {
                    copyDimension(dimension + 1);
                }
                else
                {
                    var value = source.GetValue(indices);
                    var cloned = CloneInternal(value, visited);
                    target.SetValue(cloned, indices);
                }
            }
        }
    }

    /// <summary>
    /// Recursively populates the fields of the destination object with values from the source object, handling reference preservation if a visited dictionary is provided. The method checks for simple types and delegates to avoid unnecessary copying, and it uses reflection to copy fields for complex objects. For arrays, it handles multi-dimensional arrays by recursively populating each element. The method continues to populate fields from base types until it reaches the top of the inheritance hierarchy.
    /// </summary>
    /// <param name="source">The source object from which to copy field values.</param>
    /// <param name="destination">The destination object to which field values are copied.</param>
    /// <param name="options">Options to control the cloning behavior.</param>
    public static void Populate(object source, object destination, DeepCloneOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var visited = ShouldPreserveReferences(options)
            ? new Dictionary<object, object?>(ReferenceEqualityComparer.Instance)
            : null;

        PopulateInternal(source, destination, visited);
    }

    /// <summary>
    /// Recursively populates the fields of the destination object with values from the source object, handling reference preservation if a visited dictionary is provided. The method checks for simple types and delegates to avoid unnecessary copying, and it uses reflection to copy fields for complex objects. For arrays, it handles multi-dimensional arrays by recursively populating each element. The method continues to populate fields from base types until it reaches the top of the inheritance hierarchy.
    /// </summary>
    /// <param name="source">The source object from which to copy field values.</param>
    /// <param name="target">The target object to which field values are copied.</param>
    /// <param name="visited">A dictionary to track visited objects for reference preservation.</param>
    public static void PopulateInternal(object source, object target, IDictionary<object, object?>? visited)
    {
        var type = source.GetType();

        if (type.IsSimple())
            return;

        if (visited?.TryGetValue(source, out _) == true)
            return;

        if (source is Delegate)
            return;

        visited?[source] = target;

        if (source is Array sourceArray && target is Array targetArray)
        {
            CopyArrayElements(sourceArray, targetArray, visited);
            return;
        }

        CopyFields(type, source, target, visited);
    }

    private static bool ShouldPreserveReferences(DeepCloneOptions? options) => options?.PreserveReferences ?? true;
}
