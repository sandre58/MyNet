// -----------------------------------------------------------------------
// <copyright file="MailKitService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MyNet.Utilities;
using MyNet.Utilities.Logging;
using MyNet.Mail;
using MyNet.Mail.Models;
using MyNet.Mail.Smtp;

namespace MyNet.Mail.MailKit;

/// <summary>
/// Creates a sender that uses the given SmtpClientOptions when sending with MailKit. Since the client is internal this will dispose of the client.
/// </summary>
/// <param name="smtpClientOptions">The SmtpClientOptions to use to create the MailKit client.</param>
/// <param name="logger">The logger to use for logging.</param>
public sealed partial class MailKitService(SmtpClientOptions smtpClientOptions, ILogger? logger = null) : IMailService
{
    private readonly ILogger _logger = logger ?? Log.Create<MailKitService>();

    /// <summary>
    /// Create a MimMessage so MailKit can send it.
    /// </summary>
    /// <returns>The mail message.</returns>
    /// <param name="email">Email data.</param>
    public static MimeMessage CreateMailMessage(IEmail email)
    {
        var data = email.Data;

        var message = new MimeMessage
        {
            Subject = data.Subject
        };

        message.From.Add(new MailboxAddress(data.From.Name, data.From.Address));
        data.To.ForEach(x => message.To.Add(new MailboxAddress(x.Name, x.Address)));
        data.Cc.ForEach(x => message.Cc.Add(new MailboxAddress(x.Name, x.Address)));
        data.Bcc.ForEach(x => message.Bcc.Add(new MailboxAddress(x.Name, x.Address)));
        data.ReplyTo.ForEach(x => message.ReplyTo.Add(new MailboxAddress(x.Name, x.Address)));

        var builder = new BodyBuilder();
        if (!string.IsNullOrEmpty(data.PlaintextAlternativeBody))
        {
            builder.TextBody = data.PlaintextAlternativeBody;
            builder.HtmlBody = data.Body;
        }
        else if (!data.IsHtml)
        {
            builder.TextBody = data.Body;
        }
        else
        {
            builder.HtmlBody = data.Body;
        }

        data.Attachments.ForEach(x =>
        {
            if (x.Data is null)
            {
                return;
            }

            var attachment = builder.Attachments.Add(x.Filename, x.Data, ContentType.Parse(x.ContentType));
            if (!string.IsNullOrWhiteSpace(x.ContentId))
            {
                attachment.ContentId = x.ContentId;
            }
        });

        message.Body = builder.ToMessageBody();

        foreach (var header in data.Headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        message.Priority = data.Priority switch
        {
            Priority.Low => MessagePriority.NonUrgent,
            Priority.Normal => MessagePriority.Normal,
            Priority.High => MessagePriority.Urgent,
            _ => message.Priority
        };

        return message;
    }

    /// <summary>
    /// Send the specified email.
    /// </summary>
    /// <returns>A response with any errors and a success boolean.</returns>
    /// <param name="email">Email.</param>
    /// <param name="token">Cancellation Token.</param>
    public SendResponse Send(IEmail email, CancellationToken? token = null)
    {
        var response = new SendResponse();
        using var message = CreateMailMessage(email);

        using (_logger.MeasureTime(nameof(Send)))
        {
            if (token?.IsCancellationRequested ?? false) return response;
            try
            {
                token?.ThrowIfCancellationRequested();
                CheckOptions();
                CheckMessage(message);

                if (smtpClientOptions.UsePickupDirectory)
                {
                    SaveToPickupDirectory(message, smtpClientOptions.MailPickupDirectory);
                    return response;
                }

                var server = smtpClientOptions.Server!;
                using var client = new SmtpClient
                {
                    SslProtocols = SslProtocols.None
                };
                client.Connect(
                    server,
                    smtpClientOptions.Port,
                    GetSecureSocketOptions(smtpClientOptions),
                    token.GetValueOrDefault());

                // Note: only needed if the SMTP server requires authentication
                if (smtpClientOptions.RequiresAuthentication)
                {
                    client.Authenticate(smtpClientOptions.User!, smtpClientOptions.Password!, token.GetValueOrDefault());
                }

                _ = client.Send(message, token.GetValueOrDefault());
                client.Disconnect(true, token.GetValueOrDefault());
            }
            catch (OperationCanceledException)
            {
                response.ErrorMessages.Add("Send canceled.");
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
            }
        }

        return response;
    }

    /// <summary>
    /// Send the specified email.
    /// </summary>
    /// <returns>A response with any errors and a success boolean.</returns>
    /// <param name="email">Email.</param>
    /// <param name="token">Cancellation Token.</param>
    public async Task<SendResponse> SendAsync(IEmail email, CancellationToken? token = null)
    {
        var response = new SendResponse();
        using var message = CreateMailMessage(email);

        using (_logger.MeasureTime(nameof(SendAsync)))
        {
            if (token?.IsCancellationRequested ?? false) return response;
            try
            {
                token?.ThrowIfCancellationRequested();
                CheckOptions();
                CheckMessage(message);

                if (smtpClientOptions.UsePickupDirectory)
                {
                    await SaveToPickupDirectoryAsync(message, smtpClientOptions.MailPickupDirectory).ConfigureAwait(false);
                    return response;
                }

                var server = smtpClientOptions.Server!;
                using var client = new SmtpClient
                {
                    SslProtocols = SslProtocols.None
                };
                await client.ConnectAsync(
                    server,
                    smtpClientOptions.Port,
                    GetSecureSocketOptions(smtpClientOptions),
                    token.GetValueOrDefault()).ConfigureAwait(false);

                // Note: only needed if the SMTP server requires authentication
                if (smtpClientOptions.RequiresAuthentication)
                {
                    await client.AuthenticateAsync(smtpClientOptions.User!, smtpClientOptions.Password!, token.GetValueOrDefault()).ConfigureAwait(false);
                }

                _ = await client.SendAsync(message, token.GetValueOrDefault()).ConfigureAwait(false);
                await client.DisconnectAsync(true, token.GetValueOrDefault()).ConfigureAwait(false);

                LogMailHasBeenSentSuccessfullyEmail(email);
            }
            catch (OperationCanceledException)
            {
                response.ErrorMessages.Add("Send canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                response.ErrorMessages.Add(ex.Message);
            }

            return response;
        }
    }

    public bool CanConnect()
    {
        SmtpClient? client = null;

        try
        {
            CheckOptions();

            var server = smtpClientOptions.Server!;
            client = new()
            {
                SslProtocols = SslProtocols.None
            };
            client.Connect(
                server,
                smtpClientOptions.Port,
                GetSecureSocketOptions(smtpClientOptions));

            if (smtpClientOptions.RequiresAuthentication)
            {
                client.Authenticate(smtpClientOptions.User!, smtpClientOptions.Password!);
            }

            client.Disconnect(true);
        }
        catch (Exception e)
        {
            LogSmtpConnectivityCheckFailed(e);
            return false;
        }
        finally
        {
            client?.Dispose();
        }

        return true;
    }

    public async Task<bool> CanConnectAsync()
    {
        SmtpClient? client = null;

        try
        {
            CheckOptions();
            const SslProtocols protocols = SslProtocols.None;

            var server = smtpClientOptions.Server!;
            client = new()
            {
                SslProtocols = protocols
            };
            await client.ConnectAsync(
                server,
                smtpClientOptions.Port,
                GetSecureSocketOptions(smtpClientOptions)).ConfigureAwait(false);

            if (smtpClientOptions.RequiresAuthentication)
            {
                await client.AuthenticateAsync(smtpClientOptions.User!, smtpClientOptions.Password!).ConfigureAwait(false);
            }

            await client.DisconnectAsync(true).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            client?.Dispose();
        }

        return true;
    }

    /// <summary>
    /// Saves email to a pickup directory.
    /// </summary>
    /// <param name="message">Message to save for pickup.</param>
    /// <param name="pickupDirectory">Pickup directory.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Ignore for using async")]
    private static async Task SaveToPickupDirectoryAsync(MimeMessage message, string? pickupDirectory)
    {
        Directory.CreateDirectory(pickupDirectory ?? string.Empty);
        var path = Path.Combine(pickupDirectory ?? string.Empty, Guid.NewGuid() + ".eml");

        await using var stream = new FileStream(path, FileMode.CreateNew);
        await message.WriteToAsync(stream).ConfigureAwait(false);
    }

    private static void SaveToPickupDirectory(MimeMessage message, string? pickupDirectory)
    {
        Directory.CreateDirectory(pickupDirectory ?? string.Empty);
        var path = Path.Combine(pickupDirectory ?? string.Empty, Guid.NewGuid() + ".eml");
        using var stream = new FileStream(path, FileMode.CreateNew);
        message.WriteTo(stream);
    }

    private static void CheckMessage(MimeMessage message)
    {
        if (message.From.Mailboxes.All(x => string.IsNullOrEmpty(x.Address)))
            throw new EmptySenderAddressesException();
    }

    private static SecureSocketOptions GetSecureSocketOptions(SmtpClientOptions options) =>
        options.UseSsl ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.None;

    private void CheckOptions()
    {
        if (smtpClientOptions.UsePickupDirectory)
        {
            if (string.IsNullOrWhiteSpace(smtpClientOptions.MailPickupDirectory))
                throw new ArgumentException("Pickup directory must be defined when UsePickupDirectory is enabled.");

            return;
        }

        if (string.IsNullOrWhiteSpace(smtpClientOptions.Server))
            throw new UndefinedServerException();

        if (smtpClientOptions.Port is < 1 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(smtpClientOptions.Port), "SMTP port must be between 1 and 65535.");

        switch (smtpClientOptions.RequiresAuthentication)
        {
            case true when string.IsNullOrWhiteSpace(smtpClientOptions.User):
                throw new ArgumentException("SMTP user must be defined when authentication is required.");
            case true when smtpClientOptions.Password is null:
                throw new ArgumentException("SMTP password must be defined when authentication is required.");
        }
    }

    [LoggerMessage(LogLevel.Information, "SMTP connectivity check failed.")]
    partial void LogSmtpConnectivityCheckFailed(Exception exception);

    [LoggerMessage(LogLevel.Information, "Mail has been sent successfully: {Email}")]
    partial void LogMailHasBeenSentSuccessfullyEmail(IEmail email);
}
