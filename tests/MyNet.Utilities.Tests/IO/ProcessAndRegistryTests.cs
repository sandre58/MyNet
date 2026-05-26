// -----------------------------------------------------------------------
// <copyright file="ProcessAndRegistryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using MyNet.IO;
using MyNet.IO.Registry;
using MyNet.Platform.Windows.Registry.FileHistory;
using MyNet.Utilities.Process;
using Xunit;

namespace MyNet.Utilities.Tests.IO;

public sealed class ProcessAndRegistryTests
{
    [Fact]
    public void ProcessHelper_Start_WithWhitespace_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.Start(" "));

    [Fact]
    public void ProcessHelper_TryStart_WithWhitespace_ReturnsFalse()
        => Assert.False(ProcessHelper.TryStart(" "));

    [Fact]
    public void ProcessHelper_Open_WithWhitespaceExecutable_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.Open(" "));

    [Fact]
    public void ProcessHelper_OpenFile_WithWhitespace_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.OpenFile(" "));

    [Fact]
    public void ProcessHelper_OpenInExcel_WithWhitespace_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.OpenInExcel(" "));

    [Fact]
    public void ProcessHelper_OpenFolder_WithWhitespace_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.OpenFolder(" "));

    [Fact]
    public void RegistryPath_Combine_And_ToString_ReturnExpectedPath()
    {
        var path = RegistryPath.Combine("HKEY_CURRENT_USER\\Software", "MyNet");

        Assert.Equal("HKEY_CURRENT_USER\\Software\\MyNet", path.ToString(), StringComparer.Ordinal);
    }

    [Fact]
    public void RegistryEntry_StoresPathAndItem()
    {
        var path = RegistryPath.Combine("HKEY_CURRENT_USER", "MyNet");
        var entry = new RegistryEntry<string>(path, "value");

        Assert.Equal(path, entry.Path);
        Assert.Equal("value", entry.Item, StringComparer.Ordinal);
    }

    [Fact]
    public void RegistryRecentFileEntry_DefaultsToEmptyStrings()
    {
        var entry = new RegistryRecentFileEntry();

        Assert.Equal(string.Empty, entry.Name, StringComparer.Ordinal);
        Assert.Equal(string.Empty, entry.Path, StringComparer.Ordinal);
        Assert.False(entry.IsPinned);
        Assert.False(entry.IsRecovered);
        Assert.Equal(0L, entry.LastAccessUtc);
    }

    [Fact]
    public void DriveExtensions_HasEnoughSpace_ReturnsNoErrorForZeroRequiredSpaceOnReadyDrive()
    {
        var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)!);

        Assert.Equal(DiskDriveInfo.NoError, drive.HasEnoughSpace(0));
    }

    [Fact]
    public void DriveExtensions_HasEnoughSpace_ReturnsInsufficientSpaceForHugeRequiredSpaceOnReadyDrive()
    {
        var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)!);

        Assert.Equal(DiskDriveInfo.InsufficientSpace, drive.HasEnoughSpace(double.MaxValue));
    }

    [Fact]
    public void DriveExtensions_HasEnoughSpace_ReturnsDiskNotFoundForUnusedDriveLetter()
    {
        var unusedLetter = Enumerable.Range('D', 'Z' - 'D' + 1)
            .Select(x => ((char)x).ToString())
            .First(letter => !DriveInfo.GetDrives().Any(d => d.Name.StartsWith(letter, StringComparison.OrdinalIgnoreCase)));

        var drive = new DriveInfo($"{unusedLetter}:\\");

        Assert.Equal(DiskDriveInfo.DiskNotFound, drive.HasEnoughSpace(1));
    }
}
