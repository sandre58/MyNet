// -----------------------------------------------------------------------
// <copyright file="TextFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using MyNet.Generator;

namespace MyNet.Fakers.Text;

/// <summary>
/// Current implementation of <see cref="ITextFaker"/>.
/// </summary>
public sealed class TextFaker(IRandomGenerator random) : ITextFaker
{
    private const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed non risus. Suspendisse lectus tortor, dignissim sit amet, adipiscing nec, ultricies sed, dolor. Cras elementum ultrices diam. Maecenas ligula massa, varius a, semper congue, euismod non, mi. Proin porttitor, orci nec nonummy molestie, enim est eleifend mi, non fermentum diam nisl sit amet erat. Duis semper. Duis arcu massa, scelerisque vitae, consequat in, pretium a, enim. Pellentesque congue. Ut in risus volutpat libero pharetra tempor. Cras vestibulum bibendum augue. Praesent egestas leo in pede. Praesent blandit odio eu enim. Pellentesque sed dui ut augue blandit sodales. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Aliquam nibh. Mauris ac mauris sed pede pellentesque fermentum. Maecenas adipiscing ante non diam sodales hendrerit.\nUt velit mauris, egestas sed, gravida nec, ornare ut, mi. Aenean ut orci vel massa suscipit pulvinar. Nulla sollicitudin. Fusce varius, ligula non tempus aliquam, nunc turpis ullamcorper nibh, in tempus sapien eros vitae ligula. Pellentesque rhoncus nunc et augue. Integer id felis. Curabitur aliquet pellentesque diam. Integer quis metus vitae elit lobortis egestas. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Morbi vel erat non mauris convallis vehicula. Nulla et sapien. Integer tortor tellus, aliquam faucibus, convallis id, congue eu, quam. Mauris ullamcorper felis vitae erat. Proin feugiat, augue non elementum posuere, metus purus iaculis lectus, et tristique ligula justo vitae magna.\n\nAliquam convallis sollicitudin purus. Praesent aliquam, enim at fermentum mollis, ligula massa adipiscing nisl, ac euismod nibh nisl eu lectus. Fusce vulputate sem at sapien. Vivamus leo. Aliquam euismod libero eu enim. Nulla nec felis sed leo placerat imperdiet. Aenean suscipit nulla in justo. Suspendisse cursus rutrum augue. Nulla tincidunt tincidunt mi. Curabitur iaculis, lorem vel rhoncus faucibus, felis magna fermentum augue, et ultricies lacus lorem varius purus. Curabitur eu amet.";

    private static readonly string[] LoremWords = BuildUniqueWords();

    private static string[] BuildUniqueWords()
    {
        var cleaned = LoremIpsum
            .Replace(",", string.Empty, StringComparison.Ordinal)
            .Replace(".", string.Empty, StringComparison.Ordinal);

        var words = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.Ordinal).ToList();

        return [..words];
    }

    /// <inheritdoc />
    public string Word(int? length = null)
    {
        var word = random.Item(LoremWords);

        return length is null ? word : word.Length > length ? word[..length.Value] : word;
    }

    /// <inheritdoc />
    public string Words(int min, int max)
    {
        var count = random.Int(min, max);
        var builder = new StringBuilder();

        for (var i = 0; i < count; i++)
        {
            if (i > 0)
                builder.Append(' ');
            builder.Append(Word());
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public string Sentence(int minWords = 4, int maxWords = 12)
    {
        var words = Words(minWords, maxWords);

        return words.Length == 0 ? "." : char.ToUpper(words[0], CultureInfo.CurrentCulture) + words[1..] + ".";
    }

    /// <inheritdoc />
    public string Paragraph(int minSentences = 2, int maxSentences = 5)
    {
        var count = random.Int(minSentences, maxSentences);
        var builder = new StringBuilder();

        for (var i = 0; i < count; i++)
        {
            if (i > 0)
                builder.Append(' ');
            builder.Append(Sentence());
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public string Lorem(int? wordCount = null)
    {
        if (!wordCount.HasValue)
            return LoremIpsum;

        var words = LoremIpsum.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var builder = new StringBuilder();
        var count = Math.Min(wordCount.Value, words.Length);

        for (var i = 0; i < count; i++)
        {
            if (i > 0)
                builder.Append(' ');
            builder.Append(words[i]);
        }

        return builder.ToString();
    }
}
