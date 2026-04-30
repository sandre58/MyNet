// -----------------------------------------------------------------------
// <copyright file="IGroupingProperty.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace MyNet.Observable.Collections.Grouping;

/// <summary>
/// Defines a grouping property for type T, including the expression to access the property to group by.
/// </summary>
/// <typeparam name="T">The type of items to be grouped.</typeparam>
public interface IGroupingProperty<T>
{
    /// <summary>
    /// Provides an expression that accesses the property of type T to be used for grouping.
    /// </summary>
    /// <returns>An expression that represents the property to group by.</returns>
    Expression<Func<T, object?>> ProvideExpression();
}
