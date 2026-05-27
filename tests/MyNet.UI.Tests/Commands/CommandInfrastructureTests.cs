// -----------------------------------------------------------------------
// <copyright file="CommandInfrastructureTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Commands;
using Xunit;

namespace MyNet.UI.Tests.Commands;

public sealed class CommandInfrastructureTests
{
    [Fact]
    public void RelayCommand_Execute_ShouldHonorCanExecute()
    {
        var invoked = false;
        var command = new RelayCommand(() => invoked = true, () => false);

        command.Execute(null);

        Assert.False(invoked);
    }

    [Fact]
    public void RelayCommandOfT_CanExecute_ShouldBeFalse_ForInvalidParameterType()
    {
        var command = new RelayCommand<int>(_ => { });

        Assert.False(command.CanExecute("invalid"));
    }

    [Fact]
    public async Task AsyncRelayCommand_ShouldPreventConcurrentExecution()
    {
        var started = 0;
        var completion = new TaskCompletionSource<object?>();
        var command = new AsyncRelayCommand(async () =>
        {
            Interlocked.Increment(ref started);
            await completion.Task.ConfigureAwait(false);
        });

        var firstExecution = command.ExecuteAsync(null);
        await Task.Yield();

        Assert.Equal(1, Volatile.Read(ref started));
        Assert.False(command.CanExecute(null));

        await command.ExecuteAsync(null);
        Assert.Equal(1, Volatile.Read(ref started));

        completion.SetResult(null);
        await firstExecution.ConfigureAwait(true);

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void CommandFactoryExtensions_CreateRequired_ShouldIgnoreNullParameter()
    {
        var factory = new RelayCommandFactory();
        var invoked = false;
        var command = factory.CreateRequired<string>(_ => invoked = true);

        command.Execute(null);
        Assert.False(invoked);

        command.Execute("ok");
        Assert.True(invoked);
    }
}
