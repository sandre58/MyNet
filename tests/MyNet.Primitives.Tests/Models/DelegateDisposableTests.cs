// -----------------------------------------------------------------------
// <copyright file="DelegateDisposableTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Primitives.Tests.Models;

public sealed class DelegateDisposableTests
{
    [Fact]
    public void Dispose_ExecutesActionOnce()
    {
        var count = 0;
        var disposable = new DelegateDisposable(() => count++);

        disposable.Dispose();
        disposable.Dispose();

        Assert.Equal(1, count);
    }
}
