// -----------------------------------------------------------------------
// <copyright file="CustomExpirationPolicy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Caching.Policies;

/// <summary>
/// The custom expiration policy.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CustomExpirationPolicy"/> class.
/// </remarks>
/// <param name="isExpiredFunc">
/// The function to check if the policy is expired.
/// </param>
/// <param name="resetAction">
/// The action that will be executed if the item is read before expiration.
/// </param>
public sealed class CustomExpirationPolicy(Func<bool> isExpiredFunc, Action? resetAction = null) : ExpirationPolicy(resetAction is not null)
{
    #region Fields

    /// <summary>
    /// The function to check if the policy is expired.
    /// </summary>
    private readonly Func<bool> _isExpiredFunc = isExpiredFunc ?? throw new ArgumentNullException(nameof(isExpiredFunc));

    /// <summary>
    ///  The action that will be executed if the item is read before expiration.
    /// </summary>
    private readonly Action? _resetAction = resetAction;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether is expired.
    /// </summary>
    public override bool IsExpired => _isExpiredFunc.Invoke();
    #endregion

    #region Methods

    /// <summary>
    /// Called when the policy is resetting.
    /// </summary>
    protected override void OnReset() => _resetAction?.Invoke();

    #endregion
}
