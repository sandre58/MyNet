// -----------------------------------------------------------------------
// <copyright file="MailSmtpService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Utilities.Mail.Models;
using Attachment = System.Net.Mail.Attachment;

namespace MyNet.Utilities.Mail.Smtp;

public sealed class MailSmtpService(SmtpClient smtpClient) : IMailService, IDisposable
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose in class Dispose()")]
    public MailSmtpService(SmtpClientOptions options)
        : this(CreateSmtpClient(options))
    {
    }

    private static SmtpClient CreateSmtpClient(SmtpClientOptions options)
    {
        var client = new SmtpClient(options.Server, options.Port)
        {
            EnableSsl = options.UseSsl,
            PickupDirectoryLocation = options.MailPickupDirectory,
            DeliveryMethod = options.UsePickupDirectory
                ? SmtpDeliveryMethod.SpecifiedPickupDirectory
                : SmtpDeliveryMethod.Network,
            UseDefaultCredentials = !options.RequiresAuthentication
        };

        if (options.RequiresAuthentication)
        {
            client.Credentials = new NetworkCredential(options.User, options.Password);
        }

        return client;
    }

    public static MailMessage CreateMailMessage(IEmail email)
    {
        var data = email.Data;
        MailMessage? message;

        // Smtp seems to require the HTML version as the alternative.
        if (!string.IsNullOrEmpty(data.PlaintextAlternativeBody))
        {
            message = new()
            {
                Subject = data.Subject,
                Body = data.PlaintextAlternativeBody,
                IsBodyHtml = false,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new(data.From.Address, data.From.Name)
            };

            var mimeType = new ContentType("text/html; charset=UTF-8");
            var alternate = AlternateView.CreateAlternateViewFromString(data.Body, mimeType);
            message.AlternateViews.Add(alternate);
        }
        else
        {
            message = new()
            {
                Subject = data.Subject,
                Body = data.Body,
                IsBodyHtml = data.IsHtml,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new(data.From.Address, data.From.Name)
            };
        }

        foreach (var header in data.Headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        data.To.ForEach(x => message.To.Add(new MailAddress(x.Address, x.Name)));
        data.Cc.ForEach(x => message.CC.Add(new MailAddress(x.Address, x.Name)));
        data.Bcc.ForEach(x => message.Bcc.Add(new MailAddress(x.Address, x.Name)));
        data.ReplyTo.ForEach(x => message.ReplyToList.Add(new MailAddress(x.Address, x.Name)));

        message.Priority = data.Priority switch
        {
            Priority.Low => MailPriority.Low,
            Priority.Normal => MailPriority.Normal,
            Priority.High => MailPriority.High,
            _ => message.Priority
        };

        data.Attachments.ForEach(x =>
        {
            if (x.Data == null) return;
            var a = new Attachment(x.Data, x.Filename, x.ContentType) { ContentId = x.ContentId };

            message.Attachments.Add(a);
        });

        return message;
    }

    public SendResponse Send(IEmail email, CancellationToken? token = null) =>
        SendAsync(email, token).ConfigureAwait(false).GetAwaiter().GetResult();

    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Intended to be called from synchronous code")]
    public async Task<SendResponse> SendAsync(IEmail email, CancellationToken? token = null)
    {
        var response = new SendResponse();

        if (token?.IsCancellationRequested ?? false)
        {
            response.ErrorMessages.Add("Send canceled.");
            return response;
        }

        try
        {
            token?.ThrowIfCancellationRequested();

            using var message = CreateMailMessage(email);
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            smtpClient.SendCompleted += handler;
            try
            {
                smtpClient.SendAsync(message, tcs);
                await using var registration = token?.Register(static state => ((SmtpClient)state!).SendAsyncCancel(), smtpClient);
                _ = await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                smtpClient.SendCompleted -= handler;
            }

            void handler(object? s, AsyncCompletedEventArgs e)
            {
                _ = e.UserState != tcs
                    ? tcs.TrySetException(new InvalidOperationException("Unexpected UserState"))
                    : e.Cancelled
                        ? tcs.TrySetCanceled()
                        : e.Error != null
                            ? tcs.TrySetException(e.Error)
                            : tcs.TrySetResult(true);
            }
        }
        catch (OperationCanceledException)
        {
            response.ErrorMessages.Add("Send canceled.");
        }
        catch (Exception ex)
        {
            response.ErrorMessages.Add(ex.Message);
        }

        return response;
    }

    public bool CanConnect() => SmtpHelper.TestSmtpConnection(smtpClient.Host, smtpClient.Port);

    public Task<bool> CanConnectAsync() => Task.FromResult(CanConnect());

    public void Dispose() => smtpClient.Dispose();
}
