// -----------------------------------------------------------------------
// <copyright file="InflectorExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyNet.Globalization.Facade;
using MyNet.Text;
using Xunit;

namespace MyNet.Globalization.Tests.Extensions;

[Collection("UseCultureSequential")]
public class InflectorExtensionsTests
{
    [Theory]
    [UseCulture("en")]
    [ClassData(typeof(EnglishPluralTestSource))]
    public void Pluralize(string singular, string plural) => Assert.Equal(plural, singular.Pluralize());

    [Theory]
    [UseCulture("en")]
    [ClassData(typeof(EnglishPluralTestSource))]
    public void PluralizeWordsWithUnknownPlurality(string singular, string plural)
    {
        Assert.Equal(plural, plural.Pluralize(false));
        Assert.Equal(plural, singular.Pluralize(false));
    }

    [Theory]
    [UseCulture("en")]
    [ClassData(typeof(EnglishPluralTestSource))]
    public void Singularize(string singular, string plural) => Assert.Equal(singular, plural.Singularize());

    [Theory]
    [UseCulture("en")]
    [ClassData(typeof(EnglishPluralTestSource))]
    public void SingularizeWordsWithUnknownSingularity(string singular, string plural)
    {
        Assert.Equal(singular, singular.Singularize(false));
        Assert.Equal(singular, plural.Singularize(false));
    }

    [Theory]
    [UseCulture("fr-FR")]
    [ClassData(typeof(FrenchPluralTestSource))]
    public void PluralizeFrench(string singular, string plural) => Assert.Equal(plural, singular.Pluralize());

    [Theory]
    [UseCulture("fr-FR")]
    [ClassData(typeof(FrenchPluralTestSource))]
    [SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "Culture changes")]
    public void PluralizeWordsWithUnknownPluralityFrench(string singular, string plural)
    {
        Assert.Equal(plural, plural.Pluralize(false));
        Assert.Equal(plural, singular.Pluralize(false));
    }

    [Theory]
    [UseCulture("fr-FR")]
    [ClassData(typeof(FrenchPluralTestSource))]
    public void SingularizeFrench(string singular, string plural) => Assert.Equal(singular, plural.Singularize());

    [Theory]
    [UseCulture("fr-FR")]
    [ClassData(typeof(FrenchPluralTestSource))]
    [SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "Culture changes")]
    public void SingularizeWordsWithUnknownSingularityFrench(string singular, string plural)
    {
        Assert.Equal(singular, singular.Singularize(false));
        Assert.Equal(singular, plural.Singularize(false));
    }

    [Theory]
    [UseCulture("en")]
    [InlineData("tire", "tires")]
    [InlineData("body", "bodies")]
    [InlineData("traxxa", "traxxas")]
    public void SingularizeSkipSimpleWords(string singular, string plural) => Assert.Equal(singular, plural.Singularize(skipSimpleWords: true));

    // Uppercases individual words and removes some characters
    [Theory]
    [InlineData("some title", "Some Title")]
    [InlineData("some-title", "Some-Title")]
    [InlineData("sometitle", "Sometitle")]
    [InlineData("some-title: The begining", "Some-Title: The Begining")]
    [InlineData("some_title:_the_begining", "Some_Title:_The_Begining")]
    [InlineData("some title: The_begining", "Some Title: The_Begining")]
    public void Titleize(string input, string expectedOuput) => Assert.Equal(expectedOuput, input.ToTitleCase());

    [Theory]
    [InlineData("customer", "Customer")]
    [InlineData("CUSTOMER", "CUSTOMER")]
    [InlineData("CUStomer", "CUStomer")]
    [InlineData("customer_name", "CustomerName")]
    [InlineData("customer_first_name", "CustomerFirstName")]
    [InlineData("customer_first_name goes here", "CustomerFirstNameGoesHere")]
    [InlineData("customer name", "CustomerName")]
    [InlineData("customer   name", "CustomerName")]
    public void Pascalize(string input, string expectedOutput) => Assert.Equal(expectedOutput, input.ToPascalCase());

    // Same as pascalize, except first char is lowercase
    [Theory]
    [InlineData("customer", "customer")]
    [InlineData("CUSTOMER", "cUSTOMER")]
    [InlineData("CUStomer", "cUStomer")]
    [InlineData("customer_name", "customerName")]
    [InlineData("customer_first_name", "customerFirstName")]
    [InlineData("customer_first_name goes here", "customerFirstNameGoesHere")]
    [InlineData("customer name", "customerName")]
    [InlineData("customer   name", "customerName")]
    [InlineData("", "")]
    public void Camelize(string input, string expectedOutput) => Assert.Equal(expectedOutput, input.ToCamelCase());

    // transform words into lowercase and separate with a -
    [Theory]
    [InlineData("SomeWords", "some-words")]
    [InlineData("SOME words TOGETHER", "some-words-together")]
    [InlineData("A spanish word EL niño", "a-spanish-word-el-niño")]
    [InlineData("SomeForeignWords ÆgÑuÄgypten", "some-foreign-words-æg-ñu-ägypten")]
    [InlineData("A VeryShortSENTENCE", "a-very-short-sentence")]
    public void Kebaberize(string input, string expectedOutput) => Assert.Equal(expectedOutput, input.ToKebabCase());
}

internal sealed class EnglishPluralTestSource : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["search", "searches"];
        yield return ["switch", "switches"];
        yield return ["fix", "fixes"];
        yield return ["box", "boxes"];
        yield return ["process", "processes"];
        yield return ["address", "addresses"];
        yield return ["case", "cases"];
        yield return ["stack", "stacks"];
        yield return ["wish", "wishes"];
        yield return ["fish", "fish"];

        yield return ["category", "categories"];
        yield return ["query", "queries"];
        yield return ["ability", "abilities"];
        yield return ["agency", "agencies"];
        yield return ["movie", "movies"];

        yield return ["archive", "archives"];

        yield return ["index", "indices"];

        yield return ["wife", "wives"];
        yield return ["safe", "saves"];
        yield return ["half", "halves"];

        yield return ["move", "moves"];

        yield return ["salesperson", "salespeople"];
        yield return ["person", "people"];

        yield return ["spokesman", "spokesmen"];
        yield return ["man", "men"];
        yield return ["woman", "women"];
        yield return ["freshman", "freshmen"];
        yield return ["chairman", "chairmen"];
        yield return ["human", "humans"];
        yield return ["personnel", "personnel"];
        yield return ["staff", "staff"];

        yield return ["basis", "bases"];
        yield return ["diagnosis", "diagnoses"];

        yield return ["data", "data"];
        yield return ["medium", "media"];
        yield return ["analysis", "analyses"];

        yield return ["node_child", "node_children"];
        yield return ["child", "children"];

        yield return ["experience", "experiences"];
        yield return ["day", "days"];

        yield return ["comment", "comments"];
        yield return ["foobar", "foobars"];
        yield return ["newsletter", "newsletters"];

        yield return ["old_news", "old_news"];
        yield return ["news", "news"];

        yield return ["series", "series"];
        yield return ["species", "species"];

        yield return ["quiz", "quizzes"];

        yield return ["perspective", "perspectives"];

        yield return ["ox", "oxen"];
        yield return ["photo", "photos"];
        yield return ["buffalo", "buffaloes"];
        yield return ["tomato", "tomatoes"];
        yield return ["dwarf", "dwarves"];
        yield return ["elf", "elves"];
        yield return ["information", "information"];
        yield return ["equipment", "equipment"];
        yield return ["bus", "buses"];
        yield return ["status", "statuses"];
        yield return ["status_code", "status_codes"];
        yield return ["mouse", "mice"];

        yield return ["house", "houses"];
        yield return ["octopus", "octopi"];
        yield return ["alias", "aliases"];
        yield return ["portfolio", "portfolios"];
        yield return ["criterion", "criteria"];

        yield return ["vertex", "vertices"];
        yield return ["matrix", "matrices"];

        yield return ["axis", "axes"];
        yield return ["testis", "testes"];
        yield return ["crisis", "crises"];

        yield return ["corn", "corn"];
        yield return ["milk", "milk"];
        yield return ["rice", "rice"];
        yield return ["shoe", "shoes"];

        yield return ["horse", "horses"];
        yield return ["prize", "prizes"];
        yield return ["edge", "edges"];

        yield return ["goose", "geese"];
        yield return ["deer", "deer"];
        yield return ["sheep", "sheep"];
        yield return ["wolf", "wolves"];
        yield return ["volcano", "volcanoes"];
        yield return ["aircraft", "aircraft"];
        yield return ["alumna", "alumnae"];
        yield return ["alumnus", "alumni"];
        yield return ["fungus", "fungi"];
        yield return ["water", "water"];
        yield return ["waters", "waters"];
        yield return ["semen", "semen"];
        yield return ["sperm", "sperm"];

        yield return ["wave", "waves"];

        yield return ["campus", "campuses"];

        yield return ["is", "are"];

        yield return ["addendum", "addenda"];
        yield return ["alga", "algae"];
        yield return ["apparatus", "apparatuses"];
        yield return ["appendix", "appendices"];
        yield return ["bias", "biases"];
        yield return ["bison", "bison"];
        yield return ["blitz", "blitzes"];
        yield return ["buzz", "buzzes"];
        yield return ["cactus", "cacti"];
        yield return ["corps", "corps"];
        yield return ["curriculum", "curricula"];
        yield return ["database", "databases"];
        yield return ["die", "dice"];
        yield return ["echo", "echoes"];
        yield return ["ellipsis", "ellipses"];
        yield return ["elk", "elk"];
        yield return ["emphasis", "emphases"];
        yield return ["embargo", "embargoes"];
        yield return ["focus", "foci"];
        yield return ["foot", "feet"];
        yield return ["fuse", "fuses"];
        yield return ["grass", "grass"];
        yield return ["hair", "hair"];
        yield return ["hero", "heroes"];
        yield return ["hippopotamus", "hippopotami"];
        yield return ["hoof", "hooves"];
        yield return ["iris", "irises"];
        yield return ["larva", "larvae"];
        yield return ["leaf", "leaves"];
        yield return ["loaf", "loaves"];
        yield return ["luggage", "luggage"];
        yield return ["means", "means"];
        yield return ["mail", "mail"];
        yield return ["millennium", "millennia"];
        yield return ["moose", "moose"];
        yield return ["mosquito", "mosquitoes"];
        yield return ["mud", "mud"];
        yield return ["nucleus", "nuclei"];
        yield return ["neurosis", "neuroses"];
        yield return ["oasis", "oases"];
        yield return ["offspring", "offspring"];
        yield return ["paralysis", "paralyses"];
        yield return ["phenomenon", "phenomena"];
        yield return ["potato", "potatoes"];
        yield return ["radius", "radii"];
        yield return ["salmon", "salmon"];
        yield return ["scissors", "scissors"];
        yield return ["shrimp", "shrimp"];
        yield return ["someone", "someone"];
        yield return ["stimulus", "stimuli"];
        yield return ["swine", "swine"];
        yield return ["syllabus", "syllabi"];
        yield return ["that", "those"];
        yield return ["thief", "thieves"];
        yield return ["this", "these"];
        yield return ["tooth", "teeth"];
        yield return ["torpedo", "torpedoes"];
        yield return ["trellis", "trellises"];
        yield return ["trout", "trout"];
        yield return ["tuna", "tuna"];
        yield return ["vertebra", "vertebrae"];
        yield return ["veto", "vetoes"];
        yield return ["virus", "viruses"];
        yield return ["walrus", "walruses"];
        yield return ["waltz", "waltzes"];
        yield return ["zombie", "zombies"];

        yield return ["cookie", "cookies"];
        yield return ["bookie", "bookies"];
        yield return ["rookie", "rookies"];
        yield return ["roomie", "roomies"];
        yield return ["smoothie", "smoothies"];

        yield return ["cache", "caches"];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class FrenchPluralTestSource : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["catégorie", "catégories"];
        yield return ["bateau", "bateaux"];
        yield return ["noyau", "noyaux"];
        yield return ["animal", "animaux"];
        yield return ["carnaval", "carnavals"];
        yield return ["portail", "portails"];
        yield return ["bail", "baux"];
        yield return ["genou", "genoux"];
        yield return ["cou", "cous"];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
