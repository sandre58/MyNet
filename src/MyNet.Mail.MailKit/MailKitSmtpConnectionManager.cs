// -----------------------------------------------------------------------
// <copyright file="MailKitSmtpConnectionManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using MyNet.Mail.Smtp;

namespace MyNet.Mail.MailKit;

/// <summary>
/// Thread-safe, reusable MailKit <see cref="SmtpClient"/> for a fixed <see cref="SmtpClientOptions"/> profile.
/// </summary>
internal sealed class MailKitSmtpConnectionManager(SmtpClientOptions options) : IDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private SmtpClient? _client;

    public async Task<string> SendAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var client = await GetConnectedClientAsync(cancellationToken).ConfigureAwait(false);
            return await client.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await ResetAsync().ConfigureAwait(false);
            throw;
        }
        finally
        {
            _ = _gate.Release();
        }
    }

    public void Dispose()
    {
        _gate.Dispose();
        _client?.Dispose();
        _client = null;
    }

    private async Task<SmtpClient> GetConnectedClientAsync(CancellationToken cancellationToken)
    {
        _client ??= new();

        if (_client.IsConnected)
        {
            return _client;
        }

        await _client.ConnectAsync(
            options.Server!,
            options.Port,
            SmtpSecureSocketOptionsResolver.Resolve(options),
            cancellationToken).ConfigureAwait(false);

        if (options.RequiresAuthentication)
        {
            await _client.AuthenticateAsync(options.User!, options.Password!, cancellationToken).ConfigureAwait(false);
        }

        return _client;
    }

    private async Task ResetAsync()
    {
        if (_client is null)
        {
            return;
        }

        try
        {
            if (_client.IsConnected)
            {
                await _client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
        catch (ServiceNotConnectedException)
        {
        }
        finally
        {
            _client.Dispose();
            _client = null;
        }
    }
}
