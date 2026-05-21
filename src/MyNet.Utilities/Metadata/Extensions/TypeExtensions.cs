// -----------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Metadata;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Retrieves the metadata associated with the specified type. This method uses the MetadataRegistry to obtain the TypeMetadata for the given type, which contains information about the properties, features, and configurations defined for that type. By calling this method on a Type object, you can easily access its metadata and utilize it for various purposes, such as configuring behaviors, applying features, or performing reflection-based operations based on the metadata information. If the provided type is null, an ArgumentNullException will be thrown to indicate that a valid type must be provided to retrieve its metadata.
        /// </summary>
        /// <returns>The TypeMetadata associated with the specified type.</returns>
        public TypeMetadata GetMetadata()
        {
            ArgumentNullException.ThrowIfNull(type);

            return MetadataRegistry.Get(type);
        }
    }
}
