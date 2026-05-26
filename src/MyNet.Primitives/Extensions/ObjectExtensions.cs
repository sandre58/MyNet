// -----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ObjectExtensions
{
    extension<T>(object value)
    {
        /// <summary>
        /// Casts the value to type T. If the value is not of type T, an <see cref="InvalidCastException"/> will be thrown.
        /// </summary>
        /// <returns>The value cast to type T.</returns>
        public T CastIn() => (T)value;
    }

    extension(object? value)
    {
        /// <summary>
        /// Executes the specified action if the value is null.
        /// </summary>
        /// <param name="ifNull">The action to execute if the value is null.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="ifNull"/> action is null.</exception>
        public void IfNull(Action ifNull)
        {
            ArgumentNullException.ThrowIfNull(ifNull);

            if (value is null)
                ifNull();
        }
    }

    extension<T>(object? value)
    {
        /// <summary>
        /// Executes the specified action if the value is of type T, passing the value cast to T as a parameter to the action. If the value is not of type T, the method does nothing.
        /// </summary>
        /// <param name="action">The action to execute if the value is of type T.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="action"/> is null.</exception>
        public void IfIs(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (value is T typed)
                action(typed);
        }
    }

    extension<T>(T? value)
    {
        /// <summary>
        /// Returns the value if it is not null; otherwise, throws an <see cref="ArgumentNullException"/> with the specified parameter name.
        /// </summary>
        /// <param name="paramName">The name of the parameter that is null.</param>
        /// <param name="message">The message to include in the exception.</param>
        /// <returns>The value if it is not null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
        public T OrThrow(string? paramName = null, string? message = null) => value ?? throw new ArgumentNullException(paramName, message);

        /// <summary>
        /// Returns the value if it is not null; otherwise, returns the specified fallback value.
        /// </summary>
        /// <param name="fallback">The fallback value to return if the value is null.</param>
        /// <returns>The value if it is not null; otherwise, the specified fallback value.</returns>
        public T Or(T fallback) => value ?? fallback;

        /// <summary>
        /// Executes the specified action if the value is not null, passing the non-null value as a parameter to the action.
        /// </summary>
        /// <param name="ifNotNull">The action to execute if the value is not null.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="ifNotNull"/> action is null.</exception>
        public void IfNotNull(Action<T> ifNotNull)
        {
            ArgumentNullException.ThrowIfNull(ifNotNull);

            if (value is not null)
                ifNotNull(value);
        }

        /// <summary>
        /// Converts the value using the provided selector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult? ConvertTo<TResult>(Func<T, TResult> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            return value is null ? default : selector(value);
        }
    }
}
