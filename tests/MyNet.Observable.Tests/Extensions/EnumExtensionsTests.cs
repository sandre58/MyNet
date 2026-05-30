// -----------------------------------------------------------------------
// <copyright file="EnumExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using MyNet.Primitives;
using Xunit;

namespace MyNet.Observable.Tests.Extensions;

[Collection("UseCultureSequential")]
public sealed class EnumExtensionsTests
{
    [Fact]
    public void GetLocalizedEnums_FromType_ReturnsAllEnumMembers()
    {
        var result = typeof(DayOfWeek).GetLocalizedEnums();

        result.Should().HaveCount(7);
        result.Select(x => x.Value).Should().BeEquivalentTo(Enum.GetValues<DayOfWeek>());
    }

    [Fact]
    public void GetLocalizedEnums_FromType_ExcludesSpecifiedValues()
    {
        var result = typeof(DayOfWeek).GetLocalizedEnums([DayOfWeek.Sunday, DayOfWeek.Saturday]);

        result.Should().HaveCount(5);
        result.Select(x => x.Value).Should().NotContain([DayOfWeek.Sunday, DayOfWeek.Saturday]);
    }

    [Fact]
    public void GetLocalizedEnums_FromType_WhenNotEnum_ThrowsArgumentException()
    {
        var act = () => typeof(string).GetLocalizedEnums();

        act.Should().Throw<ArgumentException>()
            .WithMessage("*not a system enum*");
    }

    [Fact]
    public void GetLocalizedEnums_FromType_WhenTypeIsNull_ThrowsArgumentNullException()
    {
        Type? type = null;

        var act = () => type!.GetLocalizedEnums();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetLocalizedEnums_Generic_ReturnsAllEnumMembers()
    {
        var result = EnumExtensions.GetLocalizedEnums<DayOfWeek>();

        result.Should().HaveCount(7);
        result.Select(x => x.Value).Should().BeEquivalentTo(Enum.GetValues<DayOfWeek>());
    }

    [Fact]
    public void GetLocalizedEnums_Generic_ExcludesSpecifiedValues()
    {
        var result = EnumExtensions.GetLocalizedEnums([DayOfWeek.Monday, DayOfWeek.Tuesday]);

        result.Should().HaveCount(5);
        result.Select(x => x.Value).Should().NotContain([DayOfWeek.Monday, DayOfWeek.Tuesday]);
    }

    [Fact]
    public void GetLocalizedSmartEnums_FromType_ReturnsAllSmartEnumMembers()
    {
        var result = typeof(TestStatus).GetLocalizedSmartEnums();

        result.Should().HaveCount(2);
        result.Select(x => x.Value).Should().BeEquivalentTo([TestStatus.Active, TestStatus.PendingApproval]);
    }

    [Fact]
    public void GetLocalizedSmartEnums_FromType_ExcludesSpecifiedValues()
    {
        var result = typeof(TestStatus).GetLocalizedSmartEnums([TestStatus.Active]);

        result.Should().HaveCount(1);
        result.Single().Value.Should().BeSameAs(TestStatus.PendingApproval);
    }

    [Fact]
    public void GetLocalizedSmartEnums_FromType_WhenNotSmartEnum_ThrowsArgumentException()
    {
        var act = () => typeof(DayOfWeek).GetLocalizedSmartEnums();

        act.Should().Throw<ArgumentException>()
            .WithMessage("*does not implement ISmartEnum*");
    }

    [Fact]
    public void GetLocalizedSmartEnums_Generic_ReturnsAllSmartEnumMembers()
    {
        var result = EnumExtensions.GetLocalizedSmartEnums<TestStatus>();

        result.Should().HaveCount(2);
        result.Select(x => x.Value).Should().BeEquivalentTo([TestStatus.Active, TestStatus.PendingApproval]);
    }

    [Fact]
    public void GetLocalizedSmartEnums_Generic_ExcludesSpecifiedValues()
    {
        var result = EnumExtensions.GetLocalizedSmartEnums([TestStatus.PendingApproval]);

        result.Should().HaveCount(1);
        result.Single().Value.Should().BeSameAs(TestStatus.Active);
    }

    [Fact]
    public void GetLocalizedEnums_FromType_WithInvalidExcludedValues_ThrowsArgumentException()
    {
        var act = () => typeof(DayOfWeek).GetLocalizedEnums(["Monday"]);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Wrong type*");
    }

    [Fact]
    public void GetLocalizedSmartEnums_FromType_WithInvalidExcludedValues_ThrowsArgumentException()
    {
        var act = () => typeof(TestStatus).GetLocalizedSmartEnums([DayOfWeek.Monday]);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Wrong type*");
    }

    [Fact]
    public void GetLocalizedEnums_FromType_DisplayReturnsHumanizedText()
    {
        var result = typeof(GenderType).GetLocalizedEnums();

        result.Should().ContainSingle(x => x.Value!.Equals(GenderType.Male) && x.Display == "Male");
        result.Should().ContainSingle(x => x.Value!.Equals(GenderType.Female) && x.Display == "Female");
    }

    private sealed class TestStatus(int value, string name) : SmartEnum<TestStatus, int>(value)
    {
        public static readonly TestStatus PendingApproval = new(1, "Pending Approval");

        public static readonly TestStatus Active = new(2, "Active");

        public override string Name { get; } = name;
    }
}
