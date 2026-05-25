// -----------------------------------------------------------------------
// <copyright file="BoundedValueTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.Observable.Behaviors;
using MyNet.Utilities.Intervals;
using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class BoundedValueTests
{
    [Fact]
    public void Value_OutsideRange_FailsValidation()
    {
        var sut = new BoundedValue<int>(0, 10) { Value = 15 };

        sut.Behaviors.GetOrDefault<ValidationBehavior<BoundedValue<int>>>()?.Validate();

        sut.Behaviors.GetOrDefault<ValidationBehavior<BoundedValue<int>>>()?.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void Reset_RestoresDefaultValue()
    {
        var sut = new BoundedValue<int>(0, 10, defaultValue: 5) { Value = 8 };

        sut.Reset();

        sut.Value.Should().Be(5);
    }

    [Fact]
    public void Interval_Contains_RespectsInclusiveBounds()
    {
        var range = new Interval<int>(1, 10);

        range.Contains(1).Should().BeTrue();
        range.Contains(10).Should().BeTrue();
        range.Contains(0).Should().BeFalse();
        range.Contains(11).Should().BeFalse();
    }
}
