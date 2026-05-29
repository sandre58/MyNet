<div align="center">

# MyNet.Primitives

<img src="../../assets/MyNetPrimitives.png" alt="MyNet.Primitives" width="96" height="96" />

*Foundation library for the MyNet suite: SmartEnum, intervals, unit conversions, comparers, sequences, and shared primitives.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Primitives)](https://www.nuget.org/packages/MyNet.Primitives)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **SmartEnum** | Strongly typed name/value pairs |
| **Intervals** | Ranges, sequences, and numeric bounds |
| **Conversions** | Unit conversions and physical quantities |
| **Comparers** | Guards, comparers, and minimal BCL-only dependencies |

---

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



---
## Related packages

Most MyNet packages depend on Primitives transitively. You rarely need to reference it explicitly unless you use only core types.



---
## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
