<div align="center">

# MyNet.Geography.Resources

<img src="../../assets/MyNetGeography.png" alt="MyNet.Geography.Resources" width="96" height="96" />

*Embedded multi-resolution country flag PNG assets and a default IFlagProvider for MyNet.Geography.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Geography.Resources)](https://www.nuget.org/packages/MyNet.Geography.Resources)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Flag assets** | Embedded multi-resolution country PNGs |
| **IFlagProvider** | Default implementation for Geography |
| **DI** | Registration via AddGeographyFlags() |
| **Satellite** | Optional resources for Geography consumers |

---

## Installation

```bash
dotnet add package MyNet.Geography.Resources
```

Requires **MyNet.Geography** (pulled automatically).

## Quick start

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Geography.Resources.Extensions;

var services = new ServiceCollection();
services.AddGeographyFlags();

// Resolve IFlagProvider and load images by country + FlagSize
```



---
## Related packages

- [MyNet.Geography](https://www.nuget.org/packages/MyNet.Geography)



---
## Documentation

- [Geography guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/geography.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
