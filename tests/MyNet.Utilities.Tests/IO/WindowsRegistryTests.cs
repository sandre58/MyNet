// -----------------------------------------------------------------------
// <copyright file="WindowsRegistryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.Win32;
using MyNet.Utilities.IO.Registry;
using MyNet.Utilities.IO.Registry.Windows;
using Xunit;

namespace MyNet.Utilities.Tests.IO;

[SupportedOSPlatform("windows")]
public sealed class WindowsRegistryTests : IDisposable
{
    private readonly RegistryPath _rootPath = RegistryPath.Combine("Software", "MyNetUtilitiesTests_" + Guid.NewGuid().ToString("N"));
    private readonly RegistryValueConverter _converter = new();
    private readonly WindowsRegistryStore _store;
    private readonly WindowsRegistryNavigator _navigator = new();
    private readonly WindowsRegistryQuery _query;
    private readonly WindowsRegistryService _service;

    public WindowsRegistryTests()
    {
        _store = new(_converter);
        _query = new(_converter);
        _service = new(_store, _navigator, _converter);
    }

    [Fact]
    public void RegistryValueConverter_GetKind_ReturnsExpectedKinds()
    {
        Assert.Equal(RegistryValueKind.DWord, _converter.GetKind(typeof(bool)));
        Assert.Equal(RegistryValueKind.String, _converter.GetKind(typeof(TestStatus)));
        Assert.Equal(RegistryValueKind.Binary, _converter.GetKind(typeof(byte[])));
    }

    [Fact]
    public void RegistryValueConverter_ConvertToAndFrom_RoundTripsSpecialTypes()
    {
        var guid = Guid.NewGuid();
        var date = DateTimeOffset.UtcNow;

        Assert.Equal(guid, _converter.ConvertFrom(_converter.ConvertTo(guid), typeof(Guid)));
        Assert.Equal(date, _converter.ConvertFrom(_converter.ConvertTo(date), typeof(DateTimeOffset)));
        Assert.Equal(TestStatus.Active, _converter.ConvertFrom(_converter.ConvertTo(TestStatus.Active), typeof(TestStatus)));
    }

    [Fact]
    public void WindowsRegistryStore_SetGetExistsAndRemove_RoundTripsValue()
    {
        var path = RegistryPath.Combine(_rootPath.ToString(), "StoreEntry");

        _store.Set(path, "Enabled", true);

        Assert.True(_store.Exists(path));
        Assert.True(_store.Get<bool>(path, "Enabled"));

        _store.Remove(path);

        Assert.False(_store.Exists(path));
    }

    [Fact]
    public void WindowsRegistryNavigator_GetChildrenAndCount_ReturnExpectedChildren()
    {
        var first = RegistryPath.Combine(_rootPath.ToString(), "Child1");
        var second = RegistryPath.Combine(_rootPath.ToString(), "Child2");
        _store.Set(first, "Value", "one");
        _store.Set(second, "Value", "two");

        var children = _navigator.GetChildren(_rootPath).ToArray();

        Assert.Equal(2, _navigator.Count(_rootPath));
        Assert.Contains(children, x => x.ToString().EndsWith("Child1", StringComparison.Ordinal));
        Assert.Contains(children, x => x.ToString().EndsWith("Child2", StringComparison.Ordinal));
    }

    [Fact]
    public void WindowsRegistryQuery_FindByValue_ReturnsMatchingPath()
    {
        var first = RegistryPath.Combine(_rootPath.ToString(), "One");
        var second = RegistryPath.Combine(_rootPath.ToString(), "Two");
        _store.Set(first, "Name", "Alpha");
        _store.Set(second, "Name", "Beta");

        var result = _query.FindByValue(_rootPath, "Name", "Beta");

        Assert.NotNull(result);
        Assert.Equal(second.ToString(), result.Value.ToString(), StringComparer.Ordinal);
    }

    [Fact]
    public void WindowsRegistryService_AddOrUpdate_Get_And_GetAll_WorkTogether()
    {
        var firstPath = RegistryPath.Combine(_rootPath.ToString(), "Entry1");
        var secondPath = RegistryPath.Combine(_rootPath.ToString(), "Entry2");

        _service.AddOrUpdate(new RegistryEntry<TestRegistryItem>(firstPath, new() { Name = "One", Count = 1, IsEnabled = true }));
        _service.AddOrUpdate(new RegistryEntry<TestRegistryItem>(secondPath, new() { Name = "Two", Count = 2, IsEnabled = false }));

        var single = _service.Get<TestRegistryItem>(firstPath);
        var all = _service.GetAll<TestRegistryItem>(_rootPath).ToArray();

        Assert.NotNull(single);
        Assert.Equal("One", single.Item.Name, StringComparer.Ordinal);
        Assert.Equal(1, single.Item.Count);
        Assert.True(single.Item.IsEnabled);
        Assert.Equal(2, all.Length);
        Assert.Contains(all, x => x.Item.Name == "Two");
    }

    public void Dispose() => Registry.CurrentUser.DeleteSubKeyTree(_rootPath.ToString(), false);

    private enum TestStatus
    {
        Active
    }

    private sealed class TestRegistryItem
    {
        public string Name { get; init; } = string.Empty;

        public int Count { get; init; }

        public bool IsEnabled { get; init; }
    }
}
