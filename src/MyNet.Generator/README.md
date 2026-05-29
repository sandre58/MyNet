<div align="center">

# MyNet.Generator

<img src="../../assets/MyNetGenerator.png" alt="MyNet.Generator" width="96" height="96" />

*Pseudo-random value generators, weighted sampling, and sequence helpers for unit tests, benchmarks, and prototyping.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Generator)](https://www.nuget.org/packages/MyNet.Generator)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **IRandomGenerator** | Facade for ints, strings, and collections |
| **Sampling** | Weighted picks and repeatable test doubles |
| **Culture-aware** | Random data building blocks per locale |
| **Lightweight** | No host framework assumptions |

---

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



---
## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)
- [MyNet.Fakers](https://www.nuget.org/packages/MyNet.Fakers) — higher-level locale-aware fakes



---
## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Fakers guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/fakers.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
