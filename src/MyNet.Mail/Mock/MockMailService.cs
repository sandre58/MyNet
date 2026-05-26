// -----------------------------------------------------------------------
// <copyright file="MockMailService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNet.Mail.Models;
using MyNet.Primitives;

namespace MyNet.Mail.Mock;

public partial class MockMailService(ILogger? logger = null) : IMailService
{
    [LoggerMessage(LogLevel.Information, "Simulate Sending Mail : {email}")]
    public static partial void LogSendingMail(ILogger logger, string email);

    public bool CanConnect() => true;

    public Task<bool> CanConnectAsync() => Task.FromResult(true);

    public SendResponse Send(IEmail email, CancellationToken? token = null)
    {
        var response = new SendResponse();

        try
        {
            Task.Delay(1000, token.GetValueOrDefault()).GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
            response.ErrorMessages.Add("Send canceled.");
            return response;
        }

        if (logger is not null)
            LogSendingMail(logger, email.ToString().OrEmpty());

        return response;
    }

    public async Task<SendResponse> SendAsync(IEmail email, CancellationToken? token = null)
    {
        var response = new SendResponse();

        try
        {
            await Task.Delay(1000, token.GetValueOrDefault()).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            response.ErrorMessages.Add("Send canceled.");
            return response;
        }

        if (logger is not null)
            LogSendingMail(logger, email.ToString().OrEmpty());

        return response;
    }
}
