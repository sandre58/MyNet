// -----------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for working with expressions, particularly for extracting property paths from sorting expressions. These methods are useful for scenarios where you need to infer the property name or path from a lambda expression used for sorting or other purposes. The main method in this class, GetKey, takes a sorting expression and returns the corresponding property path as a string, handling both simple and nested member access expressions. If the expression is not valid, it throws an exception with a clear message to guide the developer on how to use it correctly.
/// </summary>
public static class ExpressionExtensions
{
    extension(Expression? expression)
    {
        /// <summary>
        /// Determines whether two expressions are equal by comparing their string representations.
        /// </summary>
        /// <param name="b">The second expression to compare.</param>
        /// <returns>True if the expressions are similar; otherwise, false.</returns>
        public bool IsSimilar(Expression? b) => expression?.ToString() == b?.ToString();
    }

    extension<T>(Expression<T> expression)
    {
        /// <summary>
        /// Extracts the property path from a sorting expression. The expression should be a simple member access (e.g., x => x.Property) or a nested member access (e.g., x => x.Address.City). The method returns the property path as a string, with nested properties separated by dots (e.g., "Property" or "Address.City"). If the expression is not a valid member access, an InvalidOperationException is thrown, indicating that the sorting key cannot be inferred from the expression and suggesting to use a simple member expression or call WithKey(...) instead.
        /// </summary>
        /// <returns>The property path as a string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the expression is not a valid member access.</exception>
        public string PropertyName()
        {
            var body = expression.Body;

            if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
                body = unary.Operand;

            if (body is not MemberExpression memberExpression)
                throw new InvalidOperationException("Unable to infer sorting key from expression. Use a simple member expression (x => x.Property) or call WithKey(...).");

            var path = new Stack<string>();
            Expression? current = memberExpression;

            while (current is MemberExpression currentMember)
            {
                path.Push(currentMember.Member.Name);
                current = currentMember.Expression;
            }

            return string.Join('.', path);
        }

        /// <summary>
        /// Gets the stack of MemberInfo objects representing the member access path in the given expression, which can be a nested member access expression (e.g., () => obj.Property1.Property2). The stack will contain the MemberInfo objects in the order they are accessed, with the last accessed member on top of the stack. Returns an empty stack if the expression is not a valid member access expression.
        /// </summary>
        /// <returns>The stack of MemberInfo objects representing the member access path, or an empty stack if the expression is not a valid member access expression.</returns>
        public Stack<MemberInfo> GetMembers()
        {
            var stack = new Stack<MemberInfo>();
            var current = expression.Body;

            while (current is not null)
            {
                if (current is MemberExpression member)
                {
                    stack.Push(member.Member);
                    current = member.Expression;
                }
                else if (current is UnaryExpression unary)
                {
                    current = unary.Operand;
                }
                else
                {
                    break;
                }
            }

            return stack;
        }
    }

    extension<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        /// <summary>
        /// Gets PropertyInfo from a given expression that represents a property access. The expression should be a simple member access (e.g., x => x.Property) or a nested member access (e.g., x => x.Address.City). If the expression is not a valid member access, an ArgumentException is thrown, indicating that the expression must be a member access. If the member accessed is not a property, another ArgumentException is thrown, indicating that the member is not a property. This method is useful for scenarios where you need to retrieve metadata about a property based on an expression, such as when building dynamic queries or mapping properties in an object-relational mapper.
        /// </summary>
        /// <returns>The PropertyInfo representing the property accessed by the expression.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is not a valid member access or the member is not a property.</exception>
        public PropertyInfo PropertyInfo => expression.Body is not MemberExpression member
            ? throw new InvalidOperationException("Expression must be a member access.")
            : member.Member as PropertyInfo ?? throw new InvalidOperationException(
                "Member is not a property.");
    }
}
