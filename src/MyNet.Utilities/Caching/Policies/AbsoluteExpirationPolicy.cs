// -----------------------------------------------------------------------
// <copyright file="AbsoluteExpirationPolicy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Caching.Policies;

/// <summary>
/// The cache item will expire on the absolute expiration date time.
/// </summary>
public class AbsoluteExpirationPolicy : ExpirationPolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteExpirationPolicy"/> class.
    /// </summary>
    /// <param name="absoluteExpirationDateTime">
    /// The expiration date time.
    /// </param>
    internal AbsoluteExpirationPolicy(DateTime absoluteExpirationDateTime)
        : this(absoluteExpirationDateTime, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteExpirationPolicy"/> class.
    /// </summary>
    /// <param name="absoluteExpirationDateTime">
    /// The expiration date time.
    /// </param>
    /// <param name="canReset">
    /// The can reset.
    /// </param>
    protected AbsoluteExpirationPolicy(DateTime absoluteExpirationDateTime, bool canReset)
        : base(canReset) => AbsoluteExpirationDateTime = absoluteExpirationDateTime.ToUniversalTime();

    /// <summary>
    /// Gets a value indicating whether is expired.
    /// </summary>
    public override bool IsExpired => DateTime.UtcNow > AbsoluteExpirationDateTime;

    /// <summary>
    /// Gets or sets the expiration date time.
    /// </summary>
    protected DateTime AbsoluteExpirationDateTime { get; set; }
}
