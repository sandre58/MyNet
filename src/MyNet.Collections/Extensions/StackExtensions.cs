// -----------------------------------------------------------------------
// <copyright file="StackExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Collections;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class StackExtensions
{
    /// <summary>
    /// Removes the first occurrence of the specified item from the stack. The order of the remaining items is preserved.
    /// </summary>
    /// <param name="stack">The stack from which to remove the item.</param>
    /// <param name="item">The item to remove.</param>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if the stack is null.</exception>
    public static void Remove<T>(this Stack<T> stack, T item)
    {
        ArgumentNullException.ThrowIfNull(stack);

        var temp = new Stack<T>();

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (!Equals(current, item))
                temp.Push(current);
        }

        while (temp.Count > 0)
            stack.Push(temp.Pop());
    }
}
