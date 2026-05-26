// -----------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Metadata;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Retrieves the metadata associated with the specified type from <see cref="MetadataRegistry"/>.
        /// </summary>
        /// <returns>The TypeMetadata associated with the specified type.</returns>
        public TypeMetadata GetMetadata()
        {
            ArgumentNullException.ThrowIfNull(type);

            return MetadataRegistry.Get(type);
        }
    }
}
