// -----------------------------------------------------------------------
// <copyright file="StackExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Xunit;

namespace MyNet.Utilities.Tests.Collections;

public class StackExtensionsTests
{
    private static Stack<int> MakeStack(params int[] items)
    {
        var stack = new Stack<int>();

        for (var i = items.Length - 1; i >= 0; i--)
            stack.Push(items[i]);

        return stack;
    }

    [Fact]
    public void Remove_RemovesFirstOccurrenceAndPreservesOrder()
    {
        var stack = MakeStack(1, 2, 3, 4, 5);

        stack.Remove(3);
        var items = stack.ToArray();

        Assert.Equal(4, stack.Count);
        Assert.DoesNotContain(3, items);
        Assert.Equal([1, 2, 4, 5], items);
    }

    [Fact]
    public void Remove_DoesNothingWhenItemNotPresent()
    {
        var stack = MakeStack(1, 2, 3);
        var countBefore = stack.Count;
        stack.Remove(99);
        Assert.Equal(countBefore, stack.Count);
    }

    [Fact]
    public void Remove_WorksOnEmptyStack()
    {
        var stack = new Stack<int>();

        stack.Remove(1);
        Assert.Empty(stack);
    }

    [Fact]
    public void Remove_RemovesAllOccurrencesOfDuplicates()
    {
        // The implementation removes ALL occurrences (not just the first)
        var stack = MakeStack(1, 2, 2, 3);
        stack.Remove(2);

        Assert.Equal(2, stack.Count);
        Assert.DoesNotContain(2, stack.ToArray());
    }

    [Fact]
    public void Remove_NullStackThrowsArgumentNullException()
    {
        Stack<int> stack = null!;
        Assert.Throws<ArgumentNullException>(() => stack.Remove(1));
    }
}
