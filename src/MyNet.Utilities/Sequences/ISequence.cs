// -----------------------------------------------------------------------
// <copyright file="ISequence.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Sequences;

/// <summary>
/// Represents a sequence of values that can be generated or iterated over. This interface defines the contract for a sequence, allowing for the retrieval of the next value in the sequence as well as the current value. Implementations of this interface can represent various types of sequences, such as numeric sequences, date sequences, or any other type of ordered values. The <see cref="NextValue"/> property is used to advance the sequence and retrieve the next value, while the <see cref="CurrentValue"/> property allows access to the current value without advancing the sequence. This design enables both forward iteration and access to the current state of the sequence, making it versatile for different use cases where ordered data generation is required.
/// </summary>
/// <typeparam name="T">The type of values in the sequence.</typeparam>
public interface ISequence<out T>
{
    /// <summary>
    /// Gets the next value in the sequence and advances the sequence.
    /// </summary>
    T NextValue { get; }

    /// <summary>
    /// Gets the current value in the sequence without advancing the sequence.
    /// </summary>
    T CurrentValue { get; }
}
