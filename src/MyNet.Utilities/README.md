# MyNet.Utilities

Cross-cutting utilities: caching, encryption, authentication helpers, progress reporting, threading, deferral, and DI-friendly services.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Utilities)](https://www.nuget.org/packages/MyNet.Utilities)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Utilities
```

## Quick start

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Utilities.Cache; // register cache services in your host

var services = new ServiceCollection();
// services.AddMemoryCache(); etc. — see package extensions and tests
```

Explore namespaces under `MyNet.Utilities` for cache, encryption, progress, and threading helpers.

## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)

## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
