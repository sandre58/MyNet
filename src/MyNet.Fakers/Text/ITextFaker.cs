// -----------------------------------------------------------------------
// <copyright file="ITextFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Fakers.Text;

/// <summary>
/// Interface for generating fake text data, such as words, sentences, paragraphs, and alphanumeric strings.
/// </summary>
public interface ITextFaker
{
    /// <summary>
    /// Generates a random alphanumeric string of a specified length. If no length is provided, it generates a string of random length.
    /// </summary>
    /// <param name="length">The length of the string. If null, a random length will be used.</param>
    /// <returns>A random alphanumeric string.</returns>
    string Word(int? length = null);

    /// <summary>
    /// Generates a random string consisting of a specified number of words, with an optional range for the number of words.
    /// </summary>
    /// <param name="min">The minimum number of words.</param>
    /// <param name="max">The maximum number of words.</param>
    /// <returns>A random string of words.</returns>
    string Words(int min, int max);

    /// <summary>
    /// Generates a random sentence with a specified range for the number of words.
    /// </summary>
    /// <param name="minWords">The minimum number of words in the sentence.</param>
    /// <param name="maxWords">The maximum number of words in the sentence.</param>
    /// <returns>A random sentence.</returns>
    string Sentence(int minWords, int maxWords);

    /// <summary>
    /// Generates a random paragraph with a specified range for the number of sentences.
    /// </summary>
    /// <param name="minSentences">The minimum number of sentences in the paragraph.</param>
    /// <param name="maxSentences">The maximum number of sentences in the paragraph.</param>
    /// <returns>A random paragraph.</returns>
    string Paragraph(int minSentences, int maxSentences);

    /// <summary>
    /// Generates a random string consisting of a specified number of words, without any punctuation. This can be used for generating placeholder text or "lorem ipsum" style content.
    /// </summary>
    /// <param name="wordCount">The number of words to generate.</param>
    /// <returns>A random string of words without punctuation.</returns>
    string Lorem(int? wordCount = null);
}
