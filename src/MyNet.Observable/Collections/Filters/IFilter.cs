// -----------------------------------------------------------------------
// <copyright file="IFilter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace MyNet.Observable.Collections.Filters;

/// <summary>
/// Represents a node in a filter expression tree, which can be compiled into a predicate function.
/// </summary>
/// <typeparam name="T">The type of items to be filtered.</typeparam>
public interface IFilter<T>
{
    /// <summary>
    /// Compiles the filter node into a LINQ expression that can be used to filter a collection of items of type T.
    /// </summary>
    /// <returns>A LINQ expression that represents the filter criteria.</returns>
    Expression<Func<T, bool>> ProvideExpression();
}
