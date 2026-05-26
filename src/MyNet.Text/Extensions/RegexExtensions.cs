// -----------------------------------------------------------------------
// <copyright file="RegexExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Text;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class RegexExtensions
{
    extension(string? value)
    {
        /// <summary>
        /// Validates a string against the specified regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to match against.</param>
        /// <returns><c>true</c> if the string matches the pattern; otherwise, <c>false</c>.</returns>
        public bool Matches(Regex pattern)
        {
            ArgumentNullException.ThrowIfNull(pattern);

            return !string.IsNullOrEmpty(value) && pattern.Match(value).Length > 0;
        }

        /// <summary>
        /// Determines whether the string is a valid email address. The method validates against a precompiled email pattern regex.
        /// </summary>
        /// <returns><c>true</c> if the string is a valid email address; otherwise, <c>false</c>.</returns>
        public bool IsEmailAddress() => value.Matches(RegexPatterns.EmailPattern);

        /// <summary>
        /// Determines whether the string is a valid phone number. The method validates against a precompiled phone pattern regex.
        /// </summary>
        /// <returns><c>true</c> if the string is a valid phone number; otherwise, <c>false</c>.</returns>
        public bool IsPhoneNumber() => value.Matches(RegexPatterns.PhonePattern);
    }
}
