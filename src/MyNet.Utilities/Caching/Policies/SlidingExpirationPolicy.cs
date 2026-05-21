// -----------------------------------------------------------------------
// <copyright file="SlidingExpirationPolicy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Caching.Policies;

/// <summary>
/// The cache item will expire using the duration property as the sliding expiration.
/// </summary>
public sealed class SlidingExpirationPolicy : DurationExpirationPolicy
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SlidingExpirationPolicy"/> class.
    /// </summary>
    /// <param name="durationTimeSpan">
    /// The expiration.
    /// </param>
    internal SlidingExpirationPolicy(TimeSpan durationTimeSpan)
        : base(durationTimeSpan, true)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The reset.
    /// </summary>
    protected override void OnReset() => AbsoluteExpirationDateTime = DateTime.UtcNow.Add(DurationTimeSpan);

    #endregion
}
