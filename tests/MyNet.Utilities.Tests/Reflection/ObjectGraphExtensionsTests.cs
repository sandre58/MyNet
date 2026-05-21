// -----------------------------------------------------------------------
// <copyright file="ObjectGraphExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Utilities.Tests.Reflection;

public class ObjectGraphExtensionsTests
{
    [Fact]
    public void DeepClone_Should_Preserve_Shared_References_By_Default()
    {
        var shared = new ChildNode { Name = "shared" };
        var source = new SharedReferenceContainer { First = shared, Second = shared };

        var clone = source.DeepClone();

        Assert.NotNull(clone);
        Assert.NotSame(source, clone);
        Assert.NotNull(clone.First);
        Assert.Same(clone.First, clone.Second);
        Assert.NotSame(shared, clone.First);
        Assert.Equal("shared", clone.First!.Name);
    }

    [Fact]
    public void DeepClone_Should_Not_Preserve_Shared_References_When_Disabled()
    {
        var shared = new ChildNode { Name = "shared" };
        var source = new SharedReferenceContainer { First = shared, Second = shared };

        var clone = source.DeepClone(new() { PreserveReferences = false });

        Assert.NotNull(clone);
        Assert.NotNull(clone.First);
        Assert.NotNull(clone.Second);
        Assert.NotSame(clone.First, clone.Second);
        Assert.Equal("shared", clone.First!.Name);
        Assert.Equal("shared", clone.Second!.Name);
    }

    [Fact]
    public void DeepClone_Should_Preserve_Circular_References_By_Default()
    {
        var source = new CircularNode();
        source.Next = source;

        var clone = source.DeepClone();

        Assert.NotNull(clone);
        Assert.Same(clone, clone.Next);
    }

    [Fact]
    public void PopulateFrom_Should_Copy_Source_Values_Into_Value_Type_Array()
    {
        var source = new[] { 1, 2, 3 };
        var target = new[] { 9, 9, 9 };

        target.PopulateFrom(source);

        Assert.Equal(source, target);
    }

    [Fact]
    public void PopulateFrom_Should_Copy_Source_Values_Into_Multidimensional_Array()
    {
#pragma warning disable CA1814 // Test explicite d'un tableau multidimensionnel
        var source = new[,]
        {
            { 1, 2 },
            { 3, 4 }
        };
        var target = new[,]
        {
            { 9, 9 },
            { 9, 9 }
        };
#pragma warning restore CA1814

        target.PopulateFrom(source);

        Assert.Equal(1, target[0, 0]);
        Assert.Equal(2, target[0, 1]);
        Assert.Equal(3, target[1, 0]);
        Assert.Equal(4, target[1, 1]);
    }

    [Fact]
    public void PopulateFrom_Should_Preserve_Shared_References_In_Array_By_Default()
    {
        var shared = new ChildNode { Name = "shared" };
        var source = new[] { shared, shared };
        var target = new[] { new ChildNode(), new ChildNode() };

        target.PopulateFrom(source);

        Assert.NotNull(target[0]);
        Assert.Same(target[0], target[1]);
        Assert.NotSame(shared, target[0]);
        Assert.Equal("shared", target[0].Name);
    }

    [Fact]
    public void PopulateFrom_Should_Preserve_Shared_References_In_Object_Graph_By_Default()
    {
        var shared = new ChildNode { Name = "shared" };
        var source = new SharedReferenceContainer { First = shared, Second = shared };
        var target = new SharedReferenceContainer { First = new(), Second = new() };

        target.PopulateFrom(source);

        Assert.NotNull(target.First);
        Assert.Same(target.First, target.Second);
        Assert.NotSame(shared, target.First);
        Assert.Equal("shared", target.First!.Name);
    }

    private sealed class SharedReferenceContainer
    {
        public ChildNode? First { get; init; }

        public ChildNode? Second { get; init; }
    }

    private sealed class ChildNode
    {
        public string? Name { get; init; }
    }

    private sealed class CircularNode
    {
        public CircularNode? Next { get; set; }
    }
}
