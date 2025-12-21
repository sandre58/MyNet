// -----------------------------------------------------------------------
// <copyright file="WeakEventSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyNet.Utilities.Events;

public sealed class WeakEventSource<TEventArgs>
    where TEventArgs : EventArgs
{
    private readonly List<WeakHandler> _handlers = [];

    public void Subscribe(EventHandler<TEventArgs> handler) => _handlers.Add(new WeakHandler(handler));

    public void Unsubscribe(EventHandler<TEventArgs> handler) => _handlers.RemoveAll(h => h.Matches(handler));

    public void Raise(object sender, TEventArgs args)
    {
        for (var i = _handlers.Count - 1; i >= 0; i--)
        {
            if (!_handlers[i].Invoke(sender, args))
                _handlers.RemoveAt(i);
        }
    }

    private sealed class WeakHandler(Delegate handler)
    {
        private readonly WeakReference? _target = handler.Target != null
                ? new WeakReference(handler.Target)
                : null;

        private readonly MethodInfo _method = handler.Method;

        public bool Invoke(object sender, TEventArgs args)
        {
            if (_target == null)
            {
                _method.Invoke(null, [sender, args]);
                return true;
            }

            var target = _target.Target;
            if (target == null)
                return false;

            _method.Invoke(target, [sender, args]);
            return true;
        }

        public bool Matches(Delegate handler)
            => _method == handler.Method && _target?.Target == handler.Target;
    }
}
