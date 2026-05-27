// -----------------------------------------------------------------------
// <copyright file="MessengerTestCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Messaging.Tests;

[CollectionDefinition("Messenger")]
public sealed class MessengerTestCollection : ICollectionFixture<MessengerTestFixture>;

public sealed class MessengerTestFixture
{
    public MessengerTestFixture() => Messenger.Reset();
}
