# MyNet.Geography

ISO 3166 countries and continents, postal addresses, coordinates, and a pluggable country-flag provider contract.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Geography)](https://www.nuget.org/packages/MyNet.Geography)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Geography
```

## Quick start

```csharp
using MyNet.Geography;

var france = Country.France;
var isoAlpha2 = france.Alpha2; // "fr"
```

For flag images, add **MyNet.Geography.Resources**. For localized display names, add **MyNet.Geography.Localization**.

## Related packages

- [MyNet.Geography.Resources](https://www.nuget.org/packages/MyNet.Geography.Resources)
- [MyNet.Geography.Localization](https://www.nuget.org/packages/MyNet.Geography.Localization)
- [MyNet.Google](https://www.nuget.org/packages/MyNet.Google)

## Documentation

- [Geography guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/geography.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
