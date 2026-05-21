// -----------------------------------------------------------------------
// <copyright file="IMailService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.Utilities.Mail.Models;

namespace MyNet.Utilities.Mail;

/// <summary>
/// Interface for sending emails. Implement this interface to create a custom mail service.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Sends an email synchronously. This method will block the calling thread until the email is sent or an error occurs. Use this method when you need to ensure that the email is sent before proceeding with the next steps in your application. If you do not need to wait for the email to be sent, consider using the asynchronous SendAsync method instead, which allows for non-blocking operations and can improve the responsiveness of your application.
    /// </summary>
    /// <param name="email">The email message to send.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="SendResponse"/> indicating the result of the send operation.</returns>
    SendResponse Send(IEmail email, CancellationToken? token = null);

    /// <summary>
    /// Sends an email asynchronously. This method allows for non-blocking operations, improving the responsiveness of your application. Use this method when you do not need to wait for the email to be sent before proceeding with the next steps in your application.
    /// </summary>
    /// <param name="email">The email message to send.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task{SendResponse}"/> representing the asynchronous operation, with a <see cref="SendResponse"/> indicating the result of the send operation.</returns>
    Task<SendResponse> SendAsync(IEmail email, CancellationToken? token = null);

    /// <summary>
    /// Checks if the mail service can connect to the mail server synchronously.
    /// </summary>
    /// <returns>True if the connection is successful, otherwise false.</returns>
    bool CanConnect();

    /// <summary>
    /// Checks if the mail service can connect to the mail server asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task{Boolean}"/> representing the asynchronous operation, with a boolean indicating the result of the connection attempt.</returns>
    Task<bool> CanConnectAsync();
}
