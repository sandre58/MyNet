// -----------------------------------------------------------------------
// <copyright file="UnknownValueTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class UnknownValueTests
{
    [Fact]
    public void Instance_IsSingletonReference() => Assert.Same(UnknownValue.Instance, UnknownValue.Instance);
}
