// -----------------------------------------------------------------------
// <copyright file="ProgresserTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Progress;
using Xunit;

namespace MyNet.Utilities.Tests.Progress;

public class ProgresserTests
{
    [Fact]
    public void StartStep_WithoutBegin_ThrowsInvalidOperationException()
    {
        var progresser = new Progresser();

        Assert.Throws<InvalidOperationException>(() => progresser.StartStep("phase"));
    }

    [Fact]
    public void Begin_WithSubscriber_ReportsInitialState()
    {
        var progresser = new Progresser();
        var observer = new CaptureProgressObserver();
        progresser.Subscribe(observer);

        using var progressStep = progresser.Begin("root");

        Assert.NotNull(observer.LastReport);
        Assert.Equal(0d, observer.LastReport!.Progress, 5);
        Assert.Single(observer.LastReport.Messages);
        Assert.Equal("root", observer.LastReport.Messages[0].Message);
    }

    [Fact]
    public void ChildStepCompletion_UpdatesParentProgress()
    {
        var progresser = new Progresser();
        var observer = new CaptureProgressObserver();
        progresser.Subscribe(observer);

        using var root = progresser.Begin(2, "root");
        using (var step1 = progresser.StartStep("step1"))
        {
            step1.UpdateProgress(1);
        }

        Assert.NotNull(observer.LastReport);
        Assert.Equal(0.5d, observer.LastReport!.Progress, 5);
    }

    [Fact]
    public void StartStepUncancellable_SetsReportCanCancelToFalse()
    {
        var progresser = new Progresser();
        var observer = new CaptureProgressObserver();
        progresser.Subscribe(observer);

        using var root = progresser.Begin(() => { }, "root");
        using var progressStep = progresser.StartStepUncancellable("child");

        Assert.NotNull(observer.LastReport);
        Assert.False(observer.LastReport!.CanCancel);
    }

    [Fact]
    public void Unsubscribe_StopsFurtherReports()
    {
        var progresser = new Progresser();
        var observer = new CaptureProgressObserver();
        progresser.Subscribe(observer);

        using var root = progresser.Begin("root");
        var countAfterBegin = observer.ReportCount;

        progresser.Unsubscribe(observer);
        root.UpdateMessage(new("changed"));

        Assert.Equal(countAfterBegin, observer.ReportCount);
    }

    private sealed class CaptureProgressObserver : IProgress<ProgressReport<ProgressMessage>>
    {
        public int ReportCount { get; private set; }

        public ProgressReport<ProgressMessage>? LastReport { get; private set; }

        public void Report(ProgressReport<ProgressMessage> value)
        {
            ReportCount++;
            LastReport = value;
        }
    }
}
