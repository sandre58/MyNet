// -----------------------------------------------------------------------
// <copyright file="GeneratedMetadataBootstrapInvokerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Metadata.Generated;
using Xunit;

namespace MyNet.Metadata.Tests;

public sealed class GeneratedMetadataBootstrapInvokerTests
{
    [Fact]
    public void Ensure_InvokesBootstrapInSameAssembly()
    {
        ResetBootstrapState();

        GeneratedMetadataBootstrapInvoker.Ensure(typeof(GeneratedMetadataBootstrapInvokerTests));

        Assert.True(ObservableMetadataBootstrap.WasInvoked);
    }

    [Fact]
    public void Ensure_IsNoOp_WhenBootstrapTypeIsMissing()
    {
        ResetBootstrapState();

        GeneratedMetadataBootstrapInvoker.Ensure(typeof(string));

        Assert.False(ObservableMetadataBootstrap.WasInvoked);
    }

    private static void ResetBootstrapState() => ObservableMetadataBootstrap.ResetForTests();
}
