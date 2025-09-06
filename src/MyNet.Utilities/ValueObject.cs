// -----------------------------------------------------------------------
// <copyright file="ValueObject.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyNet.Utilities.Attributes;

namespace MyNet.Utilities;

/// <summary>
/// Base class for value objects that provides equality semantics based on property and field values.
/// Value objects are compared by their values rather than their identity.
/// Properties and fields marked with <see cref="IgnoreMemberAttribute"/> are excluded from equality comparison.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    private List<PropertyInfo>? _properties;
    private List<FieldInfo>? _fields;

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    public static bool operator ==(ValueObject? obj1, ValueObject? obj2) => obj1?.Equals(obj2) ?? Equals(obj2, null);

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    public static bool operator !=(ValueObject? obj1, ValueObject? obj2) => !(obj1 == obj2);

    /// <summary>
    /// Determines whether the current value object is equal to another value object.
    /// </summary>
    /// <param name="other">The value object to compare with.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise <c>false</c>.</returns>
    public virtual bool Equals(ValueObject? other) => Equals(other as object);

    /// <summary>
    /// Determines whether the current value object is equal to the specified object.
    /// Equality is based on comparing all public properties and fields that are not marked with <see cref="IgnoreMemberAttribute"/>.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj)
        => !(obj == null || GetType() != obj.GetType())
           && GetProperties().TrueForAll(p => PropertiesAreEqual(obj, p))
           && GetFields().TrueForAll(f => FieldsAreEqual(obj, f));

    /// <summary>
    /// Returns a hash code for the current value object based on all included properties and fields.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        var hash = GetProperties().Select(prop => prop.GetValue(this, null)).Aggregate(17, HashValue);

        return GetFields().Select(field => field.GetValue(this)).Aggregate(hash, HashValue);
    }

    private static int HashValue(int seed, object? value)
    {
        var currentHash = value?.GetHashCode() ?? 0;

        return (seed * 23) + currentHash;
    }

    private bool PropertiesAreEqual(object? obj, PropertyInfo p) => Equals(p.GetValue(this, null), p.GetValue(obj, null));

    private bool FieldsAreEqual(object? obj, FieldInfo f) => Equals(f.GetValue(this), f.GetValue(obj));

    private List<PropertyInfo> GetProperties()
    {
        _properties ??= [.. GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => !Attribute.IsDefined(p, typeof(IgnoreMemberAttribute)))];

        return _properties;
    }

    private List<FieldInfo> GetFields()
    {
        _fields ??= [.. GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(f => !Attribute.IsDefined(f, typeof(IgnoreMemberAttribute)))];

        return _fields;
    }
}
