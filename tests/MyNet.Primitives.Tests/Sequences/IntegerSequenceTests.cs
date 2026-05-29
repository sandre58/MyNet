// -----------------------------------------------------------------------
// <copyright file="IntegerSequenceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Sequences;
using Xunit;

namespace MyNet.Primitives.Tests.Sequences;

public sealed class IntegerSequenceTests
{
    [Fact]
    public void NextValue_IncrementsCurrentValue()
    {
        var sequence = new IntegerSequence(10);

        Assert.Equal(10u, sequence.CurrentValue);
        Assert.Equal(11u, sequence.NextValue);
        Assert.Equal(11u, sequence.CurrentValue);
    }

    [Fact]
    public void SetCurrentValue_UpdatesSequence()
    {
        var sequence = new IntegerSequence();
        sequence.SetCurrentValue(100);

        Assert.Equal(100u, sequence.CurrentValue);
        Assert.Equal(101u, sequence.NextValue);
    }
}
