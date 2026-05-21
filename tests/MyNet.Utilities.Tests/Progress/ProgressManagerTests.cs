// -----------------------------------------------------------------------
// <copyright file="ProgressManagerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Progress;
using Xunit;

namespace MyNet.Utilities.Tests.Progress;

public class ProgressManagerTests
{
    [Fact]
    public void StartStep_WithoutActiveSession_ReturnsNull()
    {
        ProgressManager.Initialize(new Progresser());

        var step = ProgressManager.StartStep("child");

        Assert.Null(step);
    }

    [Fact]
    public void BeginAndStartStep_WithInitializedManager_ReturnNonNull()
    {
        ProgressManager.Initialize(new Progresser());

        using var root = ProgressManager.Begin("root");
        using var child = ProgressManager.StartStep("child");

        Assert.NotNull(root);
        Assert.NotNull(child);
    }

    [Fact]
    public void Begin_WithOverloads_ReturnsStep()
    {
        ProgressManager.Initialize(new Progresser());

        using var s1 = ProgressManager.Begin(2, "a");
        using var s2 = ProgressManager.Begin([0.2, 0.8], "b");
        using var s3 = ProgressManager.Begin(() => { }, "c");
        using var s4 = ProgressManager.Begin(2, () => { }, "d");
        using var s5 = ProgressManager.Begin([1d, 3d], () => { }, "e");

        Assert.NotNull(s1);
        Assert.NotNull(s2);
        Assert.NotNull(s3);
        Assert.NotNull(s4);
        Assert.NotNull(s5);
    }

    [Fact]
    public void StartStepUncancellable_WithOverloads_ReturnsStep()
    {
        ProgressManager.Initialize(new Progresser());

        using var root = ProgressManager.Begin("root");
        using var s1 = ProgressManager.StartStepUncancellable("a");
        using var s2 = ProgressManager.StartStepUncancellable(2, "b");
        using var s3 = ProgressManager.StartStepUncancellable([0.25, 0.75], "c");

        Assert.NotNull(s1);
        Assert.NotNull(s2);
        Assert.NotNull(s3);
    }
}
