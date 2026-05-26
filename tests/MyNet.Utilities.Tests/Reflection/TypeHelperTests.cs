// -----------------------------------------------------------------------
// <copyright file="TypeHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Reflection;
using Xunit;

namespace MyNet.Utilities.Tests.Reflection;

public class TypeHelperTests
{
    #region GetAssemblyNameWithoutOverhead

    [Fact]
    public void GetAssemblyNameWithoutOverhead_StripsVersionAndToken()
    {
        const string fullName = "MyAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        var result = TypeHelper.GetAssemblyNameWithoutOverhead(fullName);
        Assert.Equal("MyAssembly", result);
    }

    [Fact]
    public void GetAssemblyNameWithoutOverhead_NoCommaReturnsOriginal()
    {
        const string simple = "MyAssembly";
        var result = TypeHelper.GetAssemblyNameWithoutOverhead(simple);
        Assert.Equal("MyAssembly", result);
    }

    #endregion

    #region GetAssemblyName

    [Fact]
    public void GetAssemblyName_ExtractsAssemblyFromFullTypeName()
    {
        const string fullTypeName = "MyNamespace.MyType, MyAssembly";
        var result = TypeHelper.GetAssemblyName(fullTypeName);
        Assert.Equal("MyAssembly", result);
    }

    [Fact]
    public void GetAssemblyName_ReturnsEmptyWhenNoAssembly()
    {
        const string noAssembly = "MyNamespace.MyType";
        var result = TypeHelper.GetAssemblyName(noAssembly);
        Assert.Equal(string.Empty, result);
    }

    #endregion

    #region FormatType

    [Fact]
    public void FormatType_CombinesTypeAndAssembly()
    {
        var result = TypeHelper.FormatType("MyAssembly", "MyNamespace.MyType");
        Assert.Equal("MyNamespace.MyType, MyAssembly", result);
    }

    #endregion

    #region GetTypeName

    [Fact]
    public void GetTypeName_ExtractsTypeFromFullTypeName()
    {
        const string fullTypeName = "MyNamespace.MyType, MyAssembly, Version=1.0.0.0";
        var result = TypeHelper.GetTypeName(fullTypeName);
        Assert.Equal("MyNamespace.MyType", result);
    }

    #endregion

    #region GetTypeNameWithoutNamespace

    [Fact]
    public void GetTypeNameWithoutNamespace_ReturnsShortTypeName()
    {
        const string fullTypeName = "MyNamespace.Sub.MyType, MyAssembly";
        var result = TypeHelper.GetTypeNameWithoutNamespace(fullTypeName);
        Assert.Equal("MyType", result);
    }

    [Fact]
    public void GetTypeNameWithoutNamespace_NoNamespaceReturnsType()
    {
        const string noNamespace = "MyType, MyAssembly";
        var result = TypeHelper.GetTypeNameWithoutNamespace(noNamespace);
        Assert.Equal("MyType", result);
    }

    #endregion

    #region GetTypeNamespace

    [Fact]
    public void GetTypeNamespace_ReturnsNamespace()
    {
        const string fullTypeName = "MyNamespace.Sub.MyType, MyAssembly";
        var result = TypeHelper.GetTypeNamespace(fullTypeName);
        Assert.Equal("MyNamespace.Sub", result);
    }

    #endregion

    #region GetInnerTypes

    [Fact]
    public void GetInnerTypes_SimpleType_ReturnsEmpty()
    {
        var result = TypeHelper.GetInnerTypes("System.String, mscorlib");
        Assert.Empty(result);
    }

    [Fact]
    public void GetInnerTypes_GenericWithOneInnerType_ReturnsOneEntry()
    {
        const string type = "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib";
        var result = TypeHelper.GetInnerTypes(type);
        Assert.Single(result);
        Assert.Contains("System.String", result[0], StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void GetInnerTypes_GenericWithTwoInnerTypes_ReturnsTwoEntries()
    {
        const string type = "System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Int32, mscorlib]], mscorlib";
        var result = TypeHelper.GetInnerTypes(type);
        Assert.Equal(2, result.Length);
    }

    #endregion

    #region FormatInnerTypes

    [Fact]
    public void FormatInnerTypes_WrapsEachTypeInBrackets()
    {
        var innerTypes = new[] { "System.String, mscorlib", "System.Int32, mscorlib" };
        var result = TypeHelper.FormatInnerTypes(innerTypes);
        Assert.Equal("[System.String, mscorlib],[System.Int32, mscorlib]", result);
    }

    [Fact]
    public void FormatInnerTypes_StripAssemblies_RemovesAssemblyPart()
    {
        var innerTypes = new[] { "System.String, mscorlib" };
        var result = TypeHelper.FormatInnerTypes(innerTypes, stripAssemblies: true);
        Assert.Equal("[System.String]", result);
    }

    #endregion

    #region GetTypeNameWithAssembly

    [Fact]
    public void GetTypeNameWithAssembly_StripVersionFromNonMicrosoftAssembly()
    {
        const string full = "MyApp.MyType, MyApp.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null";
        var result = TypeHelper.GetTypeNameWithAssembly(full);

        // Should keep assembly name without version
        Assert.Equal("MyApp.MyType, MyApp.Core", result);
    }

    #endregion

    #region GetTypeFrom

    [Fact]
    public void GetTypeFrom_ReturnsKnownType()
    {
        var type = TypeHelper.GetTypeFrom("System.String");
        Assert.NotNull(type);
        Assert.Equal(typeof(string), type);
    }

    [Fact]
    public void GetTypeFrom_UnknownTypeReturnsNull()
    {
        var type = TypeHelper.GetTypeFrom("NonExistent.Type.That.DoesNotExist");
        Assert.Null(type);
    }

    #endregion

    #region GetAssemblyByName

    [Fact]
    public void GetAssemblyByName_FindsLoadedAssembly()
    {
        var asm = TypeHelper.GetAssemblyByName("MyNet.Utilities");
        Assert.NotNull(asm);
    }

    [Fact]
    public void GetAssemblyByName_UnknownAssemblyReturnsNull()
    {
        var asm = TypeHelper.GetAssemblyByName("Does.Not.Exist");
        Assert.Null(asm);
    }

    #endregion
}
