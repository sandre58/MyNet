// -----------------------------------------------------------------------
// <copyright file="ProgresserExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Progress;
using Xunit;

namespace MyNet.Utilities.Tests.Progress;

public class ProgresserExtensionsTests
{
    [Fact]
    public void Begin_StringOverloads_ReturnStepWithExpectedMessage()
    {
        var progresser = new Progresser();

        using var s1 = progresser.Begin("hello {0}", "world");
        using var s2 = progresser.Begin(2, "hello {0}", "world");
        using var s3 = progresser.Begin([0.4, 0.6], "hello {0}", "world");
        using var s4 = progresser.Begin(() => { }, "hello {0}", "world");
        using var s5 = progresser.Begin(2, () => { }, "hello {0}", "world");
        using var s6 = progresser.Begin([0.2, 0.8], () => { }, "hello {0}", "world");

        Assert.Equal("hello world", s1.Message!.ToString());
        Assert.Equal("hello world", s2.Message!.ToString());
        Assert.Equal("hello world", s3.Message!.ToString());
        Assert.Equal("hello world", s4.Message!.ToString());
        Assert.Equal("hello world", s5.Message!.ToString());
        Assert.Equal("hello world", s6.Message!.ToString());
    }

    [Fact]
    public void StartStep_StringOverloads_ReturnStep()
    {
        var progresser = new Progresser();
        using var root = progresser.Begin("root");

        using var s1 = progresser.StartStep("step {0}", 1);
        using var s2 = progresser.StartStep(2, "step {0}", 2);
        using var s3 = progresser.StartStep([0.4, 0.6], "step {0}", 3);
        using var s4 = progresser.StartStepUncancellable("step {0}", 4);
        using var s5 = progresser.StartStepUncancellable(2, "step {0}", 5);
        using var s6 = progresser.StartStepUncancellable([0.5, 0.5], "step {0}", 6);

        Assert.Equal("step 1", s1.Message!.ToString());
        Assert.Equal("step 2", s2.Message!.ToString());
        Assert.Equal("step 3", s3.Message!.ToString());
        Assert.Equal("step 4", s4.Message!.ToString());
        Assert.Equal("step 5", s5.Message!.ToString());
        Assert.Equal("step 6", s6.Message!.ToString());
        Assert.False(s4.CanCancel);
        Assert.False(s5.CanCancel);
        Assert.False(s6.CanCancel);
    }
}
