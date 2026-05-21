// -----------------------------------------------------------------------
// <copyright file="MetadataBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace MyNet.Utilities.Metadata;

/// <summary>
/// Provides a builder for constructing metadata for a specific type. This class allows for the fluent configuration of metadata for properties of a type, enabling developers to easily define and manage metadata information associated with the properties of a type. The Property method is used to specify the property for which the metadata is being configured, and it returns a <see cref="PropertyBuilder{T,TProperty}"/> that can be used to further configure the metadata for that property. This builder pattern allows for a clear and concise way to define metadata for properties in a type-safe manner, improving code readability and maintainability when working with metadata in an application.
/// </summary>
/// <typeparam name="T">The type for which metadata is being constructed.</typeparam>
public sealed class MetadataBuilder<T>
{
    private readonly TypeMetadata _metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataBuilder{T}"/> class with the specified type metadata. This constructor is used internally by the metadata registry when constructing metadata for a type, and should not be called directly. The type metadata provides access to the overall metadata for the type, allowing for the organization and management of property-specific metadata within the context of the type's metadata.
    /// </summary>
    /// <param name="metadata">The type metadata that provides access to the overall metadata for the type.</param>
    internal MetadataBuilder(TypeMetadata metadata) => _metadata = metadata;

    /// <summary>
    /// Specifies the property for which metadata is being configured. This method takes an expression that identifies the property of the type for which metadata is being defined. The expression should be a lambda expression that accesses a property of the type, such as x => x.PropertyName. The method extracts the property name from the expression and retrieves the corresponding property metadata from the type metadata. It then returns a <see cref="PropertyBuilder{T,TProperty}"/> that can be used to further configure the metadata for that specific property, allowing for various runtime behaviors based on the configured metadata information for that property in an application.
    /// </summary>
    /// <param name="expression">A lambda expression that identifies the property being configured.</param>
    /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
    /// <returns>A <see cref="PropertyBuilder{T,TProperty}"/> that can be used to further configure the metadata for the specified property.</returns>
    public PropertyBuilder<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var name = expression.PropertyInfo.Name;
        return new(_metadata.GetProperty(name));
    }
}
