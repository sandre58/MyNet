// -----------------------------------------------------------------------
// <copyright file="PropertyValueAccess.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Safe property reads for behavior initialization while derived constructors may still be running.
/// </summary>
internal static class PropertyValueAccess
{
    /// <summary>
    /// Tries to read a property value without surfacing exceptions from getters that depend on uninitialized state.
    /// </summary>
    public static bool TryGetValue(object target, PropertyInfo property, out object? value)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(property);

        try
        {
            value = property.GetValue(target);
            return true;
        }
        catch (Exception ex) when (IsBenignGetterFailure(ex))
        {
            value = null;
            return false;
        }
    }

    private static bool IsBenignGetterFailure(Exception exception)
        => exception is NullReferenceException or InvalidOperationException
            || (exception is TargetInvocationException { InnerException: { } inner } && IsBenignGetterFailure(inner));
}
