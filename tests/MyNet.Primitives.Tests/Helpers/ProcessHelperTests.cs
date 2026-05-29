// -----------------------------------------------------------------------
// <copyright file="ProcessHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Helpers;
using Xunit;

namespace MyNet.Primitives.Tests.Helpers;

public sealed class ProcessHelperTests
{
    [Fact]
    public void Start_EmptyUri_Throws()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.Start(string.Empty));

    [Fact]
    public void TryStart_EmptyUri_ReturnsFalse()
        => Assert.False(ProcessHelper.TryStart("   "));

    [Fact]
    public void Open_EmptyExecutable_Throws()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.Open(string.Empty, "arg"));

    [Fact]
    public void OpenFile_EmptyPath_Throws()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.OpenFile(" "));

    [Fact]
    public void OpenFolder_EmptyPath_Throws()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.OpenFolder(string.Empty));

    [Fact]
    public void OpenInExcel_EmptyPath_Throws()
        => Assert.Throws<ArgumentException>(() => ProcessHelper.OpenInExcel(" "));
}
