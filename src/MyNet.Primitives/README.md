# MyNet.Primitives

Foundation library for the MyNet suite: SmartEnum, intervals, unit conversions, comparers, sequences, and shared primitives.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Primitives)](https://www.nuget.org/packages/MyNet.Primitives)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Primitives
```

## Quick start

```csharp
using MyNet.Primitives;
using MyNet.Primitives.Intervals;

public sealed class Priority : SmartEnum<Priority, int>
{
    public static readonly Priority Low = new(nameof(Low), 0);
    public static readonly Priority High = new(nameof(High), 1);

    private Priority(string name, int value) : base(name, value) { }
}

var range = new NumericRange<int>(1, 10);
```

## Related packages

Most MyNet packages depend on Primitives transitively. You rarely need to reference it explicitly unless you use only core types.

## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
