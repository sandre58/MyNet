// -----------------------------------------------------------------------
// <copyright file="DriveExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using Xunit;

namespace MyNet.IO.Tests;

public sealed class DriveExtensionsTests
{
    [Fact]
    public void HasEnoughSpace_WhenDriveNotReady_ReturnsDiskNotFound()
    {
        var drive = new DriveInfo("Z:\\");

        if (drive.IsReady)
        {
            return;
        }

        Assert.Equal(DiskDriveInfo.DiskNotFound, drive.HasEnoughSpace(1));
    }

    [Fact]
    public void HasEnoughSpace_WhenReadyAndEnoughFreeSpace_ReturnsNoError()
    {
        var root = Path.GetPathRoot(Directory.GetCurrentDirectory());
        Assert.False(string.IsNullOrEmpty(root));

        var drive = new DriveInfo(root);
        if (!drive.IsReady)
        {
            return;
        }

        Assert.Equal(DiskDriveInfo.NoError, drive.HasEnoughSpace(1));
    }

    [Fact]
    public void HasEnoughSpace_WhenReadyButInsufficientSpace_ReturnsInsufficientSpace()
    {
        var root = Path.GetPathRoot(Directory.GetCurrentDirectory());
        Assert.False(string.IsNullOrEmpty(root));

        var drive = new DriveInfo(root);
        if (!drive.IsReady)
        {
            return;
        }

        var required = drive.TotalFreeSpace + 1;

        Assert.Equal(DiskDriveInfo.InsufficientSpace, drive.HasEnoughSpace(required));
    }
}
