// -----------------------------------------------------------------------
// <copyright file="MailKitService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using MyNet.Mail.MailKit.Exceptions;
using MyNet.Mail.Models;
using MyNet.Mail.Smtp;
using MyNet.Utilities;
using MyNet.Utilities.Logging;

namespace MyNet.Mail.MailKit;

/// <summary>
/// Sends email through MailKit using the supplied <see cref="SmtpClientOptions"/>.
/// Reuses a single SMTP connection per service instance; call <see cref="Dispose"/> when done.
/// </summary>
/// <param name="smtpClientOptions">SMTP settings for this sender.</param>
/// <param name="logger">Optional logger; defaults to <see cref="Log.Create{T}"/>.</param>
public sealed partial class MailKitService(SmtpClientOptions smtpClientOptions, ILogger? logger = null) : IMailService, IDisposable
{
    private readonly ILogger _logger = logger ?? Log.Create<MailKitService>();
    private readonly MailKitSmtpConnectionManager _connectionManager = new(smtpClientOptions);
    private bool _disposed;

    /// <summary>
    /// Builds a <see cref="MimeMessage"/> from <see cref="IEmail"/> data.
    /// </summary>
    public static MimeMessage CreateMailMessage(IEmail email, Encoding? preferredEncoding = null)
    {
        ArgumentNullException.ThrowIfNull(email);

        var data = email.Data;
        var encoding = preferredEncoding ?? Encoding.UTF8;

        var message = new MimeMessage { Subject = data.Subject };

        message.From.Add(new MailboxAddress(data.From.Name, data.From.Address));

        foreach (var address in data.To)
        {
            message.To.Add(new MailboxAddress(address.Name, address.Address));
        }

        foreach (var address in data.Cc)
        {
            message.Cc.Add(new MailboxAddress(address.Name, address.Address));
        }

        foreach (var address in data.Bcc)
        {
            message.Bcc.Add(new MailboxAddress(address.Name, address.Address));
        }

        foreach (var address in data.ReplyTo)
        {
            message.ReplyTo.Add(new MailboxAddress(address.Name, address.Address));
        }

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

        foreach (var attachment in data.Attachments)
        {
            if (attachment.Data is null)
            {
                continue;
            }

            var mimeAttachment = builder.Attachments.Add(attachment.Filename, attachment.Data, ContentType.Parse(attachment.ContentType));
            if (!string.IsNullOrWhiteSpace(attachment.ContentId))
            {
                mimeAttachment.ContentId = attachment.ContentId;
            }
        }

        message.Body = builder.ToMessageBody();
        ApplyCharset(message.Body, encoding);

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

    /// <inheritdoc />
    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Synchronous wrapper over SendAsync")]
    public SendResponse Send(IEmail email, CancellationToken? token = null) => SendAsync(email, token).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <inheritdoc />
    public async Task<SendResponse> SendAsync(IEmail email, CancellationToken? token = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var response = new SendResponse();
        var cancellationToken = token ?? CancellationToken.None;

        using (_logger.MeasureTime(nameof(SendAsync)))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                AddCanceled(response);
                return response;
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                MailKitSmtpOptionsValidator.Validate(smtpClientOptions);

                using var message = CreateMailMessage(email, MailEncodingResolver.Resolve(smtpClientOptions));
                CheckMessage(message);

                if (smtpClientOptions.UsePickupDirectory)
                {
                    await SaveToPickupDirectoryAsync(message, smtpClientOptions.MailPickupDirectory, cancellationToken).ConfigureAwait(false);
                    response.MessageId = message.MessageId ?? string.Empty;
                    LogMailHasBeenSentSuccessfullyEmail(email);
                    return response;
                }

                response.MessageId = await _connectionManager.SendAsync(message, cancellationToken).ConfigureAwait(false);
                LogMailHasBeenSentSuccessfullyEmail(email);
            }
            catch (OperationCanceledException)
            {
                AddCanceled(response);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                response.ErrorMessages.Add(ex.Message);
            }
        }

        return response;
    }

    /// <inheritdoc />
    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Synchronous wrapper over CanConnectAsync")]
    public bool CanConnect() => CanConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    /// <inheritdoc />
    public async Task<bool> CanConnectAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        SmtpClient? client = null;
        try
        {
            MailKitSmtpOptionsValidator.Validate(smtpClientOptions);
            if (smtpClientOptions.UsePickupDirectory)
            {
                return Directory.Exists(smtpClientOptions.MailPickupDirectory);
            }

            client = new();
            await ConnectAndAuthenticateAsync(client, CancellationToken.None).ConfigureAwait(false);
            await client.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            LogSmtpConnectivityCheckFailed(ex);
            return false;
        }
        finally
        {
            client?.Dispose();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _connectionManager.Dispose();
    }

    private static void ApplyCharset(MimeEntity entity, Encoding encoding)
    {
        switch (entity)
        {
            case TextPart textPart:
                textPart.ContentType.Charset = encoding.WebName;
                break;
            case Multipart multipart:
                foreach (var part in multipart)
                {
                    ApplyCharset(part, encoding);
                }

                break;
        }
    }

    private static void CheckMessage(MimeMessage message)
    {
        if (message.From.Mailboxes.All(x => string.IsNullOrEmpty(x.Address)))
        {
            throw new EmptySenderAddressesException();
        }
    }

    private static void AddCanceled(SendResponse response) => response.ErrorMessages.Add("Send canceled.");

    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "await using on FileStream")]
    private static async Task SaveToPickupDirectoryAsync(MimeMessage message, string? pickupDirectory, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(pickupDirectory ?? string.Empty);
        var path = Path.Combine(pickupDirectory ?? string.Empty, Guid.NewGuid() + ".eml");

        await using var stream = new FileStream(path, FileMode.CreateNew);
        await message.WriteToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    private async Task ConnectAndAuthenticateAsync(SmtpClient client, CancellationToken cancellationToken)
    {
        await client.ConnectAsync(
            smtpClientOptions.Server!,
            smtpClientOptions.Port,
            SmtpSecureSocketOptionsResolver.Resolve(smtpClientOptions),
            cancellationToken).ConfigureAwait(false);

        if (smtpClientOptions.RequiresAuthentication)
        {
            await client.AuthenticateAsync(smtpClientOptions.User!, smtpClientOptions.Password!, cancellationToken).ConfigureAwait(false);
        }
    }

    [LoggerMessage(LogLevel.Information, "SMTP connectivity check failed.")]
    partial void LogSmtpConnectivityCheckFailed(Exception exception);

    [LoggerMessage(LogLevel.Information, "Mail has been sent successfully: {Email}")]
    partial void LogMailHasBeenSentSuccessfullyEmail(IEmail email);
}
