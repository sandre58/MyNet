// -----------------------------------------------------------------------
// <copyright file="IdentityFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using MyNet.Fakers.Text;
using MyNet.Generator;

namespace MyNet.Fakers.Identity;

/// <summary>
///  Current implementation of <see cref="IIdentityFaker"/>.
/// </summary>
/// <param name="random">The random generator instance.</param>
/// <param name="text">The text faker instance.</param>
public sealed class IdentityFaker(IRandomGenerator random, ITextFaker text) : IIdentityFaker
{
    /// <inheritdoc />
    public string Username() => $"{text.Word()}{random.Int(100, 9999)}";

    /// <inheritdoc />
    public string Password(int length = 12)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        var builder = new StringBuilder(length);

        for (var i = 0; i < length; i++)
            builder.Append((char)random.Int(33, 127));

        return builder.ToString();
    }
}
