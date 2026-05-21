// -----------------------------------------------------------------------
// <copyright file="Faker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using MyNet.Fakers.Contacts;
using MyNet.Fakers.Geography;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Internet;
using MyNet.Fakers.Media;
using MyNet.Fakers.Text;
using MyNet.Globalization.Localization.Providers;
using MyNet.Utilities.Generator.Static;
using MyNet.Utilities.Text.Randomize;

namespace MyNet.Fakers.Static;

/// <summary>
/// Provides a static class for generating fake data across various categories such as names, phones, mails, countries, addresses, streets, identities, texts, domains, and colors. The <see cref="Faker"/> class serves as a centralized entry point for accessing different types of fake data generators, allowing users to easily generate realistic test data for their applications. The class is designed to be configurable, enabling users to replace the default implementations of the fake data generators with custom implementations if needed. This flexibility allows for greater control over the generated data and can be particularly useful in scenarios where specific formats or rules are required for the fake data.
/// </summary>
public static class Faker
{
    private static IFakeDataGenerator _service = CreateDefault();

    private static int _configured;

    /// <summary>
    /// Configures the <see cref="Faker"/> class to use a custom implementation of the <see cref="IFakeDataGenerator"/>. This method allows users to replace the default fake data generator with their own implementation, providing greater flexibility and control over the generated data. Once configured, the <see cref="Faker"/> class will use the provided generator for all subsequent calls to its properties (e.g., <see cref="Names"/>, <see cref="Phones"/>, etc.). It is important to note that this method can only be called once, and any subsequent attempts to configure the faker will result in an exception being thrown. This ensures that the configuration remains consistent throughout the application's lifecycle and prevents unintended changes to the fake data generation process.
    /// </summary>
    /// <param name="generator">The custom implementation of the <see cref="IFakeDataGenerator"/> to be used by the <see cref="Faker"/> class.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="generator"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="Faker"/> class has already been configured.</exception>
    public static void Configure(IFakeDataGenerator generator)
    {
        ArgumentNullException.ThrowIfNull(generator);

        if (Interlocked.Exchange(ref _configured, 1) == 1)
            throw new InvalidOperationException("Faker has already been configured.");

        _service = generator;
    }

    /// <summary>
    /// Gets the current instance of the <see cref="IFakeDataGenerator"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the fake data generator, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static INameFaker Names => _service.Names;

    /// <summary>
    /// Gets the current instance of the <see cref="IPhoneFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the phone faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static IPhoneFaker Phones => _service.Phones;

    /// <summary>
    /// Gets the current instance of the <see cref="IMailFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the mail faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static IMailFaker Mails => _service.Mails;

    /// <summary>
    /// Gets the current instance of the <see cref="ICountryFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the country faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static ICountryFaker Countries => _service.Countries;

    /// <summary>
    /// Gets the current instance of the <see cref="IAddressFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the address faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static IAddressFaker Addresses => _service.Addresses;

    /// <summary>
    /// Gets the current instance of the <see cref="IStreetFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the street faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static IStreetFaker Streets => _service.Streets;

    /// <summary>
    /// Gets the current instance of the <see cref="IIdentityFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the identity faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static IIdentityFaker Identities => _service.Identities;

    /// <summary>
    /// Gets the current instance of the <see cref="ITextFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the text faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static ITextFaker Texts => _service.Texts;

    /// <summary>
    /// Gets the current instance of the <see cref="IDomainFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the domain faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static IDomainFaker Domains => _service.Domains;

    /// <summary>
    /// Gets the current instance of the <see cref="IColorFaker"/> being used by the <see cref="Faker"/> class. This instance is initialized with a default implementation of the color faker, but can be replaced by calling the <see cref="Configure"/> method with a custom implementation.
    /// </summary>
    public static IColorFaker Colors => _service.Colors;

    private static FakeDataGenerator CreateDefault()
    {
        var nameFaker = new NameFaker(RandomGenerator.Current, new CultureScopedServiceSource<INameFakerProvider>(ResxNameFakerProvider.Create(CultureInfo.InvariantCulture)));
        var phoneFaker = new PhoneFaker(TextRandomGenerator.Current, RandomGenerator.Current, new CultureScopedServiceSource<IPhoneFakerProvider>(ResxPhoneFakerProvider.Create(CultureInfo.InvariantCulture)));
        var countryFaker = new CountryFaker(RandomGenerator.Current);
        var textFaker = new TextFaker(RandomGenerator.Current);
        var streetFaker = new StreetFaker(RandomGenerator.Current, new CultureScopedServiceSource<IAddressFakerProvider>(ResxAddressFakerProvider.Create(CultureInfo.InvariantCulture)));
        var addressFaker = new AddressFaker(TextRandomGenerator.Current, RandomGenerator.Current, new CultureScopedServiceSource<IAddressFakerProvider>(ResxAddressFakerProvider.Create(CultureInfo.InvariantCulture)), streetFaker, countryFaker);
        var identityFaker = new IdentityFaker(RandomGenerator.Current, textFaker);
        var domainFaker = new DomainFaker(RandomGenerator.Current, new CultureScopedServiceSource<IDomainFakerProvider>(ResxDomainFakerProvider.Create(CultureInfo.InvariantCulture)));
        var colorFaker = new ColorFaker(RandomGenerator.Current);
        var mailFaker = new MailFaker(RandomGenerator.Current, identityFaker, domainFaker);

        return new(
            nameFaker,
            phoneFaker,
            mailFaker,
            countryFaker,
            addressFaker,
            streetFaker,
            identityFaker,
            textFaker,
            domainFaker,
            colorFaker);
    }

    /// <summary>
    /// A simple implementation of the <see cref="ICultureScopedServiceSource{TService}"/> interface that always returns the same instance of the service, regardless of the culture specified. This implementation is useful for services that do not have culture-specific variations and can be shared across different cultures without any issues. By using this class, you can easily provide a single instance of a service to the <see cref="Faker"/> class without having to worry about culture-specific logic or variations in the service implementation.
    /// </summary>
    /// <param name="instance">The instance of the service to be returned.</param>
    /// <typeparam name="TService">The type of the service.</typeparam>
    private sealed class CultureScopedServiceSource<TService>(TService instance) : ICultureScopedServiceSource<TService>
        where TService : class, ICultureScoped
    {
        /// <inheritdoc/>
        public TService Get(CultureInfo? culture = null) => instance;

        /// <inheritdoc/>
        public bool TryGet(CultureInfo? culture, [NotNullWhen(true)] out TService? service)
        {
            service = instance;
            return true;
        }
    }
}
