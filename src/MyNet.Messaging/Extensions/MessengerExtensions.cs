// -----------------------------------------------------------------------
// <copyright file="MessengerExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Messaging;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for IMessenger to provide convenient utilities and higher-level APIs.
/// </summary>
public static class MessengerExtensions
{
    extension(IMessenger messenger)
    {
        /// <summary>
        /// Registers a recipient to receive a message once, then automatically unregisters.
        /// Useful for one-time notifications.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to listen for.</typeparam>
        /// <param name="recipient">The recipient object.</param>
        /// <param name="action">The action to execute once.</param>
        public void RegisterOnce<TMessage>(object recipient,
            Action<TMessage> action)
        {
            Action<TMessage>? wrappedAction = null;
            wrappedAction = msg =>
            {
                try
                {
                    action(msg);
                }
                finally
                {
                    if (wrappedAction != null)
                    {
                        messenger.Unregister(recipient, wrappedAction);
                    }
                }
            };

            messenger.Register(recipient, wrappedAction, keepTargetAlive: true);
        }

        /// <summary>
        /// Registers a recipient to receive derived messages of a type.
        /// Convenient shorthand for Register with receiveDerivedMessagesToo=true.
        /// </summary>
        /// <typeparam name="TMessage">The base message type to listen for.</typeparam>
        /// <param name="recipient">The recipient object.</param>
        /// <param name="action">The action to execute.</param>
        public void RegisterForDerivedMessages<TMessage>(object recipient,
            Action<TMessage> action)
            where TMessage : class =>
            messenger.Register(recipient, true, action, keepTargetAlive: false);

        /// <summary>
        /// Registers a recipient to receive messages on a specific channel (token).
        /// </summary>
        /// <typeparam name="TMessage">The type of message to listen for.</typeparam>
        /// <param name="recipient">The recipient object.</param>
        /// <param name="channel">The channel identifier.</param>
        /// <param name="action">The action to execute.</param>
        public void RegisterOnChannel<TMessage>(object recipient,
            string channel,
            Action<TMessage> action) =>
            messenger.Register(recipient, channel, action, keepTargetAlive: false);

        /// <summary>
        /// Sends a message on a specific channel (token).
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <param name="channel">The channel identifier.</param>
        public void SendOnChannel<TMessage>(TMessage message,
            string channel) =>
            messenger.Send(message, channel);

        /// <summary>
        /// Sends a message only to recipients of a specific type.
        /// Convenient shorthand for Send{TMessage, TTarget}.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <typeparam name="TTarget">The type of recipients to target.</typeparam>
        /// <param name="message">The message to send.</param>
        public void SendTo<TMessage, TTarget>(TMessage message)
            where TTarget : class =>
            messenger.Send<TMessage, TTarget>(message);

        /// <summary>
        /// Unregisters a recipient from all message types and channels.
        /// Convenient shorthand for standalone Unregister.
        /// </summary>
        /// <param name="recipient">The recipient to unregister.</param>
        public void UnregisterAll(object recipient) => messenger.Unregister(recipient);

        /// <summary>
        /// Unregisters a recipient from a specific token/channel.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <param name="channel">The channel identifier.</param>
        public void UnregisterFromChannel<TMessage>(object recipient, string channel) => messenger.Unregister<TMessage>(recipient, channel);

        /// <summary>
        /// Creates a predicate-based observer that registers for messages and filters them.
        /// </summary>
        /// <typeparam name="TMessage">The type of message.</typeparam>
        /// <param name="recipient">The recipient object.</param>
        /// <param name="predicate">The filter predicate.</param>
        /// <param name="action">The action to execute when predicate matches.</param>
        public void RegisterWithFilter<TMessage>(object recipient,
            Func<TMessage, bool> predicate,
            Action<TMessage> action)
        {
            Action<TMessage> filteredAction = msg =>
            {
                if (predicate(msg))
                {
                    action(msg);
                }
            };

            messenger.Register<TMessage>(
                recipient,
                filteredAction,
                keepTargetAlive: true);
        }

        /// <summary>
        /// Sends a broadcast message with optional filtering by target type.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="messageFactory">Factory function to create the message.</param>
        public void SendFromFactory<TMessage>(Func<TMessage> messageFactory)
        {
            var message = messageFactory();
            messenger.Send(message);
        }
    }
}
