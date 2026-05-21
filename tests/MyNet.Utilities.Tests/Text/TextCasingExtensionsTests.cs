// -----------------------------------------------------------------------
// <copyright file="TextCasingExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Utilities.Text.TextCasing;
using Xunit;
using TextPortal = MyNet.Utilities.Text.Text;

namespace MyNet.Utilities.Tests.Text;

public class TextCasingExtensionsTests
{
    [Theory]
    [InlineData("HelloWorld", "hello_world")]
    [InlineData("hello world", "hello_world")]
    [InlineData("SomeHTTPValue", "some_http_value")]
    public void ToSnakeCase_UsesSnakeCaseTransform(string input, string expected) => Assert.Equal(expected, input.ToSnakeCase());

    [Theory]
    [InlineData("HelloWorld", "hello-world")]
    [InlineData("hello world", "hello-world")]
    [InlineData("SomeHTTPValue", "some-http-value")]
    public void ToKebabCase_UsesKebabCaseTransform(string input, string expected) => Assert.Equal(expected, input.ToKebabCase());

    [Theory]
    [InlineData("hello_world", "HelloWorld")]
    [InlineData("hello world", "HelloWorld")]
    [InlineData("alreadyPascal", "AlreadyPascal")]
    public void Pascalize_UsesPascalCaseTransform(string input, string expected) => Assert.Equal(expected, input.ToPascalCase());

    [Theory]
    [InlineData("hello_world", "helloWorld")]
    [InlineData("hello world", "helloWorld")]
    [InlineData("alreadyPascal", "alreadyPascal")]
    public void Camelize_UsesCamelCaseTransform(string input, string expected) => Assert.Equal(expected, input.ToCamelCase());

    [Fact]
    public void Casing_PascalCase_And_CamelCase_AreAvailableAsTransforms()
    {
        Assert.Equal("HelloWorld", Casing.PascalCase.Apply("hello_world", CultureInfo.InvariantCulture));
        Assert.Equal("helloWorld", Casing.CamelCase.Apply("hello_world", CultureInfo.InvariantCulture));
        Assert.Equal("hello_world", Casing.SnakeCase.Apply("HelloWorld", CultureInfo.InvariantCulture));
        Assert.Equal("hello-world", Casing.KebabCase.Apply("HelloWorld", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Pipeline_PascalCase_And_CamelCase_Work()
    {
        var pascal = TextPortal.For("hello_world", CultureInfo.InvariantCulture).PascalCase().Value;
        var camel = TextPortal.For("hello_world", CultureInfo.InvariantCulture).CamelCase().Value;
        var snake = TextPortal.For("HelloWorld", CultureInfo.InvariantCulture).SnakeCase().Value;
        var kebab = TextPortal.For("HelloWorld", CultureInfo.InvariantCulture).KebabCase().Value;

        Assert.Equal("HelloWorld", pascal);
        Assert.Equal("helloWorld", camel);
        Assert.Equal("hello_world", snake);
        Assert.Equal("hello-world", kebab);
    }
}
