// -----------------------------------------------------------------------
// <copyright file="PatternFakerExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Moq;
using MyNet.Utilities.Generator;
using MyNet.Utilities.Text.Randomize;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class PatternFakerExtensionsTests
{
    [Fact]
    public void RandomizeWithRandomPattern_ShouldPickPattern_AndRandomize()
    {
        var patternGenerator = new Mock<ITextRandomGenerator>(MockBehavior.Strict);
        var random = new Mock<IRandomGenerator>(MockBehavior.Strict);
        var patterns = new[] { "##-##", "??" };

        random.Setup(x => x.Item(It.IsAny<IReadOnlyCollection<string>>())).Returns("##-##");
        patternGenerator.Setup(x => x.Randomize("##-##")).Returns("12-34");

        var result = patternGenerator.Object.RandomizeWithRandomPattern(random.Object, patterns);

        result.Should().Be("12-34");
    }
}
