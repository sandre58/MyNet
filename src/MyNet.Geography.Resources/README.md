# MyNet.Geography.Resources

Embedded multi-resolution country flag PNG assets and a default `IFlagProvider` for MyNet.Geography.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Geography.Resources)](https://www.nuget.org/packages/MyNet.Geography.Resources)

**Target framework:** .NET 10

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

## Related packages

- [MyNet.Geography](https://www.nuget.org/packages/MyNet.Geography)

## Documentation

- [Geography guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/geography.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
