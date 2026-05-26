// -----------------------------------------------------------------------
// <copyright file="BooleanExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class BooleanExtensions
{
    extension(bool value)
    {
        /// <summary>
        /// Executes the specified action if the boolean is true. If the boolean is false, the action is not executed.
        /// </summary>
        /// <param name="ifTrue">The action to execute if the boolean is true.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="ifTrue"/> action is null.</exception>
        public void IfTrue(Action ifTrue)
        {
            ArgumentNullException.ThrowIfNull(ifTrue);

            if (value)
                ifTrue();
        }

        /// <summary>
        /// Executes the specified action if the boolean is false. If the boolean is true, the action is not executed.
        /// </summary>
        /// <param name="ifFalse">The action to execute if the boolean is false.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="ifFalse"/> action is null.</exception>
        public void IfFalse(Action ifFalse)
        {
            ArgumentNullException.ThrowIfNull(ifFalse);

            if (!value)
                ifFalse();
        }
    }

    extension(bool? value)
    {
        /// <summary>
        /// Returns true if the nullable boolean has a value of true; otherwise, false. This method returns false if the nullable boolean is null or has a value of false.
        /// </summary>
        public bool IsTrue() => value == true;

        /// <summary>
        /// Returns true if the nullable boolean has a value of false; otherwise, false. This method returns false if the nullable boolean is null or has a value of true.
        /// </summary>
        public bool IsFalse() => value != true;

        /// <summary>
        /// Executes the specified action if the nullable boolean has a value of true. If the nullable boolean is null or has a value of false, the action is not executed.
        /// </summary>
        /// <param name="ifTrue">The action to execute if the nullable boolean is true.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="ifTrue"/> action is null.</exception>
        public void IfTrue(Action ifTrue)
        {
            ArgumentNullException.ThrowIfNull(ifTrue);

            if (value.IsTrue())
                ifTrue();
        }

        /// <summary>
        /// Executes the specified action if the nullable boolean has a value of false. If the nullable boolean is null or has a value of true, the action is not executed.
        /// </summary>
        /// <param name="ifFalse">The action to execute if the nullable boolean is false.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="ifFalse"/> action is null.</exception>
        public void IfFalse(Action ifFalse)
        {
            ArgumentNullException.ThrowIfNull(ifFalse);

            if (value.IsFalse())
                ifFalse();
        }
    }
}
