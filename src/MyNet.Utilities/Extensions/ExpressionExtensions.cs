// -----------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ExpressionExtensions
{
    public static string GetKey<T>(this Expression<Func<T, object?>> sortExpression)
    {
        var body = sortExpression.Body;

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
}
