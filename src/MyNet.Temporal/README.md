# MyNet.Temporal

TimeSpan decomposition, duration breakdown, and temporal helpers built on MyNet.Primitives.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Temporal)](https://www.nuget.org/packages/MyNet.Temporal)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Temporal
```

## Quick start

```csharp
using MyNet.Temporal.Decomposition.Extensions;

var parts = TimeSpan.FromHours(25).Decompose();
```

## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)
- [MyNet.Humanizer](https://www.nuget.org/packages/MyNet.Humanizer)

## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
