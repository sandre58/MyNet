// -----------------------------------------------------------------------
// <copyright file="ReflectionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace MyNet.Reflection.Tests;

public class ReflectionExtensionsTests
{
    [Fact]
    public void GetPublicProperties_Should_Return_Independent_Array_Instances()
    {
        var first = typeof(PropertyBag).GetPublicProperties();
        first[0] = typeof(ExternalPropertyBag).GetProperty(nameof(ExternalPropertyBag.ExternalOnly))!;

        var second = typeof(PropertyBag).GetPublicProperties();

        Assert.All(second, property => Assert.Equal(typeof(PropertyBag), property.DeclaringType));
        Assert.Contains(second, property => property.Name == nameof(PropertyBag.Name));
        Assert.Contains(second, property => property.Name == nameof(PropertyBag.Count));
    }

    [Fact]
    public void GetPublicPropertiesWithAttribute_Should_Return_Independent_Array_Instances()
    {
        var first = typeof(AttributedPropertyBag).GetPublicPropertiesWithAttribute<DescriptionAttribute>();
        first[0] = typeof(ExternalPropertyBag).GetProperty(nameof(ExternalPropertyBag.ExternalOnly))!;

        var second = typeof(AttributedPropertyBag).GetPublicPropertiesWithAttribute<DescriptionAttribute>();

        Assert.Single(second);
        Assert.Equal(nameof(AttributedPropertyBag.Marked), second[0].Name);
        Assert.DoesNotContain(second, property => property.Name == nameof(AttributedPropertyBag.Unmarked));
    }

    [Fact]
    public void GetPropertyMap_Should_Return_Independent_Dictionary_Instances()
    {
        var first = typeof(PropertyBag).GetPropertyMap();
        first.Clear();

        var second = typeof(PropertyBag).GetPropertyMap();

        Assert.True(second.ContainsKey(nameof(PropertyBag.Name)));
        Assert.True(second.ContainsKey(nameof(PropertyBag.Count)));
    }

    [Fact]
    public void GetDeepPropertyValue_Should_Return_Nested_Value()
    {
        var root = new NestedParent { Child = new() { Name = "value" } };
        Assert.Equal("value", root.Child!.Name);

        var result = root.GetDeepPropertyValue<string>("Child.Name");

        Assert.Equal("value", result);
    }

    [Fact]
    public void GetDeepPropertyValue_Should_Return_Null_For_Invalid_Path()
    {
        var root = new NestedParent { Child = new() { Name = "value" } };
        Assert.Equal("value", root.Child!.Name);

        var result = root.GetDeepPropertyValue("Child.Unknown");

        Assert.Null(result);
    }

    [Fact]
    public void GetAttribute_Should_Return_Attribute_For_Defined_Enum_Value()
    {
        var attribute = TestEnum.Value.GetAttribute<DescriptionAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Value description", attribute.Description);
    }

    [Fact]
    public void GetAttribute_Should_Return_Null_For_Undefined_Enum_Value()
    {
        var attribute = ((TestEnum)42).GetAttribute<DescriptionAttribute>();

        Assert.Null(attribute);
    }

    [Fact]
    public void GetAttribute_Should_Return_Null_For_Combined_Flags_Without_Named_Field()
    {
        var attribute = (TestFlags.Read | TestFlags.Write).GetAttribute<DescriptionAttribute>();

        Assert.Null(attribute);
    }

    [Fact]
    public void GetValuesOfType_Should_Return_Only_Matching_Non_Null_Values()
    {
        var properties = typeof(ValueContainer).GetPublicProperties();
        var instance = new ValueContainer { Name = "alpha", Optional = null, Count = 5, Value = "boxed" };

        Assert.Equal("alpha", instance.Name);
        Assert.Null(instance.Optional);
        Assert.Equal(5, instance.Count);
        Assert.Equal("boxed", instance.Value);

        var values = properties.GetValuesOfType<string>(instance).ToList();

        Assert.Equal(["alpha"], values);
    }

    [Fact]
    public void GetValuesOfType_Should_Return_Empty_When_Instance_Is_Null()
    {
        var properties = typeof(ValueContainer).GetPublicProperties();

        var values = properties.GetValuesOfType<string>(null).ToList();

        Assert.Empty(values);
    }

    private sealed class PropertyBag
    {
        public string Name { get; set; } = string.Empty;

        public int Count { get; set; }
    }

    private sealed class AttributedPropertyBag
    {
        [Description("Marked")]
        public string Marked { get; set; } = string.Empty;

        public string Unmarked { get; set; } = string.Empty;
    }

    private sealed class ExternalPropertyBag
    {
        public string ExternalOnly { get; set; } = string.Empty;
    }

    private sealed class NestedParent
    {
        public NestedChild? Child { get; init; }
    }

    private sealed class NestedChild
    {
        public string? Name { get; init; }
    }

    private sealed class ValueContainer
    {
        public string? Name { get; init; }

        public string? Optional { get; init; }

        public int Count { get; init; }

        public object? Value { get; init; }
    }

    private enum TestEnum
    {
        [Description("Value description")]
        Value = 1
    }

    [Flags]
    [SuppressMessage("Roslynator", "RCS1135:Declare enum member with zero value (when enum has FlagsAttribute)", Justification = "Testing GetAttribute behavior with combined flags that don't have a named field.")]
    private enum TestFlags
    {
        [Description("Read")]
        Read = 1,

        [Description("Write")]
        Write = 2
    }
}
