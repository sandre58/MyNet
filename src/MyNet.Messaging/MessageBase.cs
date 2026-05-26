// -----------------------------------------------------------------------
// <copyright file="MessageBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Messaging;

/// <summary>
/// Base class for all messages broadcasted by the Messenger.
/// You can create your own message types by extending this class.
/// </summary>
public class MessageBase
{
    public MessageBase()
    {
    }

    public MessageBase(object sender) => Sender = sender;

    public MessageBase(object sender, object target)
        : this(sender) => Target = target;

    /// <summary>
    /// Gets or sets the message's sender.
    /// </summary>
    public object? Sender
    {
        get;
        protected set;
    }

    /// <summary>
    /// Gets or sets the message's intended target. This property can be used
    /// to give an indication as to whom the message was intended for. Of course
    /// this is only an indication, amd may be null.
    /// </summary>
    public object? Target
    {
        get;
        protected set;
    }
}
