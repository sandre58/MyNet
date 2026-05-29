# Fakers

**Package:** [MyNet.Fakers](../../src/MyNet.Fakers/README.md)

Locale-aware **fake data** for unit tests, benchmarks, and design-time data: names, addresses, phones, emails, domains, colors, and text.

Depends on **MyNet.Globalization**, **MyNet.Generator**, **MyNet.Geography**, **MyNet.Text**, **MyNet.Primitives**.

## Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Fakers;
using MyNet.Globalization.Extensions;

var services = new ServiceCollection();
services.AddGlobalization();
services.AddFakers();

var provider = services.BuildServiceProvider();
provider.UseFakers(); // wires static Faker class to DI
```

`AddFakers()` also calls `AddLocalization()` and `AddRandomGenerator()` if not already registered.

## Static API (`Faker`)

After `UseFakers()`:

```csharp
using MyNet.Fakers.Static;
using MyNet.Geography;

var person = Faker.Names.FullName();
var email = Faker.Mails.Email();
var phone = Faker.Phones.PhoneNumber();
var country = Faker.Countries.Country(); // Country SmartEnum instance
var address = Faker.Addresses.Address();   // MyNet.Geography.Address
var domain = Faker.Domains.Domain();
var color = Faker.Colors.HexColor();
var lorem = Faker.Texts.Sentence();
```

Culture follows the active localization culture (set via `ICultureService` / thread culture).

## Injectable facade (`IFakeDataGenerator`)

```csharp
public class UserFactory(IFakeDataGenerator faker)
{
    public TestUser Create() => new(
        faker.Names.FullName(),
        faker.Mails.Email(),
        faker.Addresses.Address());
}
```

Same surface as static `Faker` properties: `Names`, `Mails`, `Phones`, `Countries`, `Addresses`, `Streets`, `Identities`, `Texts`, `Domains`, `Colors`.

## Generator breakdown

| Interface | Examples |
|-----------|----------|
| `INameFaker` | `FullName()`, `FirstName()`, `LastName()` |
| `IIdentityFaker` | User identity bundles |
| `IMailFaker` | `Email()`, `UserName()` |
| `IPhoneFaker` | `PhoneNumber()` |
| `ICountryFaker` | `Country()`, `Alpha2()` |
| `IStreetFaker` | `Street(culture)` |
| `IAddressFaker` | `Address()` — full `Address` record |
| `IDomainFaker` | `Domain()`, `Url()` |
| `IColorFaker` | `HexColor()`, `RgbColor()` |
| `ITextFaker` | `Sentence()`, `Paragraph()` |

Data is backed by **embedded .resx** providers (`ResxNameFakerProvider`, `ResxAddressFakerProvider`, …) per culture.

## Culture in tests

```csharp
using System.Globalization;
using MyNet.Fakers.Geography;
using MyNet.Generator;
using MyNet.Generator.Facade;

// Direct unit test without DI:
var random = RandomGenerator.Current; // or mock IRandomGenerator
var sut = new CountryFaker(random);
var country = sut.Country();
country.Alpha2.Should().HaveLength(2);
```

For localized strings, set culture before `Faker` calls:

```csharp
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
var name = Faker.Names.FullName();
```

## With Geography

```csharp
var address = Faker.Addresses.Address();
// Street, postal code, city, Country — suitable for Humanizer / UI tests

var fr = Faker.Countries.Country();
Assert.Same(Country.France, fr); // when random picks France
```

## Related

- [Geography](geography.md)
- [Generator / Foundations](foundations.md#mynetgenerator)
- [Globalization](globalization.md)
