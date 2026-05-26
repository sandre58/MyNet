// -----------------------------------------------------------------------
// <copyright file="ObservableMetadataBootstrap.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Metadata.Tests;

#pragma warning disable IDE0130
namespace MyNet.Metadata.Generated;
#pragma warning restore IDE0130

/// <summary>
/// Test double for the source generator bootstrap type (same namespace and name as generated code).
/// </summary>
internal static class ObservableMetadataBootstrap
{
    internal static bool WasInvoked { get; private set; }

    internal static void Ensure(Type type)
    {
        if (type == typeof(GeneratedMetadataBootstrapInvokerTests))
            WasInvoked = true;
    }

    internal static void ResetForTests() => WasInvoked = false;
}
