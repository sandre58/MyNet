// -----------------------------------------------------------------------
// <copyright file="BooleanExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Utilities.Tests.Extensions;

public class BooleanExtensionsTests
{
    #region bool.IfTrue / IfFalse

    [Fact]
    public void IfTrue_WhenTrue_ExecutesAction()
    {
        var executed = false;
        true.IfTrue(() => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void IfTrue_WhenFalse_DoesNotExecuteAction()
    {
        var executed = false;
        false.IfTrue(() => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void IfTrue_NullAction_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => true.IfTrue(null!));

    [Fact]
    public void IfFalse_WhenFalse_ExecutesAction()
    {
        var executed = false;
        false.IfFalse(() => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void IfFalse_WhenTrue_DoesNotExecuteAction()
    {
        var executed = false;
        true.IfFalse(() => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void IfFalse_NullAction_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => false.IfFalse(null!));

    #endregion

    #region bool?.IsTrue / IsFalse / IfTrue / IfFalse

    [Fact]
    public void Nullable_IsTrue_ReturnsTrueOnlyWhenValueIsTrue()
    {
        bool? t = true;
        bool? f = false;
        bool? n = null;

        Assert.True(t.IsTrue());
        Assert.False(f.IsTrue());
        Assert.False(n.IsTrue());
    }

    [Fact]
    public void Nullable_IsFalse_ReturnsTrueForFalseOrNull()
    {
        bool? t = true;
        bool? f = false;
        bool? n = null;

        Assert.False(t.IsFalse());
        Assert.True(f.IsFalse());
        Assert.True(n.IsFalse());
    }

    [Fact]
    public void Nullable_IfTrue_WhenTrue_ExecutesAction()
    {
        bool? value = true;
        var executed = false;
        value.IfTrue(() => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void Nullable_IfTrue_WhenFalse_DoesNotExecuteAction()
    {
        bool? value = false;
        var executed = false;
        value.IfTrue(() => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void Nullable_IfTrue_WhenNull_DoesNotExecuteAction()
    {
        bool? value = null;
        var executed = false;
        value.IfTrue(() => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void Nullable_IfFalse_WhenFalse_ExecutesAction()
    {
        bool? value = false;
        var executed = false;
        value.IfFalse(() => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void Nullable_IfFalse_WhenNull_ExecutesAction()
    {
        bool? value = null;
        var executed = false;
        value.IfFalse(() => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void Nullable_IfFalse_WhenTrue_DoesNotExecuteAction()
    {
        bool? value = true;
        var executed = false;
        value.IfFalse(() => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void Nullable_IfTrue_NullAction_ThrowsArgumentNullException()
    {
        bool? value = true;
        Assert.Throws<ArgumentNullException>(() => value.IfTrue(null!));
    }

    [Fact]
    public void Nullable_IfFalse_NullAction_ThrowsArgumentNullException()
    {
        bool? value = false;
        Assert.Throws<ArgumentNullException>(() => value.IfFalse(null!));
    }

    #endregion
}
