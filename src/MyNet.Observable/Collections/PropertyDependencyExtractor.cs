// -----------------------------------------------------------------------
// <copyright file="PropertyDependencyExtractor.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MyNet.Observable.Collections;

/// <summary>
/// Extracts the names of properties accessed in filter and sort expressions for items of type T.
/// </summary>
/// <typeparam name="T">The type of items managed by the extended collection.</typeparam>
public sealed class PropertyDependencyExtractor<T>
{
    /// <summary>
    /// Extracts the names of properties accessed in the given expression by traversing the expression tree and collecting property names from member access expressions, binary expressions, unary expressions, and method call expressions. The resulting set contains the unique names of properties that are accessed in the expression, which can be used to determine dependencies for filtering and sorting operations.
    /// </summary>
    /// <param name="expression">The expression to analyze.</param>
    /// <returns>A set of property names accessed in the expression.</returns>
    private static HashSet<string> Extract(Expression expression)
    {
        var set = new HashSet<string>();
        Visit(expression, set);
        return set;
    }

    /// <summary>
    /// Recursively visits the given expression and its sub-expressions to extract property names. The method handles different types of expressions, including member access expressions (to extract property names), binary expressions (to visit both left and right sub-expressions), unary expressions (to visit the operand), and method call expressions (to visit all arguments). The extracted property names are added to the provided set, which accumulates the unique property names accessed in the expression tree.
    /// </summary>
    /// <param name="expr">The expression to visit.</param>
    /// <param name="set">The set to which extracted property names are added.</param>
    private static void Visit(Expression expr, HashSet<string> set)
    {
        while (true)
        {
            switch (expr)
            {
                case MemberExpression { Member.MemberType: MemberTypes.Property } m:
                    set.Add(m.Member.Name);
                    expr = m.Expression!;
                    continue;

                case BinaryExpression b:
                    Visit(b.Left, set);
                    expr = b.Right;
                    continue;

                case UnaryExpression u:
                    expr = u.Operand;
                    continue;

                case MethodCallExpression c:
                    foreach (var a in c.Arguments) Visit(a, set);
                    break;
            }

            break;
        }
    }

    /// <summary>
    /// Extracts the names of properties accessed in the given filter expression by analyzing its expression tree. The method uses the Extract method to traverse the expression tree and collect property names from member access expressions, binary expressions, unary expressions, and method call expressions. The resulting set contains the unique names of properties that are accessed in the filter expression, which can be used to determine dependencies for filtering operations.
    /// </summary>
    /// <param name="expression">The filter expression to analyze.</param>
    /// <returns>A set of property names accessed in the filter expression.</returns>
    public IReadOnlySet<string> ExtractFilter(Expression<Func<T, bool>> expression) => Extract(expression.Body);

    /// <summary>
    /// Extracts the names of properties accessed in the given sort expression by analyzing its expression tree. The method uses the Extract method to traverse the expression tree and collect property names from member access expressions, binary expressions, unary expressions, and method call expressions. The resulting set contains the unique names of properties that are accessed in the sort expression, which can be used to determine dependencies for sorting operations.
    /// </summary>
    /// <param name="expression">The sort expression to analyze.</param>
    /// <returns>A set of property names accessed in the sort expression.</returns>
    public IReadOnlySet<string> ExtractSort(Expression<Func<T, object?>> expression) => Extract(expression.Body);
}
