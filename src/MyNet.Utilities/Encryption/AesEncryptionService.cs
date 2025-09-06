// -----------------------------------------------------------------------
// <copyright file="AesEncryptionService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace MyNet.Utilities.Encryption;

/// <summary>
/// Provides AES-GCM based encryption and decryption utilities using a pre-shared key.
/// </summary>
public class AesEncryptionService(byte[] key) : IEncryptionService
{
    private const int KeyBytes = 16;
    private const int NonceBytes = 12;

    /// <summary>
    /// Concatenates two byte arrays into a single array.
    /// </summary>
    /// <param name="a">First byte array.</param>
    /// <param name="b">Second byte array.</param>
    /// <returns>The concatenated result of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static byte[] Concat(byte[] a, byte[] b)
    {
        var output = new byte[a.Length + b.Length];

        for (var i = 0; i < a.Length; i++)
        {
            output[i] = a[i];
        }

        for (var j = 0; j < b.Length; j++)
        {
            output[a.Length + j] = b[j];
        }

        return output;
    }

    /// <summary>
    /// Returns a sub-array extracted from the provided data.
    /// </summary>
    /// <param name="data">Source array.</param>
    /// <param name="start">Starting index.</param>
    /// <param name="length">Number of bytes to copy.</param>
    /// <returns>A new array containing the requested segment.</returns>
    public static byte[] SubArray(byte[] data, int start, int length)
    {
        var result = new byte[length];

        Array.Copy(data, start, result, 0, length);

        return result;
    }

    /// <summary>
    /// Encrypts the provided bytes using AES-GCM and returns a combined payload containing tag, nonce and ciphertext.
    /// </summary>
    /// <param name="toEncrypt">Plain bytes to encrypt.</param>
    /// <returns>Combined tag + nonce + ciphertext.</returns>
    public byte[] Encrypt(byte[] toEncrypt)
    {
        var tag = new byte[KeyBytes];
        var nonce = new byte[NonceBytes];
        var cipherText = new byte[toEncrypt.Length];

        using var cipher = new AesGcm(key, KeyBytes);
        cipher.Encrypt(nonce, toEncrypt, cipherText, tag);

        return Concat(tag, Concat(nonce, cipherText));
    }

    /// <summary>
    /// Encrypts the provided string using UTF8 encoding and returns the result as a base64 string.
    /// </summary>
    /// <param name="text">The text to encrypt.</param>
    /// <returns>Base64-encoded encrypted payload.</returns>
    public string Encrypt(string? text) => Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(text ?? string.Empty)));

    /// <summary>
    /// Decrypts the combined payload produced by <see cref="Encrypt(byte[])"/> and returns the plaintext bytes.
    /// </summary>
    /// <param name="cipherText">Combined tag + nonce + ciphertext produced by <see cref="Encrypt(byte[])"/>.</param>
    /// <returns>The decrypted plaintext bytes.</returns>
    public byte[] Decrypt(byte[] cipherText)
    {
        var tag = SubArray(cipherText, 0, KeyBytes);
        var nonce = SubArray(cipherText, KeyBytes, NonceBytes);

        var toDecrypt = SubArray(cipherText, KeyBytes + NonceBytes, cipherText.Length - tag.Length - nonce.Length);
        var decryptedData = new byte[toDecrypt.Length];

        using var cipher = new AesGcm(key, KeyBytes);
        cipher.Decrypt(nonce, toDecrypt, tag, decryptedData);

        return decryptedData;
    }

    /// <summary>
    /// Decrypts the provided base64-encoded payload and returns the decrypted UTF8 string.
    /// </summary>
    /// <param name="text">Base64-encoded payload to decrypt.</param>
    /// <returns>Decrypted UTF8 string.</returns>
    public string Decrypt(string? text) => !string.IsNullOrEmpty(text) ? Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(text))).TrimEnd('\0') : string.Empty;
}
