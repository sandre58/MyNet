// -----------------------------------------------------------------------
// <copyright file="IEncryptionService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Encryption;

/// <summary>
/// Defines simple text encryption and decryption operations.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the specified text and returns a base64-encoded representation of the encrypted payload.
    /// </summary>
    /// <param name="text">The text to encrypt.</param>
    /// <returns>A base64 string representing the encrypted payload.</returns>
    string Encrypt(string? text);

    /// <summary>
    /// Decrypts the specified base64-encoded encrypted payload and returns the original text.
    /// </summary>
    /// <param name="text">A base64 string produced by <see cref="Encrypt(string?)"/>.</param>
    /// <returns>The decrypted text.</returns>
    string Decrypt(string? text);
}
