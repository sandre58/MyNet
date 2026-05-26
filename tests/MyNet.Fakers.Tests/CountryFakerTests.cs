// -----------------------------------------------------------------------
// <copyright file="CountryFakerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using MyNet.Fakers.Geography;
using MyNet.Generator;
using MyNet.Geography;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class CountryFakerTests
{
    [Fact]
    public void CountryAccessors_ShouldReturnValuesFromSelectedCountry()
    {
        var random = new Mock<IRandomGenerator>();
        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<Country>>())).Returns((IReadOnlyCollection<Country> values) => values.First());

        var sut = new CountryFaker(random.Object);

        var country = sut.Country();
        sut.Name().Should().NotBeNullOrWhiteSpace();
        sut.Alpha2().Should().HaveLength(2);
        sut.Alpha3().Should().HaveLength(3);
        sut.Iso().Should().BeGreaterThan(0);
        country.Should().NotBeNull();
    }
}
