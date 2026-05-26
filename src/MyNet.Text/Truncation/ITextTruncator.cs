// -----------------------------------------------------------------------
// <copyright file="ITextTruncator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Text.Truncation;

/// <summary>
/// Can truncate a string to a fixed length, number of characters or number of words.
/// </summary>
public interface ITextTruncator : ITextTransform;
