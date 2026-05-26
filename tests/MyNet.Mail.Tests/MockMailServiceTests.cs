// -----------------------------------------------------------------------
// <copyright file="MockMailServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.Mail.Mock;
using Xunit;

namespace MyNet.Mail.Tests;

public class MockMailServiceTests
{
    [Fact]
    public void CanConnect_ReturnsTrue()
    {
        var service = new MockMailService();

        Assert.True(service.CanConnect());
    }

    [Fact]
    public async Task CanConnectAsync_ReturnsTrue()
    {
        var service = new MockMailService();

        Assert.True(await service.CanConnectAsync());
    }

    [Fact]
    public async Task SendAsync_WhenCanceled_AddsErrorMessage()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var service = new MockMailService();
        var response = await service.SendAsync(Email.From("from@test.com").To("to@test.com"), cts.Token);

        Assert.Contains("Send canceled.", response.ErrorMessages);
    }

    [Fact]
    public void Send_WhenCanceled_AddsErrorMessage()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var service = new MockMailService();
        var response = service.Send(Email.From("from@test.com").To("to@test.com"), cts.Token);

        Assert.Contains("Send canceled.", response.ErrorMessages);
    }
}
