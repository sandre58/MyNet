// -----------------------------------------------------------------------
// <copyright file="ReflectionComparerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Xunit;

namespace MyNet.Reflection.Tests;

public class ReflectionComparerTests
{
    [Fact]
    public void Compare_ObjectInterfaceImplementation_NullValues_ReturnsZero()
    {
        // Arrange
        var sortDescriptions = new List<ReflectionSortDescription>();
        var comparer = new ReflectionComparer<object>(sortDescriptions);

        // Act
        var result = comparer.Compare(null, null);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Compare_ObjectInterfaceImplementation_OneValueIsNull_ReturnsOne()
    {
        // Arrange
        var sortDescriptions = new List<ReflectionSortDescription>
        {
            new("Length") // Assuming "Length" property exists in string class
        };
        var comparer = new ReflectionComparer<object>(sortDescriptions);

        // Act
        var result = comparer.Compare("test", null);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Compare_ObjectInterfaceImplementation_BothValuesAreNotNull_ReturnsComparisonResult()
    {
        // Arrange
        var sortDescriptions = new List<ReflectionSortDescription>
        {
            new("Length") // Assuming "Length" property exists in string class
        };
        var comparer = new ReflectionComparer<string>(sortDescriptions);

        // Act
        var result = comparer.Compare("test", "abc");

        // Assert
        Assert.True(result > 0); // "test" is longer than "abcd"
    }

    [Fact]
    public void Compare_GenericInterfaceImplementation_NullValues_ReturnsZero()
    {
        // Arrange
        var sortDescriptions = new List<ReflectionSortDescription>
        {
            new("Length") // Assuming "Length" property exists in string class
        };
        var comparer = new ReflectionComparer<string>(sortDescriptions);

        // Act
        var result = comparer.Compare(null, null);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Compare_GenericInterfaceImplementation_OneValueIsNull_ReturnsOne()
    {
        // Arrange
        var sortDescriptions = new List<ReflectionSortDescription>
        {
            new("Length") // Assuming "Length" property exists in string class
        };
        var comparer = new ReflectionComparer<string>(sortDescriptions);

        // Act
        var result = comparer.Compare("test", null);

        // Assert
        Assert.Equal(1, result);
    }
}
