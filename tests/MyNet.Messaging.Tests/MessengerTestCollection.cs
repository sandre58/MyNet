// -----------------------------------------------------------------------
// <copyright file="MessengerTestCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MyNet.Messaging.Tests;

[CollectionDefinition("Messenger")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "CollectionDefinition is a xUnit convention and must be public and non-generic.")]
[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "CollectionDefinition is a xUnit convention and must be public.")]
public sealed class MessengerTestCollection : ICollectionFixture<MessengerTestFixture>;

internal sealed class MessengerTestFixture
{
    public MessengerTestFixture() => Messenger.Reset();
}
