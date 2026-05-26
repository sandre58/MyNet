// -----------------------------------------------------------------------
// <copyright file="PropertyAccessorCacheTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MyNet.Reflection;
using Xunit;

namespace MyNet.Utilities.Tests.Reflection;

public class PropertyAccessorCacheTests
{
    [Fact]
    public void Get_WithSimpleProperty_ReturnsAccessorValue()
    {
        var accessor = PropertyAccessorCache<Person>.Get(nameof(Person.Name));
        var person = new Person { Name = "Alice" };

        Assert.Equal("Alice", accessor(person));
    }

    [Fact]
    public void Get_WithNestedProperty_ReturnsAccessorValue()
    {
        var accessor = PropertyAccessorCache<Person>.Get("Address.City");
        var person = new Person { Address = new() { City = "Paris" } };

        Assert.Equal("Paris", accessor(person));
    }

    [Fact]
    public void Get_CachesAccessor_ForSameProperty()
    {
        var first = PropertyAccessorCache<Person>.Get(nameof(Person.Name));
        var second = PropertyAccessorCache<Person>.Get(nameof(Person.Name));

        Assert.Same(first, second);
    }

    [Fact]
    public void Get_WithUnknownProperty_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => PropertyAccessorCache<Person>.Get("UnknownProperty"));

    private sealed class Person
    {
        public string? Name { get; init; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used for testing nested property access")]
        public Address? Address { get; init; }
    }

    private sealed class Address
    {
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used for testing nested property access")]
        public string? City { get; init; }
    }
}
