# Geography

Country data, addresses, flags, localized names, and Google Maps integration.

## Packages

| Package | Role |
|---------|------|
| [MyNet.Geography](../../src/MyNet.Geography/README.md) | `Country`, `Address`, `Coordinates`, `ICountryFlagProvider` contract |
| [MyNet.Geography.Resources](../../src/MyNet.Geography.Resources/README.md) | Embedded PNG flags + `EmbeddedCountryFlagProvider` |
| [MyNet.Geography.Localization](../../src/MyNet.Geography.Localization/README.md) | Localized country names |
| [MyNet.Google](../../src/MyNet.Google/README.md) | Geocoding, directions, map URLs |

## Recommended stack

```bash
dotnet add package MyNet.Geography
dotnet add package MyNet.Geography.Resources
dotnet add package MyNet.Geography.Localization   # optional
dotnet add package MyNet.Google                   # optional
```

---

## Countries (`Country` SmartEnum)

```csharp
using MyNet.Geography;

var france = Country.France;
var code = france.Alpha2;   // "fr"
var iso = france.Iso;       // numeric ISO 3166
var continent = france.Continent;

// Parse
Country.TryFromValue(france.Value, out var parsed);
Country.TryFromName("France", out var byName);
```

Hundreds of static `Country.*` members are generated from ISO 3166 data.

---

## Addresses

```csharp
var address = new Address(
    Street: "10 rue de Rivoli",
    PostalCode: "75001",
    City: "Paris",
    Country: Country.France);

var line = address.ToString();
// "10 rue de Rivoli 75001 Paris France"
```

Optional `Coordinates` for lat/long. Format for display with [Humanizer](humanizer.md) `IAddressFormatter`.

---

## Flag images

### Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Geography.Resources.Extensions;

services.AddGeographyFlags(); // ICountryFlagProvider → EmbeddedCountryFlagProvider
```

### Usage

```csharp
using MyNet.Geography;
using MyNet.Geography.Resources;

var flags = serviceProvider.GetRequiredService<ICountryFlagProvider>();

using var stream = flags.Open(Country.France, FlagSize.Pixel32);
var bytes = flags.GetBytes(Country.France, FlagSize.Pixel128);
```

`FlagSize`: `Pixel16`, `Pixel24`, `Pixel32`, `Pixel48`, `Pixel64`, `Pixel128`.

Embedded assets: `Flags/{size}/{alpha2}{size}.png` in Geography.Resources.

---

## Localized country names

Add **MyNet.Geography.Localization** and use Humanizer / display services with country resources (`CountryResources.resx`, `en` satellite).

```csharp
// After AddHumanizer + geography localization resources in DI:
var display = Country.France.Humanize();
```

---

## Google Maps (`MyNet.Google`)

All public types are under **`MyNet.Google.Maps`**.

### Geocoding

```csharp
using MyNet.Google.Maps;
using MyNet.Geography;

// GoogleLocationService — requires API key / HTTP setup in your host
ILocationService maps = CreateGoogleLocationService();

var region = maps.GetRegionFromCoordinates(48.8566, 2.3522);
var address = maps.GetAddressFromCoordinates(48.8566, 2.3522);

var coords = maps.GetCoordinatesFromAddress("10 Rue de Rivoli, Paris");
var coords2 = maps.GetCoordinatesFromAddress(postalAddress);
```

Handle `RequestDeniedException`, `QueryLimitExceededException` for quota/auth errors.

### Directions

```csharp
Directions directions = maps.GetDirections(
    originLat, originLng,
    destLat, destLng,
    travelMode: TravelMode.Driving);
```

### Open in browser

```csharp
using MyNet.Google.Maps;

var settings = new GoogleMapsSettings { /* query, zoom, map type */ };
GoogleMapsHelper.OpenGoogleMaps(settings);

// Extension on Address:
myAddress.OpenInGoogleMaps();
```

API keys and HTTP behavior are **host responsibilities** — the library builds requests and parses XML/JSON responses. See `tests/MyNet.Google.Tests`.

---

## With Fakers

```csharp
using MyNet.Fakers.Static;

var address = Faker.Addresses.Address();
var country = Faker.Countries.Country();
```

See [Fakers guide](fakers.md).

---

## Related

- [Humanizer](humanizer.md)
- [Fakers](fakers.md)
- [Globalization](globalization.md)
