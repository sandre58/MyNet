// -----------------------------------------------------------------------
// <copyright file="ObservableMetadataBootstrap.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Tests.Metadata;

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
