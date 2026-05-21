// -----------------------------------------------------------------------
// <copyright file="RecentFileModelsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Utilities.IO.FileHistory;
using Xunit;

namespace MyNet.Utilities.Tests.IO;

public sealed class RecentFileModelsTests
{
    [Fact]
    public void RecentFile_RecordEquality_UsesValues()
    {
        var timestamp = DateTimeOffset.UtcNow;
        var left = new RecentFile { Name = "Doc", Path = "doc.txt", LastAccessedAt = timestamp, IsPinned = true };
        var right = new RecentFile { Name = "Doc", Path = "doc.txt", LastAccessedAt = timestamp, IsPinned = true };

        Assert.Equal(left, right);
    }

    [Fact]
    public void RecentFilesOptions_Defaults_AreInitialized()
    {
        var options = new RecentFilesOptions();

        Assert.Equal(string.Empty, options.BasePath, StringComparer.Ordinal);
        Assert.Equal(0, options.MaxEntries);
        Assert.Empty(options.SupportedExtensions);
    }

    [Fact]
    public void RecentFilesOptions_SupportedExtensions_CanUseCaseInsensitiveSet()
    {
        var options = new RecentFilesOptions
        {
            SupportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".txt" }
        };

        Assert.Contains(".TXT", options.SupportedExtensions);
    }
}
