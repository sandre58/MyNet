// -----------------------------------------------------------------------
// <copyright file="DirectoryServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using MyNet.Utilities.IO;
using Xunit;

namespace MyNet.Utilities.Tests.IO;

public sealed class DirectoryServiceTests
{
    [Fact]
    public void Constructor_ExpandsEnvironmentVariables_AndCreatesRootDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Environment.SetEnvironmentVariable("MYNET_IO_TEST_ROOT", root);

        try
        {
            var service = new DirectoryService("%MYNET_IO_TEST_ROOT%", NullLogger<DirectoryService>.Instance);

            Assert.Equal(root, service.RootDirectory, StringComparer.OrdinalIgnoreCase);
            Assert.True(Directory.Exists(service.RootDirectory));
        }
        finally
        {
            Environment.SetEnvironmentVariable("MYNET_IO_TEST_ROOT", null);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [Fact]
    public void CreateSubDirectory_CreatesDirectoryUnderRoot()
    {
        using var fixture = new DirectoryFixture();

        var result = fixture.Service.CreateSubDirectory("exports");

        Assert.Equal(Path.Combine(fixture.Root, "exports"), result, StringComparer.OrdinalIgnoreCase);
        Assert.True(Directory.Exists(result));
    }

    [Fact]
    public void GetFileName_NormalizesExtensionAndPreferredFileName()
    {
        using var fixture = new DirectoryFixture();

        var result = fixture.Service.GetFileName("csv", "report.txt");

        Assert.Equal(Path.Combine(fixture.Root, "report.csv"), result, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetFileName_WhenExtensionMissing_UsesTmpExtension()
    {
        using var fixture = new DirectoryFixture();

        var result = fixture.Service.GetFileName();

        Assert.Equal(".tmp", Path.GetExtension(result), StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateFile_CreatesPreferredFileNameWhenAvailable()
    {
        using var fixture = new DirectoryFixture();

        var result = fixture.Service.CreateFile("txt", "report.txt");

        Assert.Equal(Path.Combine(fixture.Root, "report.txt"), result, StringComparer.OrdinalIgnoreCase);
        Assert.True(File.Exists(result));
    }

    [Fact]
    public void CreateFile_WhenPreferredNameExists_FallsBackToUniqueFile()
    {
        using var fixture = new DirectoryFixture();
        var preferred = Path.Combine(fixture.Root, "report.txt");
        File.WriteAllText(preferred, "existing");

        var result = fixture.Service.CreateFile("txt", "report.txt");

        Assert.NotEqual(preferred, result, StringComparer.OrdinalIgnoreCase);
        Assert.True(File.Exists(result));
    }

    [Fact]
    public void Clean_RemovesFilesAndSubDirectoriesButKeepsRoot()
    {
        using var fixture = new DirectoryFixture();
        File.WriteAllText(Path.Combine(fixture.Root, "a.txt"), "content");
        Directory.CreateDirectory(Path.Combine(fixture.Root, "sub"));
        File.WriteAllText(Path.Combine(fixture.Root, "sub", "b.txt"), "content");

        fixture.Service.Clean();

        Assert.True(Directory.Exists(fixture.Root));
        Assert.Empty(Directory.GetFiles(fixture.Root));
        Assert.Empty(Directory.GetDirectories(fixture.Root));
    }

    [Fact]
    public void Delete_RemovesRootDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var service = new DirectoryService(root, NullLogger<DirectoryService>.Instance);
        File.WriteAllText(Path.Combine(root, "a.txt"), "content");

        service.Delete();

        Assert.False(Directory.Exists(root));
    }

    private sealed class DirectoryFixture : IDisposable
    {
        public DirectoryFixture()
        {
            Root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Service = new(Root, NullLogger<DirectoryService>.Instance);
        }

        public string Root { get; }

        public DirectoryService Service { get; }

        public void Dispose()
        {
            if (Directory.Exists(Root)) Directory.Delete(Root, true);
        }
    }
}
