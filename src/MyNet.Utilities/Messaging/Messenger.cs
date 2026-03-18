// -----------------------------------------------------------------------
// <copyright file="Messenger.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace MyNet.Utilities.Messaging;

/// <summary>
/// The Messenger is a class allowing objects to exchange messages.
/// </summary>
public class Messenger : IMessenger
{
    private static readonly Lock CreationLock = new();
    private readonly Lock _recipientsOfSubclassesActionLock = new();
    private readonly Lock _recipientsStrictActionLock = new();
    private readonly Lock _registerLock = new();
    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsOfSubclassesAction;
    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsStrictAction;

    /// <summary>
    /// Gets the Messenger's default instance, allowing
    /// to register and send messages in a static manner.
    /// </summary>
    public static IMessenger? Default
    {
        get
        {
            if (field != null) return field;
            lock (CreationLock)
            {
                field = new Messenger();
            }

            return field;
        }

        private set;
    }

    /// <summary>
    /// Provides a way to override the Messenger.Default instance with
    /// a custom instance, for example for unit testing purposes.
    /// </summary>
    /// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
    public static void OverrideDefault(IMessenger newMessenger) => Default = newMessenger;

    /// <summary>
    /// Sets the Messenger's default (static) instance to null.
    /// </summary>
    public static void Reset() => Default = null;

    #region IMessenger Members

    /// <summary>
    /// Registers a recipient for a type of message TMessage. The action
    /// parameter will be executed when a corresponding message is sent.
    /// <para>Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers
    /// for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.</param>
    public virtual void Register<TMessage>(
        object recipient,
        Action<TMessage> action,
        bool keepTargetAlive = false) => Register(recipient, null, false, action, keepTargetAlive);

    /// <summary>
    /// Registers a recipient for a type of message TMessage.
    /// The action parameter will be executed when a corresponding
    /// message is sent. See the receiveDerivedMessagesToo parameter
    /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
    /// messages implementing TMessage) can be received too.
    /// <para>Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.</para>
    /// <para>However, if you use closures and set keepTargetAlive to true, you might
    /// cause a memory leak if you don't call <see cref="Unregister"/> when you are cleaning up.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers
    /// for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token for a messaging channel. If a recipient registers
    /// using a token, and a sender sends a message using the same token, then this
    /// message will be delivered to the recipient. Other recipients who did not
    /// use a token when registering (or who used a different token) will not
    /// get the message. Similarly, messages sent without any token, or with a different
    /// token, will not be delivered to that recipient.</param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.</param>
    public virtual void Register<TMessage>(
        object recipient,
        object token,
        Action<TMessage> action,
        bool keepTargetAlive = false) => Register(recipient, token, false, action, keepTargetAlive);

    /// <summary>
    /// Registers a recipient for a type of message TMessage.
    /// The action parameter will be executed when a corresponding
    /// message is sent. See the receiveDerivedMessagesToo parameter
    /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
    /// messages implementing TMessage) can be received too.
    /// <para>Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers
    /// for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token for a messaging channel. If a recipient registers
    /// using a token, and a sender sends a message using the same token, then this
    /// message will be delivered to the recipient. Other recipients who did not
    /// use a token when registering (or who used a different token) will not
    /// get the message. Similarly, messages sent without any token, or with a different
    /// token, will not be delivered to that recipient.</param>
    /// <param name="receiveDerivedMessagesToo">If true, message types deriving from
    /// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
    /// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
    /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
    /// and ExecuteOrderMessage to the recipient that registered.
    /// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
    /// transmitted to the recipient. For example, if a SendOrderMessage
    /// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
    /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
    /// and ExecuteOrderMessage to the recipient that registered.</para>
    /// </param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.</param>
    public virtual void Register<TMessage>(
        object? recipient,
        object? token,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        lock (_registerLock)
        {
            var messageType = typeof(TMessage);

            Dictionary<Type, List<WeakActionAndToken>> recipients;

            if (receiveDerivedMessagesToo)
            {
                _recipientsOfSubclassesAction ??= [];

                recipients = _recipientsOfSubclassesAction;
            }
            else
            {
                _recipientsStrictAction ??= [];

                recipients = _recipientsStrictAction;
            }

            lock (recipients)
            {
                List<WeakActionAndToken> list;

                if (!recipients.TryGetValue(messageType, out var value))
                {
                    list = [];
                    recipients.Add(messageType, list);
                }
                else
                {
                    list = value;
                }

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                var weakAction = new WeakAction<TMessage>(recipient, action, keepTargetAlive);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

                var item = new WeakActionAndToken
                {
                    Action = weakAction,
                    Token = token
                };

                list.Add(item);
            }
        }

        Cleanup();
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage.
    /// The action parameter will be executed when a corresponding
    /// message is sent. See the receiveDerivedMessagesToo parameter
    /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
    /// messages implementing TMessage) can be received too.
    /// <para>Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers
    /// for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="receiveDerivedMessagesToo">If true, message types deriving from
    /// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
    /// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
    /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
    /// and ExecuteOrderMessage to the recipient that registered.
    /// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
    /// transmitted to the recipient. For example, if a SendOrderMessage
    /// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
    /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
    /// and ExecuteOrderMessage to the recipient that registered.</para>
    /// </param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.</param>
    public virtual void Register<TMessage>(
        object recipient,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive = false) => Register(recipient, null, receiveDerivedMessagesToo, action, keepTargetAlive);

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach all recipients that registered for this message type
    /// using one of the Register methods.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    public virtual void Send<TMessage>(TMessage message) => SendToTargetOrType(message, null, null);

    public virtual void Send<TMessage>() => SendToTargetOrType(Activator.CreateInstance<TMessage>(), null, null);

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach only recipients that registered for this message type
    /// using one of the Register methods, and that are
    /// of the targetType.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <typeparam name="TTarget">The type of recipients that will receive
    /// the message. The message won't be sent to recipients of another type.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    public virtual void Send<TMessage, TTarget>(TMessage message) => SendToTargetOrType(message, typeof(TTarget), null);

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach only recipients that registered for this message type
    /// using one of the Register methods, and that are
    /// of the targetType.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    /// <param name="token">A token for a messaging channel. If a recipient registers
    /// using a token, and a sender sends a message using the same token, then this
    /// message will be delivered to the recipient. Other recipients who did not
    /// use a token when registering (or who used a different token) will not
    /// get the message. Similarly, messages sent without any token, or with a different
    /// token, will not be delivered to that recipient.</param>
    public virtual void Send<TMessage>(TMessage message, object token) => SendToTargetOrType(message, null, token);

    /// <summary>
    /// Unregisters a message recipient completely. After this method
    /// is executed, the recipient will not receive any messages anymore.
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    public virtual void Unregister(object? recipient)
    {
        UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
        UnregisterFromLists(recipient, _recipientsStrictAction);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages only.
    /// After this method is executed, the recipient will not receive messages
    /// of type TMessage anymore, but will still receive other message types (if it
    /// registered for them previously).
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <typeparam name="TMessage">The type of messages that the recipient wants
    /// to unregister from.</typeparam>
    public virtual void Unregister<TMessage>(object? recipient) => Unregister<TMessage>(recipient, null, null);

    /// <summary>
    /// Unregisters a message recipient for a given type of messages only and for a given token.
    /// After this method is executed, the recipient will not receive messages
    /// of type TMessage anymore with the given token, but will still receive other message types
    /// or messages with other tokens (if it registered for them previously).
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="token">The token for which the recipient must be unregistered.</param>
    /// <typeparam name="TMessage">The type of messages that the recipient wants
    /// to unregister from.</typeparam>
    public virtual void Unregister<TMessage>(object? recipient, object? token) => Unregister<TMessage>(recipient, token, null);

    /// <summary>
    /// Unregisters a message recipient for a given type of messages and for
    /// a given action. Other message types will still be transmitted to the
    /// recipient (if it registered for them previously). Other actions that have
    /// been registered for the message type TMessage and for the given recipient (if
    /// available) will also remain available.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages that the recipient wants
    /// to unregister from.</typeparam>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="action">The action that must be unregistered for
    /// the recipient and for the message type TMessage.</param>
    public virtual void Unregister<TMessage>(object? recipient, Action<TMessage>? action) => Unregister(recipient, null, action);

    /// <summary>
    /// Unregisters a message recipient for a given type of messages, for
    /// a given action and a given token. Other message types will still be transmitted to the
    /// recipient (if it registered for them previously). Other actions that have
    /// been registered for the message type TMessage, for the given recipient and other tokens (if
    /// available) will also remain available.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages that the recipient wants
    /// to unregister from.</typeparam>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="token">The token for which the recipient must be unregistered.</param>
    /// <param name="action">The action that must be unregistered for
    /// the recipient and for the message type TMessage.</param>
    public virtual void Unregister<TMessage>(object? recipient, object? token, Action<TMessage>? action)
    {
        UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
        UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
        Cleanup();
    }

    #endregion

    /// <summary>
    /// Scans the recipients' lists for "dead" instances and removes them.
    /// Since recipients are stored as <see cref="WeakReference"/>,
    /// recipients can be garbage collected even though the Messenger keeps
    /// them in a list. During the cleanup operation, all "dead"
    /// recipients are removed from the lists. Since this operation
    /// can take a moment, it is only executed when the application is
    /// idle. For this reason, a user of the Messenger class should use
    /// RequestCleanup instead of forcing one with the
    /// <see cref="Cleanup" /> method.
    /// </summary>
    public void Cleanup()
    {
        CleanupList(_recipientsOfSubclassesAction);
        CleanupList(_recipientsStrictAction);
    }

    private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (lists == null)
        {
            return;
        }

        lock (lists)
        {
            var listsToRemove = new List<Type>();
            foreach (var list in lists)
            {
                var recipientsToRemove = list.Value
                    .Where(item => item.Action is not { IsAlive: true })
                    .ToList();

                foreach (var recipient in recipientsToRemove)
                {
                    _ = list.Value.Remove(recipient);
                }

                if (list.Value.Count == 0)
                {
                    listsToRemove.Add(list.Key);
                }
            }

            foreach (var key in listsToRemove)
            {
                _ = lists.Remove(key);
            }
        }
    }

    private static void SendToList<TMessage>(
        TMessage? message,
        IEnumerable<WeakActionAndToken>? weakActionsAndTokens,
        Type? messageTargetType,
        object? token)
    {
        if (weakActionsAndTokens == null) return;

        // Clone to protect from people registering in a "receive message" method
        // Correction Messaging BL0004.007
        var list = weakActionsAndTokens.ToList();
        var listClone = list.Take(list.Count).ToList();

        foreach (var item in listClone)
        {
            if (item.Action is IExecuteWithObject executeAction
                && item.Action is { IsAlive: true, Target: not null }
                && (messageTargetType == null
                    || item.Action.Target.GetType() == messageTargetType
                    || messageTargetType.IsInstanceOfType(item.Action.Target))
                && ((item.Token == null && token == null)
                    || (item.Token?.Equals(token) == true)))
            {
                executeAction.ExecuteWithObject(message);
            }
        }
    }

    private static void UnregisterFromLists(object? recipient, Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (recipient == null
            || lists is not { Count: not 0 })
        {
            return;
        }

        lock (lists)
        {
            foreach (var weakAction in lists.Keys.SelectMany(messageType => lists[messageType].Select(item => (IExecuteWithObject?)item.Action).OfType<IExecuteWithObject>().Where(weakAction => recipient == weakAction.Target)))
            {
                weakAction.MarkForDeletion();
            }
        }
    }

    private static void UnregisterFromLists<TMessage>(
        object? recipient,
        object? token,
        Action<TMessage>? action,
        Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        var messageType = typeof(TMessage);

        if (recipient == null
            || lists is not { Count: not 0 }
            || !lists.TryGetValue(messageType, out var value))
        {
            return;
        }

        lock (lists)
        {
            foreach (var item in value)
            {
                if (item.Action is WeakAction<TMessage> weakActionCasted
                    && recipient == weakActionCasted.Target
                    && (action == null
                        || action.Method.Name == weakActionCasted.MethodName)
                    && (token?.Equals(item.Token) != false))
                {
                    item.Action.MarkForDeletion();
                }
            }
        }
    }

    private void SendToTargetOrType<TMessage>(TMessage? message, Type? messageTargetType, object? token)
    {
        var messageType = typeof(TMessage);

        if (_recipientsOfSubclassesAction != null)
        {
            // Clone to protect from people registering in a "receive message" method
            // Correction Messaging BL0008.002
            var listClone =
                _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count).ToList();

            foreach (var type in listClone)
            {
                List<WeakActionAndToken>? list = null;

                if (messageType == type
                    || messageType.IsSubclassOf(type)
                    || type.IsAssignableFrom(messageType))
                {
                    lock (_recipientsOfSubclassesActionLock)
                    {
                        list = [.. _recipientsOfSubclassesAction[type].Take(_recipientsOfSubclassesAction[type].Count)];
                    }
                }

                SendToList(message, list, messageTargetType, token);
            }
        }

        if (_recipientsStrictAction != null)
        {
            List<WeakActionAndToken>? list = null;

            lock (_recipientsStrictActionLock)
            {
                if (_recipientsStrictAction.TryGetValue(messageType, out var value))
                {
                    list = [.. value.Take(value.Count)];
                }
            }

            if (list != null)
            {
                SendToList(message, list, messageTargetType, token);
            }
        }

        Cleanup();
    }

    #region Nested type: WeakActionAndToken

    private readonly record struct WeakActionAndToken
    {
        public WeakAction? Action { get; init; }

        public object? Token { get; init; }
    }

    #endregion
}
