// -----------------------------------------------------------------------
// <copyright file="IFakeDataGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Fakers.Contacts;
using MyNet.Fakers.Geography;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Internet;
using MyNet.Fakers.Media;
using MyNet.Fakers.Text;

namespace MyNet.Fakers;

/// <summary>
/// Interface representing a faker that provides various types of fake data generators, such as names, addresses, and more. This interface serves as a contract for implementing classes to provide specific fake data generation functionalities.
/// </summary>
public interface IFakeDataGenerator
{
    /// <summary>
    /// Gets an instance of <see cref="INameFaker"/> that provides methods for generating fake names, including first names, last names, full names, and more. This property allows users to access name-related fake data generation functionalities through the faker interface.
    /// </summary>
    INameFaker Names { get; }

    /// <summary>
    /// Gets an instance of <see cref="IPhoneFaker"/> that provides methods for generating fake phone numbers, including national, mobile, and international formats. This property allows users to access phone-related fake data generation functionalities through the faker interface.
    /// </summary>
    IPhoneFaker Phones { get; }

    /// <summary>
    /// Gets an instance of <see cref="IMailFaker"/> that provides methods for generating fake email addresses, including various formats and domains. This property allows users to access email-related fake data generation functionalities through the faker interface.
    /// </summary>
    IMailFaker Mails { get; }

    /// <summary>
    /// Gets an instance of <see cref="ICountryFaker"/> that provides methods for generating fake country names, codes, and other related information. This property allows users to access country-related fake data generation functionalities through the faker interface.
    /// </summary>
    ICountryFaker Countries { get; }

    /// <summary>
    /// Gets an instance of <see cref="IAddressFaker"/> that provides methods for generating fake addresses, including street addresses, postal codes, and more. This property allows users to access address-related fake data generation functionalities through the faker interface.
    /// </summary>
    IAddressFaker Addresses { get; }

    /// <summary>
    /// Gets an instance of <see cref="IStreetFaker"/> that provides methods for generating fake street names and related information. This property allows users to access street-related fake data generation functionalities through the faker interface.
    /// </summary>
    IStreetFaker Streets { get; }

    /// <summary>
    /// Gets an instance of <see cref="IIdentityFaker"/> that provides methods for generating fake identities, including personal information and identifiers. This property allows users to access identity-related fake data generation functionalities through the faker interface.
    /// </summary>
    IIdentityFaker Identities { get; }

    /// <summary>
    /// Gets an instance of <see cref="ITextFaker"/> that provides methods for generating fake text, including sentences, paragraphs, and more. This property allows users to access text-related fake data generation functionalities through the faker interface.
    /// </summary>
    ITextFaker Texts { get; }

    /// <summary>
    /// Gets an instance of <see cref="IDomainFaker"/> that provides methods for generating fake domain names and related information. This property allows users to access domain-related fake data generation functionalities through the faker interface.
    /// </summary>
    IDomainFaker Domains { get; }

    /// <summary>
    /// Gets an instance of <see cref="IColorFaker"/> that provides methods for generating fake colors, including named colors and hexadecimal color codes. This property allows users to access color-related fake data generation functionalities through the faker interface.
    /// </summary>
    IColorFaker Colors { get; }
}
