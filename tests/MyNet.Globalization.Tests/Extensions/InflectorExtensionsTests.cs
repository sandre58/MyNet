// -----------------------------------------------------------------------
// <copyright file="InflectorExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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

internal sealed class EnglishPluralTestSource : TheoryData<string, string>
{
    public EnglishPluralTestSource()
    {
        Add("search", "searches");
        Add("switch", "switches");
        Add("fix", "fixes");
        Add("box", "boxes");
        Add("process", "processes");
        Add("address", "addresses");
        Add("case", "cases");
        Add("stack", "stacks");
        Add("wish", "wishes");
        Add("fish", "fish");

        Add("category", "categories");
        Add("query", "queries");
        Add("ability", "abilities");
        Add("agency", "agencies");
        Add("movie", "movies");

        Add("archive", "archives");

        Add("index", "indices");

        Add("wife", "wives");
        Add("safe", "saves");
        Add("half", "halves");

        Add("move", "moves");

        Add("salesperson", "salespeople");
        Add("person", "people");

        Add("spokesman", "spokesmen");
        Add("man", "men");
        Add("woman", "women");
        Add("freshman", "freshmen");
        Add("chairman", "chairmen");
        Add("human", "humans");
        Add("personnel", "personnel");
        Add("staff", "staff");

        Add("basis", "bases");
        Add("diagnosis", "diagnoses");

        Add("data", "data");
        Add("medium", "media");
        Add("analysis", "analyses");

        Add("node_child", "node_children");
        Add("child", "children");

        Add("experience", "experiences");
        Add("day", "days");

        Add("comment", "comments");
        Add("foobar", "foobars");
        Add("newsletter", "newsletters");

        Add("old_news", "old_news");
        Add("news", "news");

        Add("series", "series");
        Add("species", "species");

        Add("quiz", "quizzes");

        Add("perspective", "perspectives");

        Add("ox", "oxen");
        Add("photo", "photos");
        Add("buffalo", "buffaloes");
        Add("tomato", "tomatoes");
        Add("dwarf", "dwarves");
        Add("elf", "elves");
        Add("information", "information");
        Add("equipment", "equipment");
        Add("bus", "buses");
        Add("status", "statuses");
        Add("status_code", "status_codes");
        Add("mouse", "mice");

        Add("house", "houses");
        Add("octopus", "octopi");
        Add("alias", "aliases");
        Add("portfolio", "portfolios");
        Add("criterion", "criteria");

        Add("vertex", "vertices");
        Add("matrix", "matrices");

        Add("axis", "axes");
        Add("testis", "testes");
        Add("crisis", "crises");

        Add("corn", "corn");
        Add("milk", "milk");
        Add("rice", "rice");
        Add("shoe", "shoes");

        Add("horse", "horses");
        Add("prize", "prizes");
        Add("edge", "edges");

        Add("goose", "geese");
        Add("deer", "deer");
        Add("sheep", "sheep");
        Add("wolf", "wolves");
        Add("volcano", "volcanoes");
        Add("aircraft", "aircraft");
        Add("alumna", "alumnae");
        Add("alumnus", "alumni");
        Add("fungus", "fungi");
        Add("water", "water");
        Add("waters", "waters");
        Add("semen", "semen");
        Add("sperm", "sperm");

        Add("wave", "waves");

        Add("campus", "campuses");

        Add("is", "are");

        Add("addendum", "addenda");
        Add("alga", "algae");
        Add("apparatus", "apparatuses");
        Add("appendix", "appendices");
        Add("bias", "biases");
        Add("bison", "bison");
        Add("blitz", "blitzes");
        Add("buzz", "buzzes");
        Add("cactus", "cacti");
        Add("corps", "corps");
        Add("curriculum", "curricula");
        Add("database", "databases");
        Add("die", "dice");
        Add("echo", "echoes");
        Add("ellipsis", "ellipses");
        Add("elk", "elk");
        Add("emphasis", "emphases");
        Add("embargo", "embargoes");
        Add("focus", "foci");
        Add("foot", "feet");
        Add("fuse", "fuses");
        Add("grass", "grass");
        Add("hair", "hair");
        Add("hero", "heroes");
        Add("hippopotamus", "hippopotami");
        Add("hoof", "hooves");
        Add("iris", "irises");
        Add("larva", "larvae");
        Add("leaf", "leaves");
        Add("loaf", "loaves");
        Add("luggage", "luggage");
        Add("means", "means");
        Add("mail", "mail");
        Add("millennium", "millennia");
        Add("moose", "moose");
        Add("mosquito", "mosquitoes");
        Add("mud", "mud");
        Add("nucleus", "nuclei");
        Add("neurosis", "neuroses");
        Add("oasis", "oases");
        Add("offspring", "offspring");
        Add("paralysis", "paralyses");
        Add("phenomenon", "phenomena");
        Add("potato", "potatoes");
        Add("radius", "radii");
        Add("salmon", "salmon");
        Add("scissors", "scissors");
        Add("shrimp", "shrimp");
        Add("someone", "someone");
        Add("stimulus", "stimuli");
        Add("swine", "swine");
        Add("syllabus", "syllabi");
        Add("that", "those");
        Add("thief", "thieves");
        Add("this", "these");
        Add("tooth", "teeth");
        Add("torpedo", "torpedoes");
        Add("trellis", "trellises");
        Add("trout", "trout");
        Add("tuna", "tuna");
        Add("vertebra", "vertebrae");
        Add("veto", "vetoes");
        Add("virus", "viruses");
        Add("walrus", "walruses");
        Add("waltz", "waltzes");
        Add("zombie", "zombies");

        Add("cookie", "cookies");
        Add("bookie", "bookies");
        Add("rookie", "rookies");
        Add("roomie", "roomies");
        Add("smoothie", "smoothies");

        Add("cache", "caches");
    }
}

internal sealed class FrenchPluralTestSource : TheoryData<string, string>
{
    public FrenchPluralTestSource()
    {
        Add("catégorie", "catégories");
        Add("bateau", "bateaux");
        Add("noyau", "noyaux");
        Add("animal", "animaux");
        Add("carnaval", "carnavals");
        Add("portail", "portails");
        Add("bail", "baux");
        Add("genou", "genoux");
        Add("cou", "cous");
    }
}
