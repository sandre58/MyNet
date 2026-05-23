// -----------------------------------------------------------------------
// <copyright file="ObservableMetadataBootstrap.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Metadata.Generated;

internal static class ObservableMetadataBootstrap
{
    internal static bool WasInvoked { get; private set; }

    internal static void Ensure(Type type)
    {
        if (type == typeof(MyNet.Utilities.Tests.Metadata.GeneratedMetadataBootstrapInvokerTests))
            WasInvoked = true;
    }

    internal static void ResetForTests() => WasInvoked = false;
}
