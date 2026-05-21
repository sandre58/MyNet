// -----------------------------------------------------------------------
// <copyright file="ObjectGraphExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using MyNet.Utilities.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ObjectGraphExtensions
{
    extension<T>(T obj)
    {
        /// <summary>
        /// Returns a Deep Clone / Deep Copy of an object of type T using a recursive call to the CloneMethod specified above, with the provided options for cloning behavior.
        /// </summary>
        /// <param name="options">Options for deep copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? DeepClone(DeepCloneOptions? options = null) => ObjectGraphMapper.Clone(obj, options);

        /// <summary>
        /// Populates the fields and properties of the current object with the values from the specified source object of the same type, effectively copying the state from the source to the current object. This method uses reflection to perform a deep copy of the object's graph, ensuring that all nested objects are also copied. If either the current object or the source object is null, an ArgumentNullException is thrown.
        /// </summary>
        /// <param name="source">The source object from which to copy values.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopulateFrom(T source)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ArgumentNullException.ThrowIfNull(source);

            ObjectGraphMapper.Populate(source, obj);
        }
    }
}
