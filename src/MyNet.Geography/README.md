<div align="center">

# MyNet.Geography

<img src="../../assets/MyNetGeography.png" alt="MyNet.Geography" width="96" height="96" />

*ISO 3166 countries and continents, postal addresses, coordinates, and a pluggable country-flag provider contract.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Geography)](https://www.nuget.org/packages/MyNet.Geography)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **ISO 3166** | Country and continent model |
| **Addresses** | Postal addresses and coordinates |
| **Flags** | Pluggable IFlagProvider contract |
| **Core types** | Shared geography model across the suite |

---

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



---
## Related packages

- [MyNet.Geography.Resources](https://www.nuget.org/packages/MyNet.Geography.Resources)
- [MyNet.Geography.Localization](https://www.nuget.org/packages/MyNet.Geography.Localization)
- [MyNet.Google](https://www.nuget.org/packages/MyNet.Google)



---
## Documentation

- [Geography guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/geography.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
