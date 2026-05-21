// -----------------------------------------------------------------------
// <copyright file="WeakEventSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyNet.Utilities.Events;

/// <summary>
/// Provides a weak event source implementation that allows subscribers to be garbage collected if they are no longer referenced elsewhere, preventing memory leaks in event-driven scenarios.
/// </summary>
/// <typeparam name="TEventArgs">The type of event arguments.</typeparam>
public sealed class WeakEventSource<TEventArgs>
    where TEventArgs : EventArgs
{
    private readonly List<WeakSubscription> _subscriptions = [];

    /// <summary>
    /// Subscribes to the event with a weak reference to the handler, allowing the subscriber to be garbage collected if it is no longer referenced elsewhere. This helps prevent memory leaks in event-driven scenarios where subscribers may not always unsubscribe from events.
    /// </summary>
    /// <param name="handler">The event handler to subscribe.</param>
    public void Subscribe(EventHandler<TEventArgs> handler) => _subscriptions.Add(new(handler));

    /// <summary>
    /// Unsubscribes from the event.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe.</param>
    public void Unsubscribe(EventHandler<TEventArgs> handler) => _subscriptions.RemoveAll(x => x.Matches(handler));

    /// <summary>
    /// Raises the event, invoking all subscribed handlers.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event arguments.</param>
    public void Raise(object sender, TEventArgs args)
    {
        for (var i = _subscriptions.Count - 1; i >= 0; i--)
        {
            if (!_subscriptions[i].TryInvoke(sender, args))
                _subscriptions.RemoveAt(i);
        }
    }

    /// <summary>
    /// Represents a weak subscription to an event handler, holding a weak reference to the target object and the method information for invocation. This allows the target object to be garbage collected if it is no longer referenced elsewhere, preventing memory leaks in event-driven scenarios.
    /// </summary>
    private sealed class WeakSubscription
    {
        private readonly WeakReference? _targetReference;
        private readonly MethodInfo _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakSubscription"/> class with the specified event handler, storing a weak reference to the target object and the method information for invocation. This allows the target object to be garbage collected if it is no longer referenced elsewhere, preventing memory leaks in event-driven scenarios.
        /// </summary>
        /// <param name="handler">The event handler to subscribe.</param>
        public WeakSubscription(EventHandler<TEventArgs> handler)
        {
            _method = handler.Method;

            if (handler.Target != null)
                _targetReference = new(handler.Target);
        }

        /// <summary>
        /// Attempts to invoke the event handler with the specified sender and event arguments. If the target object has been garbage collected, the method will not be invoked and the subscription will be removed from the event source. This helps prevent memory leaks in event-driven scenarios where subscribers may not always unsubscribe from events.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event arguments.</param>
        /// <returns>True if the event handler was invoked; otherwise, false.</returns>
        public bool TryInvoke(object sender, TEventArgs args)
        {
            if (_targetReference == null)
            {
                _method.Invoke(null, [sender, args]);
                return true;
            }

            var target = _targetReference.Target;
            if (target == null)
                return false;

            _method.Invoke(target, [sender, args]);
            return true;
        }

        /// <summary>
        /// Determines whether the specified event handler matches the method and target of this weak subscription. This is used to identify and remove subscriptions when unsubscribing from the event source, ensuring that the correct handlers are removed while allowing for garbage collection of subscribers that are no longer referenced elsewhere.
        /// </summary>
        /// <param name="handler">The event handler to compare with this weak subscription.</param>
        /// <returns>True if the specified event handler matches the method and target of this weak subscription; otherwise, false.</returns>
        public bool Matches(EventHandler<TEventArgs> handler) =>
            _method == handler.Method &&
            _targetReference?.Target == handler.Target;
    }
}
