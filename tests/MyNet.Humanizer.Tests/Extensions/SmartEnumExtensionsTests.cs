// -----------------------------------------------------------------------
// <copyright file="SmartEnumExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Humanizer.Static;
using MyNet.Utilities;
using Xunit;

#pragma warning disable CA2263 // Intentional: these tests validate the Type-based overload behavior.

namespace MyNet.Humanizer.Tests.Extensions;

[Collection("UseCultureSequential")]
public class SmartEnumExtensionsTests
{
    [Fact]
    public void Humanize_WithHumanizeFallbackDisabled_ReturnsSmartEnumName()
    {
        var result = TestStatus.PendingApproval.Humanize(options: new());

        Assert.Equal("Pending Approval", result);
    }

    [Fact]
    public void Humanize_DefaultOptions_ReturnsNonEmptyValue()
    {
        var result = TestStatus.PendingApproval.Humanize();

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void DehumanizeToGeneric_WithExactName_ReturnsMatchingSmartEnum()
    {
        var result = "Pending Approval".DehumanizeTo<TestStatus>();

        Assert.Same(TestStatus.PendingApproval, result);
    }

    [Fact]
    public void DehumanizeToGeneric_IsCaseInsensitive()
    {
        var result = "pending approval".DehumanizeTo<TestStatus>();

        Assert.Same(TestStatus.PendingApproval, result);
    }

    [Fact]
    public void DehumanizeToType_WithExactName_ReturnsMatchingSmartEnum()
    {
        var result = MyNet.Humanizer.Static.SmartEnumExtensions.DehumanizeTo("Active", typeof(TestStatus));

        Assert.Same(TestStatus.Active, result);
    }

    [Fact]
    public void DehumanizeToType_WithNullTargetType_ThrowsArgumentNullException() => _ = Assert.Throws<ArgumentNullException>(() => MyNet.Humanizer.Static.SmartEnumExtensions.DehumanizeTo("Active", null!));

    [Fact]
    public void DehumanizeToType_WithNonSmartEnumType_ThrowsArgumentException() => _ = Assert.Throws<ArgumentException>(() => MyNet.Humanizer.Static.SmartEnumExtensions.DehumanizeTo("Active", typeof(string)));

    [Fact]
    public void DehumanizeToGeneric_WithNullOrWhiteSpaceInput_ThrowsKeyNotFoundException() => _ = Assert.Throws<KeyNotFoundException>(() => " ".DehumanizeTo<TestStatus>());

    [Fact]
    public void TryDehumanizeTo_WithUnknownValue_ReturnsFalseAndNullResult()
    {
        var success = "Unknown".TryDehumanizeTo<TestStatus>(out var result);

        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryDehumanizeTo_WithKnownValue_ReturnsTrueAndSmartEnum()
    {
        var success = "Active".TryDehumanizeTo<TestStatus>(out var result);

        Assert.True(success);
        Assert.Same(TestStatus.Active, result);
    }

    private sealed class TestStatus : SmartEnum<TestStatus, int>
    {
        public static readonly TestStatus PendingApproval = new(1, "Pending Approval");
        public static readonly TestStatus Active = new(2, "Active");

        private TestStatus(int value, string name)
            : base(value)
            => Name = name;

        public override string Name { get; }
    }
}
