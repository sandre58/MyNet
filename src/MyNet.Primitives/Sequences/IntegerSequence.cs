// -----------------------------------------------------------------------
// <copyright file="IntegerSequence.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Primitives.Sequences;

/// <summary>
/// A simple sequence generator. Note: it is NOT thread-safe.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IntegerSequence"/> class.
/// </remarks>
/// <param name="seed">The sequence initial value.</param>
public class IntegerSequence(uint seed) : ISequence<uint>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerSequence"/> class.
    /// The sequence's initial current value is 0.
    /// </summary>
    public IntegerSequence()
        : this(0u) { }

    /// <summary>
    /// Gets this sequence's current value.
    /// </summary>
    public uint CurrentValue { get; private set; } = seed;

    /// <summary>
    /// Gets computes and retrieves this sequence's next value.
    /// </summary>
    /// <remarks>
    /// When called, the value of <see cref="CurrentValue"/> is updated.
    /// </remarks>
    public uint NextValue => ++CurrentValue;

    /// <summary>
    /// Sets the current value to the specified value.
    /// Subsequent call to <see cref="NextValue"/> will return this <paramref name="value"/> + 1.
    /// </summary>
    /// <param name="value">The new sequence current value.</param>
    public void SetCurrentValue(uint value) => CurrentValue = value;
}
