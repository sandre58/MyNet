// -----------------------------------------------------------------------
// <copyright file="FileHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using Xunit;

namespace MyNet.IO.Tests;

public sealed class FileHelperTests
{
    [Fact]
    public void EnsureDirectoryExists_CreatesMissingDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        try
        {
            FileHelper.EnsureDirectoryExists(path);

            Assert.True(Directory.Exists(path));
        }
        finally
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
        }
    }

    [Fact]
    public void EnsureDirectoryExists_WhenFileExistsAtPath_ThrowsIOException()
    {
        var path = Path.GetTempFileName();

        try
        {
            Assert.Throws<IOException>(() => FileHelper.EnsureDirectoryExists(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void EnsureFileExists_WhenFilenameIsMissing_ThrowsInvalidOperationException()
        => Assert.Throws<InvalidOperationException>(() => FileHelper.EnsureFileExists(string.Empty));

    [Fact]
    public void EnsureFileExists_WhenFileDoesNotExist_ThrowsFileNotFoundException()
        => Assert.Throws<FileNotFoundException>(() => FileHelper.EnsureFileExists(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))));

    [Fact]
    public void EnsureFileExists_WhenFileExists_ReturnsFilename()
    {
        var path = Path.GetTempFileName();

        try
        {
            Assert.Equal(path, FileHelper.EnsureFileExists(path), StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void TryDeleteFile_WhenFileExists_DeletesFileAndReturnsTrue()
    {
        var path = Path.GetTempFileName();

        var result = FileHelper.TryDeleteFile(path);

        Assert.True(result);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void TryDeleteFile_WhenFileDoesNotExist_ReturnsFalse()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        Assert.False(FileHelper.TryDeleteFile(path));
    }

    [Fact]
    public void TryCreateFile_WhenFileDoesNotExist_CreatesFileAndReturnsTrue()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");

        try
        {
            var result = FileHelper.TryCreateFile(path);

            Assert.True(result);
            Assert.True(File.Exists(path));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void TryCreateFile_WhenFileAlreadyExists_ReturnsFalse()
    {
        var path = Path.GetTempFileName();

        try
        {
            Assert.False(FileHelper.TryCreateFile(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
