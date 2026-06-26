<div align="center">

# MyNet.Temporal

<img src="../../assets/MyNetTemporal.png" alt="MyNet.Temporal" width="96" height="96" />

*TimeSpan decomposition, duration breakdown, and temporal helpers built on MyNet.Primitives for readable time representations.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Temporal)](https://www.nuget.org/packages/MyNet.Temporal)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Decomposition** | TimeSpan breakdown into readable parts |
| **Durations** | Helpers for UI labels and logging |
| **Primitives** | Built on MyNet.Primitives temporal types |
| **Humanizer** | Pairs with localized display formatters |

---

## Installation

```bash
dotnet add package MyNet.Temporal
```

## Quick start

```csharp
using MyNet.Temporal.Decomposition.Extensions;

var parts = TimeSpan.FromHours(25).Decompose();
```




---
## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)
- [MyNet.Humanizer](https://www.nuget.org/packages/MyNet.Humanizer)




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
