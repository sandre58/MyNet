# MyNet.Globalization

Culture-aware formatting, localization services, inflection, and translation resource registration for DI-based applications.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Globalization)](https://www.nuget.org/packages/MyNet.Globalization)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Globalization
```

## Quick start

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization.Extensions;

var services = new ServiceCollection();
services.AddGlobalization();
services.AddLocalization();
services.AddInflection();
```

## Related packages

- [MyNet.Text](https://www.nuget.org/packages/MyNet.Text)
- [MyNet.Humanizer](https://www.nuget.org/packages/MyNet.Humanizer)
- [MyNet.Observable](https://www.nuget.org/packages/MyNet.Observable)

## Documentation

- [Globalization guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/globalization.md) (includes translation system)
- [Documentation index](https://github.com/sandre58/MyNet2/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet2/blob/main/LICENSE).
