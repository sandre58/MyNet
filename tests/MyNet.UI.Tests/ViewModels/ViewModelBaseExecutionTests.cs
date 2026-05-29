// -----------------------------------------------------------------------
// <copyright file="ViewModelBaseExecutionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI.ViewModels;
using Xunit;

namespace MyNet.UI.Tests.ViewModels;

public class ViewModelBaseExecutionTests
{
    [Fact]
    public async Task ExecuteAsync_Rethrows_WhenOnExecutionErrorReturnsFalseAsync()
    {
        var sut = new TestViewModel { HandleErrors = false };

        var act = sut.RunExecuteAsync;

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteSafeAsync_DoesNotRethrow_WhenOnExecutionErrorReturnsFalseAsync()
    {
        var sut = new TestViewModel { HandleErrors = false };

        await sut.RunExecuteSafeAsync();

        sut.ErrorHandlerInvoked.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_Swallows_WhenOnExecutionErrorReturnsTrueAsync()
    {
        var sut = new TestViewModel { HandleErrors = true };

        await sut.RunExecuteAsync();

        sut.ErrorHandlerInvoked.Should().BeTrue();
    }

    private sealed class TestViewModel : ViewModelBase
    {
        public bool HandleErrors { get; init; }

        public bool ErrorHandlerInvoked { get; private set; }

        public Task RunExecuteAsync()
            => ExecuteAsync(_ => throw new InvalidOperationException("boom"));

        public Task RunExecuteSafeAsync()
            => ExecuteSafeAsync(_ => throw new InvalidOperationException("boom"));

        protected override bool OnExecutionError(Exception exception)
        {
            ErrorHandlerInvoked = true;
            return HandleErrors;
        }
    }
}
