// -----------------------------------------------------------------------
// <copyright file="ListConjunctionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Formatting.Collections;
using Xunit;

namespace MyNet.Humanizer.Tests.Formatting;

public class ListConjunctionTests
{
    [Fact]
    public void ListConjunction_HasAndValue() => Assert.Equal(ListConjunction.And, ListConjunction.And);

    [Fact]
    public void ListConjunction_HasOrValue() => Assert.Equal(ListConjunction.Or, ListConjunction.Or);

    [Fact]
    public void ListConjunction_AndAndOr_AreNotEqual() => Assert.NotEqual(ListConjunction.And, ListConjunction.Or);
}
