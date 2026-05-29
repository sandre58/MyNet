# MyNet.Generator

Pseudo-random value generators and sampling helpers for unit tests, benchmarks, and prototyping.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Generator)](https://www.nuget.org/packages/MyNet.Generator)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Generator
```

## Quick start

```csharp
using MyNet.Generator.Facade;

var value = RandomGenerator.Current.Int(1, 100);
var item = RandomGenerator.Current.Item(["a", "b", "c"]);
```

## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)
- [MyNet.Fakers](https://www.nuget.org/packages/MyNet.Fakers) — higher-level locale-aware fakes

## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Fakers guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/fakers.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
