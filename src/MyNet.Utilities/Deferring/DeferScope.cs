// -----------------------------------------------------------------------
// <copyright file="DeferScope.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Deferring;

/// <summary>
/// Internal scope type representing a deferral. Disposing this instance ends the scope.
/// </summary>
internal sealed class DeferScope : IDisposable
{
    private readonly Deferrer _deferrerScopeBase;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeferScope"/> class and registers it with the provided <see cref="Deferrer"/>.
    /// </summary>
    /// <param name="sender">The deferrer that created this scope.</param>
    public DeferScope(Deferrer sender)
    {
        _deferrerScopeBase = sender;

        _deferrerScopeBase.Push(this);
    }

    /// <summary>
    /// Ends the deferral scope and triggers deferred execution if no other scopes remain.
    /// </summary>
    public void Dispose()
    {
        _deferrerScopeBase.Pop();

        _deferrerScopeBase.EndDefer();
    }
}
